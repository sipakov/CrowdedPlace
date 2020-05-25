using System.Threading.Tasks;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Models;

namespace OnlineDemonstrator.MobileApi.Interfaces
{
    public interface IObjectionableReasonService
    {
        Task<BaseResult> AddAsync(ObjectionableContent objectionableContent);
    }
}