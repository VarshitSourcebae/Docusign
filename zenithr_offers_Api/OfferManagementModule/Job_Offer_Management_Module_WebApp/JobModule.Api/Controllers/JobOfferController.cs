using JobModule.Services.Dtos;
using JobModule.Services.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobModule.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class JobOfferController : ControllerBase
	{
		private readonly IJobOfferService _service;

		public JobOfferController(IJobOfferService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] JobOfferCreateDto dto)
		{
			var result = await _service.CreateJobOfferAsync(dto);
			return Ok(result);
		}

		[HttpGet("{id}/pdf")]
		public async Task<IActionResult> GetPdf(int id)
		{
			var pdfBytes = await _service.GetPdfAsync(id);
			return File(pdfBytes, "application/pdf", $"offer_{id}.pdf");
		}

		[HttpGet("status/{envelopeId}")]
		public async Task<IActionResult> GetEnvelopeStatus(string envelopeId)
		{
			try
			{
				var status = await _service.GetEnvelopeStatusAsync(envelopeId);

				return Ok(new { EnvelopeId = envelopeId, Status = status });
			}
			catch (Exception ex)
			{
				return BadRequest(new { Error = ex.Message });
			}
		}
		[HttpPost("send/{id}")]
		public async Task<IActionResult> SendEnvelope(int id)
		{
			try
			{
				var envelopeId = await _service.SendEnvelopeAsync(id);

				return Ok(new { EnvelopeId = envelopeId });
			}
			catch (Exception ex)
			{
				return BadRequest(new { Error = ex.Message });
			}
		}

		[HttpGet("document/{envelopeId}")]
		public async Task<IActionResult> GetSignedDocument(string envelopeId)
		{
			try
			{

				var fileBytes = await _service.GetSignedPdfAsync(envelopeId);

				return File(fileBytes, "application/pdf", $"Signed_{envelopeId}.pdf");
			}
			catch (Exception ex)
			{
				return BadRequest(new { Error = ex.Message });
			}
		}
	}
}
