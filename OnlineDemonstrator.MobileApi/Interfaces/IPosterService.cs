using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Models;

namespace OnlineDemonstrator.MobileApi.Interfaces
{
    public interface IPosterService
    {
        Task<Poster> AddPosterAsync(PosterIn posterIn, DateTime currentDateTime);

        Task<List<PosterOut>> GetFromActualDemonstrations(int postersCountInDemonstration);

        Task<List<PosterOut>> GetPostersByDemonstrationId(int demonstrationId);

        Task<PosterOut> GetPosterById(Guid deviceId, DateTime createdDate, int demonstrationId);
        
        Task<Poster> AddPosterToExistDemonstrationAsync(PosterIn posterIn, DateTime currentDateTime);

        Task<Poster> AddPosterToExpiredDemonstrationAsync(PosterIn posterIn, DateTime currentDateTime);

    }
}