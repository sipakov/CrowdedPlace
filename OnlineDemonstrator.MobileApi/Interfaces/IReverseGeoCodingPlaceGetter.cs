using System.Threading.Tasks;
using OnlineDemonstrator.MobileApi.Models;

namespace OnlineDemonstrator.MobileApi.Interfaces
{
    public interface IReverseGeoCodingPlaceGetter
    {
        Task<Address> GetAddressByGeoPosition(double latitude, double longitude);
    }
}