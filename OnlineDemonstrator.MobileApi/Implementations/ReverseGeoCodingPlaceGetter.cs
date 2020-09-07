using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineDemonstrator.MobileApi.Interfaces;
using OnlineDemonstrator.MobileApi.Models;

namespace OnlineDemonstrator.MobileApi.Implementations
{
    public class ReverseGeoCodingPlaceGetter : IReverseGeoCodingPlaceGetter
    {
        private readonly IConfiguration _config;

        public ReverseGeoCodingPlaceGetter(IConfiguration config)
        {
            _config = config;
        }
        
        public async Task<Address> GetAddressByGeoPosition(double latitude, double longitude, string locale)
        {
            var key = _config.GetSection("KeyApiGoogle").Value;
            
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}&key={key}&language={locale}";
            
            var request = WebRequest.Create(url);
            var response = await request.GetResponseAsync();
            
            var newStream = response.GetResponseStream();
            var sr = new StreamReader(newStream ?? throw new InvalidOperationException());    
            var result = await sr.ReadToEndAsync();
            await newStream.DisposeAsync();

            GoogleGeoCodeResponse geoAddress;
            try
            {
                geoAddress = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(result);
            }
            catch (Exception)
            {
                return new Address();
            }

            return new Address
            {
                FormattedAddress = geoAddress.status == "OK" ? geoAddress.results[0].formatted_address : string.Empty
            };
        }
    }
}