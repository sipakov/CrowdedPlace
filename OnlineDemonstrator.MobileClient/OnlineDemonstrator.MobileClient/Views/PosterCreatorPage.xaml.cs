using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using OnlineDemonstrator.Libraries.Domain;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Url;
using OnlineDemonstrator.Libraries.Network.Interfaces;
using OnlineDemonstrator.MobileClient.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OnlineDemonstrator.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PosterCreatorPage : ContentPage
    {
        private readonly INetwork _network;
        private readonly double _latitude;
        private readonly double _longitude;
        private readonly string _geocodeAddressCountry;
        private readonly string _geocodeAddressCity;
        private readonly string _geocodeAddressArea;
        private readonly Guid _deviceId;
        private const string PatternToValidateMessageTitle = @"^[a-zA-Zа-яА-Я0-9@#$%&*+\-_(),+':;?.,!\[\]\s\\/]+$";

        public PosterCreatorPage(double latitude, double longitude, string geocodeAddressCountry, string geocodeAddressCity, string geocodeAddressArea, Guid deviceId)
        {
            _latitude = latitude;
            _longitude = longitude;
            _geocodeAddressCountry = geocodeAddressCountry;
            _geocodeAddressCity = geocodeAddressCity;
            _geocodeAddressArea = geocodeAddressArea;
            _deviceId = deviceId;
            
            InitializeComponent();
            _network = App.Container.Resolve<INetwork>();
            
        }

        private async Task AddPoster(double latitude, double longitude, string geocodeAddressCountry, string geocodeAddressCity, string geocodeAddressArea, string name, string title, string message, Guid deviceId)
        {
            var fullUrl = $"{Url.AddPoster}";
            
            var poster = new PosterIn
            {
                Name = name,
                Title = title,
                Message = message,
                Latitude = latitude,
                Longitude = longitude,
                CountryName = geocodeAddressCountry,
                CityName = geocodeAddressCity,
                AreaName = geocodeAddressArea,
                DeviceId = deviceId
            };
            var serializedObj = JsonConvert.SerializeObject(poster);

            var (baseResult, content) = await _network.LoadDataPostAsync(fullUrl,  serializedObj, null);

            switch (baseResult.Result)
            {
                case StatusCode.Ok:
                    await Navigation.PopModalAsync(true);
                    break;
                default:
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    break;
            }
        }
        
        private async void OnAddPosterButtonClicked(object sender, EventArgs e)
        {

            var isValid = ValidateMessageTitle();
            if (!isValid)
            {
                await DisplayAlert(AppResources.Notification, AppResources.MessageTitleAreRequired, AppResources.Ok);
                return;
            }

            var name = NameEditor.Text;
            var title = TitleEditor.Text;
            var message = MessageEditor.Text;
            await AddPoster(_latitude, _longitude, _geocodeAddressCountry, _geocodeAddressCity, _geocodeAddressArea, name, title, message, _deviceId);
        }
        
        private bool ValidateMessageTitle()
        {
            return !string.IsNullOrEmpty(MessageEditor.Text) && !string.IsNullOrEmpty(TitleEditor.Text);
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<object> (this, "ReloadPosters");
            //Navigation.PopModalAsync(true);
        }
    }
}