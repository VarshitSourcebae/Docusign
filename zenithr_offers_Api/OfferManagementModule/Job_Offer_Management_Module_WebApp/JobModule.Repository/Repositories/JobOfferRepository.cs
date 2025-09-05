using JobModule.Domain.Entities;
using JobModule.Domain.Enums;
using JobModule.Domain.RepoInterfaces;
using JobModule.Repository.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Repository.Repositories
{
	public class JobOfferRepository : IJobOfferRepository
	{
		private readonly ApplicationDbContext _dbcontext;

		public JobOfferRepository(ApplicationDbContext context)
		{
				_dbcontext = context;
		}

		public async Task<JobOffer> AddAsync(JobOffer jobOffer)
		{
			_dbcontext.MyProperty.Add(jobOffer);
			await _dbcontext.SaveChangesAsync();
			return jobOffer;
		}

		public async Task<JobOffer?> GetByIdAsync(int id) =>
		await _dbcontext.MyProperty.FindAsync(id);

		public async Task UpdateAsync(JobOffer jobOffer)
		{
			_dbcontext.MyProperty.Update(jobOffer);
			await _dbcontext.SaveChangesAsync();
		}
		public async Task UpdateByEnvelopeIdAsync(string envId, JobOfferStatus status)
		{
			var jobOffer = await _dbcontext.MyProperty.FirstOrDefaultAsync(x => x.EnvelopeId == envId);

			if (jobOffer == null)
			{
				throw new KeyNotFoundException($"No Record found with EnvelopeId: {envId}");
			}

			jobOffer.Status = status;
			jobOffer.UpdatedAt = DateTime.UtcNow;

			_dbcontext.MyProperty.Update(jobOffer);
			await _dbcontext.SaveChangesAsync();
		}
	}
}
