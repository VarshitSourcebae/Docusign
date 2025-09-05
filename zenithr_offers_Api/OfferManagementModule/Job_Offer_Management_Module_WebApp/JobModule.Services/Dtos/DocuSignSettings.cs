using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Services.Dtos
{
	public class DocuSignSettings
	{
		public string IntegrationKey { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		public string AuthServer { get; set; } = string.Empty;
		public string BasePath { get; set; } = string.Empty;
		public string AccountId { get; set; } = string.Empty;
		public string PrivateKeyFile { get; set; } = string.Empty;
	}
}	
