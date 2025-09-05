using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Services.Dtos
{
	public class JobOfferResponseDto
	{
		public int Id { get; set; }
		public string RecipientName { get; set; } = string.Empty;
		public string RecipientEmail { get; set; } = string.Empty;
		public string PdfFilePath { get; set; } = string.Empty;
		public string? EnvelopeId { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
