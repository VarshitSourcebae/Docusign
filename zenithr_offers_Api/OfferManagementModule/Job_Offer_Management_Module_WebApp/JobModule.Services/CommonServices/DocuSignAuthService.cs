using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.eSign.Model;
using JobModule.Domain.Entities;
using JobModule.Services.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Crypto.IO;
using QuestPDF;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Services.CommonServices
{
	public interface IDocuSignAuthService 
	{
		Task<string> GenerateAccessTokenAsync();
		Task<string> SendEnvelopeAsync(JobOffer jobOffer);
		Task<string> GetEnvelopeStatusAsync(string envelopeId);
		Task<byte[]> GetSignedDocumentAsync(string envelopeId);
	}
	public class DocuSignAuthService: IDocuSignAuthService
	{
		private readonly DocuSignSettings _settings;
		private readonly ApiClient _apiClient;

		public DocuSignAuthService(IOptions<DocuSignSettings> settings)
		{
			_settings = settings.Value;
			_apiClient = new ApiClient(_settings.BasePath);
		}

		public async Task<string> GenerateAccessTokenAsync()
		{
			try
			{
				string privateKey = File.ReadAllText(_settings.PrivateKeyFile);

				var scopes = new List<string> { "signature", "impersonation" };

				OAuth.OAuthToken token = await _apiClient.RequestJWTUserTokenAsync(
					_settings.IntegrationKey,
					_settings.UserId,
					_settings.AuthServer, // must be "account-d.docusign.com"
					Encoding.ASCII.GetBytes(privateKey),
					1,
					scopes);

				return token.access_token;
			}
			catch (ApiException apiEx)
			{
				throw new Exception($"DocuSign API Error: {apiEx.Message} | Response: {apiEx.ErrorContent}");
			}
			catch (Exception ex)
			{
				throw new Exception( $"General Error: {ex.Message}");
			}
		}

		public async Task<string> SendEnvelopeAsync(JobOffer jobOffer)
		{
			var accessToken = await GenerateAccessTokenAsync();
			var userInfo = await _apiClient.GetUserInfoAsync(accessToken);
			var account = userInfo.Accounts.FirstOrDefault();
			var accountId = account.AccountId;
			// Attach token
			_apiClient.Configuration.DefaultHeader.Remove("Authorization");
			_apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

			var envelopesApi = new EnvelopesApi(_apiClient);

			// Load a document
			var docBytes = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "OfferLetterStaticFile", jobOffer.PdfFilePath.TrimStart('/')));

			var document = new Document
			{
				DocumentBase64 = Convert.ToBase64String(docBytes),
				Name = jobOffer.RecipientName, // will show in DocuSign UI
				FileExtension = "pdf",
				DocumentId = jobOffer.Id.ToString()
			};

			// Define signer
			var signer = new Signer
			{
				Email = jobOffer.RecipientEmail,
				Name = jobOffer.RecipientName,
				RecipientId = jobOffer.Id.ToString(),
				RoutingOrder = "1"
			};

			// Add a SignHere tab (coordinates are optional if you want free placement)
			signer.Tabs = new Tabs
			{
				SignHereTabs = new List<SignHere>
					{
						new SignHere
						{
							AnchorString = "/sig1/",
							AnchorUnits = "pixels",
							AnchorXOffset = "0",
							AnchorYOffset = "0"
						}
					}
			};

			// Create envelope definition
			var envelopeDefinition = new EnvelopeDefinition
			{
				EmailSubject = "Please sign this document",
				Documents = new List<Document> { document },
				Recipients = new Recipients { Signers = new List<Signer> { signer } },
				Status = "sent" // "created" = draft, "sent" = send immediately
			};

			// Send the envelope
			var results = await envelopesApi.CreateEnvelopeAsync(accountId, envelopeDefinition);

			return results.EnvelopeId;
		}

		public async Task<string> GetEnvelopeStatusAsync(string envelopeId)
		{
			var accessToken = await GenerateAccessTokenAsync();
			var userInfo = await _apiClient.GetUserInfoAsync(accessToken);
			var accountId = userInfo.Accounts.First().AccountId;
			// Attach token
			_apiClient.Configuration.DefaultHeader.Remove("Authorization");
			_apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

			var envelopesApi = new EnvelopesApi(_apiClient);

			Envelope envelope = await envelopesApi.GetEnvelopeAsync(accountId, envelopeId);

			return envelope.Status; // "sent", "completed", "declined", etc.
		}

		public async Task<byte[]> GetSignedDocumentAsync(string envelopeId)
		{
			var accessToken = await GenerateAccessTokenAsync();
			var userInfo = await _apiClient.GetUserInfoAsync(accessToken);
			var accountId = userInfo.Accounts.First().AccountId;
			// Attach token
			_apiClient.Configuration.DefaultHeader.Remove("Authorization");
			_apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

			var envelopesApi = new EnvelopesApi(_apiClient);

			// Usually the final signed document is "1"
			var documentBytes = envelopesApi.GetDocument(accountId, envelopeId, "1");

			using (var ms = new MemoryStream())
			{
				await documentBytes.CopyToAsync(ms);
				return ms.ToArray();
			}
		}
	}
}
