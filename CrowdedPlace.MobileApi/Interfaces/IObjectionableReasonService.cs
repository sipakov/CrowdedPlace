using System.Threading.Tasks;
using CrowdedPlace.Libraries.Domain.Entities;
using CrowdedPlace.Libraries.Domain.Models;

namespace CrowdedPlace.MobileApi.Interfaces
{
    public interface IObjectionableReasonService
    {
        Task<BaseResult> AddAsync(ObjectionableContent objectionableContent);
    }
}