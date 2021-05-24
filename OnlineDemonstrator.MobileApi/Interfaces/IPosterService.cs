using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;

namespace OnlineDemonstrator.MobileApi.Interfaces
{
    public interface IPosterService
    {
        Task<Poster> AddPosterAsync(PosterIn posterIn, DateTime currentDateTime);

        Task<List<PosterOut>> GetFromActualDemonstrations(int postersCountInDemonstration);

        Task<List<PosterOut>> GetPostersByDemonstrationId(int demonstrationId);

        Task<PosterOut> GetPosterById(string deviceId, DateTime createdDate, int demonstrationId);
        
        Task<Poster> AddPosterToExistDemonstrationAsync(PosterIn posterIn, DateTime currentDateTime);

        Task<Poster> AddPosterToExpiredDemonstrationAsync(PosterIn posterIn, DateTime currentDateTime);

    }
}