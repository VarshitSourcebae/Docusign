using JobModule.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Services.ServiceInterface
{
	public interface IJobOfferService
	{
		Task<JobOfferResponseDto> CreateJobOfferAsync(JobOfferCreateDto dto);
		Task<byte[]> GetPdfAsync(int id);
		Task<string> SendEnvelopeAsync(int id);
		Task<string> GetEnvelopeStatusAsync(string envelopeId);
		Task<byte[]> GetSignedPdfAsync(string envelopeId);
	}
}
