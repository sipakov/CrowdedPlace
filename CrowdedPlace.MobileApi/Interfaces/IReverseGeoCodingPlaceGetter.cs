using System.Threading.Tasks;
using CrowdedPlace.MobileApi.Models;

namespace CrowdedPlace.MobileApi.Interfaces
{
    public interface IReverseGeoCodingPlaceGetter
    {
        Task<Address> GetAddressByGeoPosition(double latitude, double longitude, string locale);
    }
}