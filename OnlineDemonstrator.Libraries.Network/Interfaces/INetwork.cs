using System.Threading.Tasks;
using OnlineDemonstrator.Libraries.Domain.Models;

namespace OnlineDemonstrator.Libraries.Network.Interfaces
{
    public interface INetwork
    {
        Task<(BaseResult baseResult, string response)> LoadDataPostAsync(string url, string serializedObj, string bearerToken);
        
        Task<(BaseResult baseResult, string response)> LoadDataGetAsync(string url, string bearerToken);

        Task<(BaseResult baseResult, string response)> LoadFilePostAsync(string url, byte[] file, string bearerToken);
    }
}