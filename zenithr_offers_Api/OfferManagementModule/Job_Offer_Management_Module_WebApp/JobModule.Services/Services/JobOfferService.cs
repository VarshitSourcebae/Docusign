using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using JobModule.Domain.Entities;
using JobModule.Domain.Enums;
using JobModule.Domain.RepoInterfaces;
using JobModule.Services.CommonServices;
using JobModule.Services.Dtos;
using JobModule.Services.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Services.Services
{
	public class JobOfferService : IJobOfferService
	{
		private readonly IJobOfferRepository _JobOfferRepo;
		private readonly IPdfGeneratorService _pdfGenerator;
		private readonly IDocuSignAuthService _docauth;

		public JobOfferService(IJobOfferRepository repository, IPdfGeneratorService pdfGenerator,IDocuSignAuthService Iauth)
		{
			_JobOfferRepo = repository;
			_pdfGenerator = pdfGenerator;
			_docauth = Iauth;
		}

		public async Task<JobOfferResponseDto> CreateJobOfferAsync(JobOfferCreateDto dto)
		{
			var jobOffer = new JobOffer
			{
				RecipientName = dto.RecipientName,
				RecipientEmail = dto.RecipientEmail,
				OfferContent = dto.OfferContent
			};

			await _JobOfferRepo.AddAsync(jobOffer);

			jobOffer.PdfFilePath = _pdfGenerator.GeneratePdf(jobOffer);
			await _JobOfferRepo.UpdateAsync(jobOffer);

			return MapToResponse(jobOffer);
		}

		public async Task<byte[]> GetPdfAsync(int id)
		{
			var jobOffer = await _JobOfferRepo.GetByIdAsync(id) ?? throw new Exception("Not found");
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "OfferLetterStaticFile", jobOffer.PdfFilePath.TrimStart('/'));
			return await File.ReadAllBytesAsync(filePath);
		}

		public async Task<string> SendEnvelopeAsync(int id)
		{
			try
			{
				var jobOffer = await _JobOfferRepo.GetByIdAsync(id) ?? throw new Exception("Not found");
				var res = await _docauth.SendEnvelopeAsync(jobOffer);
				jobOffer.EnvelopeId = res;
				jobOffer.Status = JobOfferStatus.Sent;
				await _JobOfferRepo.UpdateAsync(jobOffer);
				return res;
			}
			catch (Exception ex) {
				return ex.Message;
			}
		}
		public async Task<string> GetEnvelopeStatusAsync(string envelopeId)
		{
			try
			{
				var res = await _docauth.GetEnvelopeStatusAsync(envelopeId);
				if(res == "delivered")
					await _JobOfferRepo.UpdateByEnvelopeIdAsync(envelopeId, JobOfferStatus.Delivered);
				else if (res == "completed")
					await _JobOfferRepo.UpdateByEnvelopeIdAsync(envelopeId, JobOfferStatus.Completed);
				return res;
			}
			catch (Exception ex) {
				return ex.Message;
			}
		}


		public async Task<byte[]> GetSignedPdfAsync(string envelopeId)
		{
			var res = await _docauth.GetEnvelopeStatusAsync(envelopeId);

			if (res == "completed")
				return await _docauth.GetSignedDocumentAsync(envelopeId);

			return null;
		}


		#region Map response 
		private static JobOfferResponseDto MapToResponse(JobOffer jobOffer) => new JobOfferResponseDto
		{
			Id = jobOffer.Id,
			RecipientName = jobOffer.RecipientName,
			RecipientEmail = jobOffer.RecipientEmail,
			PdfFilePath = jobOffer.PdfFilePath,
			EnvelopeId = jobOffer.EnvelopeId,
			Status = jobOffer.Status.ToString()
		};
		#endregion
	}
}
