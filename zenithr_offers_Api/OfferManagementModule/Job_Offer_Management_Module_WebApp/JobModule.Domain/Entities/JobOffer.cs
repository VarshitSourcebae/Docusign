using JobModule.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Domain.Entities
{
	public class JobOffer
	{
		[Key]
		public int Id { get; set; }
		public string RecipientName { get; set; } = string.Empty;
		public string RecipientEmail { get; set; } = string.Empty;
		public string OfferContent { get; set; } = string.Empty;
		public string PdfFilePath { get; set; } = string.Empty;
		public string? EnvelopeId { get; set; }
		public JobOfferStatus Status { get; set; } = JobOfferStatus.Draft;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
