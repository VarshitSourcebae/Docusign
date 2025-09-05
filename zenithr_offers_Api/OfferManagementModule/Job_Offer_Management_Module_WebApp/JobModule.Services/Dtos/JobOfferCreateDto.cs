using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Services.Dtos
{
	public class JobOfferCreateDto
	{
		public string RecipientName { get; set; } = string.Empty;
		public string RecipientEmail { get; set; } = string.Empty;
		public string OfferContent { get; set; } = string.Empty;
	}
}
