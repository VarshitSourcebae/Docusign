using JobModule.Domain.Entities;
using JobModule.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobModule.Domain.RepoInterfaces
{
	public interface IJobOfferRepository
	{
		Task<JobOffer> AddAsync(JobOffer jobOffer);
		Task UpdateAsync(JobOffer jobOffer);
		Task<JobOffer?> GetByIdAsync(int id);
		Task UpdateByEnvelopeIdAsync(string envId, JobOfferStatus status);
	}
}
