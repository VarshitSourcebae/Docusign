using JobModule.Domain.Entities;
using JobModule.Services.Dtos;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Services.CommonServices
{
	public interface IPdfGeneratorService
	{
		string GeneratePdf(JobOffer jobOffer);
	}

	public class PDFGenerator : IPdfGeneratorService
	{
		private readonly PdfStorageOptions _options;

		public PDFGenerator(IOptions<PdfStorageOptions> options)
		{
			_options = options.Value;
		}
		public string GeneratePdf(JobOffer jobOffer)
		{
			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), _options.JobOffers);

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			var filePath = Path.Combine(folderPath, $"offer_{jobOffer.Id}.pdf");

			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(50);
					page.Content().Column(col =>
					{
						col.Item().Text("Job Offer Letter").FontSize(20).Bold();
						col.Item().Text($"Recipient: {jobOffer.RecipientName} ({jobOffer.RecipientEmail})");
						col.Item().Text($"Date: {DateTime.UtcNow:dd MMM yyyy}");
						col.Item().Text(jobOffer.OfferContent).FontSize(12);
					});
				});
			});

			try
			{
				document.GeneratePdf(filePath);
			}
			catch (Exception ex) 
			{
				throw ex;
			}
			// Return relative URL (for Angular to use directly)
			return $"/offer_{jobOffer.Id}.pdf";
		}
	}
}
