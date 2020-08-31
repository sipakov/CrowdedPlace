using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using OnlineDemonstrator.Libraries.Domain;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Enums;
using OnlineDemonstrator.Libraries.Domain.Url;
using OnlineDemonstrator.Libraries.Network.Interfaces;
using OnlineDemonstrator.MobileClient.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Xamarin.Forms.Markup;
using Map = Xamarin.Forms.Maps.Map;

namespace OnlineDemonstrator.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private readonly INetwork _network;
        private readonly Guid _deviceId;
        private int _demonstrationId;
        private int _postersCountInDemonstration;
        private const double MinDistanceInDegreesForReloadPosters = 0.05;
        private double? _lastLatitude;
        private double? _lastLongitude;

        public MapPage(Guid deviceId)
        {
            _deviceId = deviceId;
            InitializeComponent();
            const bool appIsOpened = true;
            _network = App.Container.Resolve<INetwork>();
            
            _ = GetAllActualPosters(appIsOpened);
            _ = GetDeviceById(_deviceId);
            MessagingCenter.Subscribe<DemonstrationsPage, MapSpan>(this, "MoveTo", (sender, mapSpan) =>
            {
                Map.MoveToRegion(mapSpan);
                var s = Map.VisibleRegion;
            });
            MessagingCenter.Subscribe<object>(this, "ReloadPosters", (sender) =>
            {
                _ = GetAllActualPosters();
            });
            Map.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                var m = (Map) sender;
                if (m.VisibleRegion == null) return;
                
                var currentLatitude = m.VisibleRegion?.Center.Latitude;
                var currentLongitude = m.VisibleRegion?.Center.Longitude;
                
                _ = GetNearestDemonstration();
                if (_lastLatitude != null && _lastLongitude != null)
                {
                    if (!(Math.Abs(currentLatitude.Value) - Math.Abs(_lastLatitude.Value) >
                          MinDistanceInDegreesForReloadPosters) &&
                        !(Math.Abs(currentLongitude.Value) - Math.Abs(_lastLongitude.Value) >
                          MinDistanceInDegreesForReloadPosters)) return;
                    _ = GetAllActualPosters();    
                    _lastLatitude = currentLatitude;
                    _lastLongitude = currentLongitude;
                }
                else
                {
                    _lastLatitude = currentLatitude;
                    _lastLongitude = currentLongitude;
                }
            };
        }

        private async Task GetAllActualPosters(bool appIsOpened = false)
        {
            const int postersCountInDemonstration = 10;
            const double latitudeDegrees = 0.007;
            const double longitudeDegrees = 0.007;
            var fullUrl = $"{Url.GetAllActualPosters}{postersCountInDemonstration}";
            var (baseResult, content) = await _network.LoadDataGetAsync(fullUrl, null);

            switch (baseResult.Result)
            {
                case StatusCode.Ok:
                    var actualPosters = JsonConvert.DeserializeObject<List<PosterOut>>(content);
                    Map.Pins.Clear();
                    if (appIsOpened && actualPosters.Any())
                    {
                        var firstAnyPoster = actualPosters.First();
                        var startPosition = new Position(firstAnyPoster.Latitude, firstAnyPoster.Longitude);
                        var startMapSpan = new MapSpan(startPosition, latitudeDegrees,longitudeDegrees);
                        Map.MoveToRegion(startMapSpan);    
                    }
                    foreach (var actualPoster in actualPosters)
                    {
                        var newPin = new Pin
                        {
                            Label = actualPoster.Title,
                            Address = actualPoster.Name,
                            Type = PinType.Generic,
                            Position = new Position(actualPoster.Latitude, actualPoster.Longitude)
                        };
                        newPin.InfoWindowClicked += async (s, args) =>
                        {
                            try
                            {
                                await Navigation.PushModalAsync(new Poster(actualPoster.DeviceId, actualPoster.CreatedDate, _deviceId));
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"{AppResources.NotificationError}, {e.InnerException}");
                            }
                        };
                        Map.Pins.Add(newPin);
                    }
                    break;
                default:
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    break;
            }
        }

        private async void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            var geocodeAddressCountry = string.Empty;
            var geocodeAddressCity = string.Empty;
            var geocodeAddressArea = string.Empty;
            List<Placemark> locationInfos = new List<Placemark>();
          
            try
            {
                locationInfos =
                    (await Geocoding.GetPlacemarksAsync(e.Position.Latitude, e.Position.Longitude).ConfigureAwait(true))
                    .ToList();
            }
            catch (Exception exception)
            {
                locationInfos =
                    (await Geocoding.GetPlacemarksAsync(e.Position.Latitude, e.Position.Longitude).ConfigureAwait(true))
                    .ToList();
            }
            
            var locationInfo = locationInfos?.FirstOrDefault();

            if (locationInfo != null)
            {
                geocodeAddressCountry = $"{locationInfo.CountryName}";
                geocodeAddressCity = $"{locationInfo.Locality}";
                geocodeAddressArea = $"{locationInfo.FeatureName}";
            }

            await Navigation.PushModalAsync(
                new PosterCreatorPage(e.Position.Latitude, e.Position.Longitude, geocodeAddressCountry,
                    geocodeAddressCity, geocodeAddressArea, _deviceId), true);
        }

        private async void OnImageButtonClicked(object sender, EventArgs e)
        {
            var center = Map.VisibleRegion.Center;
            var geocodeAddressCountry = string.Empty;
            var geocodeAddressCity = string.Empty;
            var geocodeAddressArea = string.Empty;

            List<Placemark> locationInfos = new List<Placemark>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                locationInfos = (await Geocoding.GetPlacemarksAsync(center.Latitude, center.Longitude).ConfigureAwait(false)).ToList();
            });

                var locationInfo = locationInfos?.FirstOrDefault();

                if (locationInfo != null)
                {
                    geocodeAddressCountry = $"{locationInfo.CountryName}";
                    geocodeAddressCity = $"{locationInfo.Locality}";
                    geocodeAddressArea = $"{locationInfo.FeatureName}"; 
                }
            

            await Navigation.PushModalAsync(
                new PosterCreatorPage(center.Latitude, center.Longitude, geocodeAddressCountry,
                    geocodeAddressCity, geocodeAddressArea, _deviceId), true);
        }

        private async Task GetNearestDemonstration()
        {
            var centerOfCurrentRegion = Map.VisibleRegion.Center;
            const int radiusForLookingDemo = 5;
            var fullUrl = $"{Url.GetNearestDemonstration}";
            var pointsIn = new PointsIn
            {
                Latitude = centerOfCurrentRegion.Latitude,
                Longitude = centerOfCurrentRegion.Longitude,
                RadiusForLookingDemo = radiusForLookingDemo
            };
            var serializableObj = JsonConvert.SerializeObject(pointsIn);
            var (baseResult, content) = await _network.LoadDataPostAsync(fullUrl, serializableObj, null);

            switch (baseResult.Result)
            {
                case StatusCode.Ok:
                    var nearestDemonstration = JsonConvert.DeserializeObject<DemonstrationOut>(content);
                    if (nearestDemonstration.Id > 0)
                    {
                        _postersCountInDemonstration = nearestDemonstration.PostersCount;
                        EverythingFromThisDemoLabelTop.Text = $"{AppResources.EverythingFromThisDemo}";
                        EverythingFromThisDemoLabelBottom.Text = $"{nearestDemonstration.DetailName}. {AppResources.NumberOfParticipants} {nearestDemonstration.PostersCount}";
                        EverythingFromThisDemoButton.IsVisible = true;
                        _demonstrationId = nearestDemonstration.Id;
                    }
                    else
                    {
                        EverythingFromThisDemoButton.IsVisible = false;
                        _demonstrationId = default;
                    }

                    break;
                default:
                    _demonstrationId = default;
                    EverythingFromThisDemoButton.IsVisible = false;
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    break;
            }
        }
        
        private async Task GetDeviceById(Guid deviceId)
        {
            var fullUrl = $"{Url.GetDeviceById}";

            var device = new DeviceIn
            {
                DeviceId = deviceId
            };
            var serializableObj = JsonConvert.SerializeObject(device);
            var (baseResult, content) = await _network.LoadDataPostAsync(fullUrl, serializableObj, null);

            switch (baseResult.Result)
            {
                case StatusCode.Ok:
                    break;
                case StatusCode.NoContent:
                    await AddDeviceAsync(deviceId);
                    break;
                case StatusCode.Forbidden:
                    await Navigation.PushModalAsync(new LicensePage(false, deviceId));
                    break;
                default:
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    break;
            }
        }
        
        private async Task AddDeviceAsync(Guid deviceId)
        {
            var fullUrl = $"{Url.AddDevice}";

            var device = new DeviceIn
            {
                DeviceId = deviceId,
                IsLicenseActivated = false
            };
            if (Device.RuntimePlatform == Device.iOS)
            {
                device.OsId = (int)OperationSystems.Ios;
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                device.OsId = (int)OperationSystems.Android;
            }
            
            var serializableObj = JsonConvert.SerializeObject(device);
            var (baseResult, content) = await _network.LoadDataPostAsync(fullUrl, serializableObj, null);

            switch (baseResult.Result)
            {
                case StatusCode.Ok:
                    await DisplayAlert(AppResources.DearUsers, AppResources.PrivacyDisplayMessage, AppResources.Ok);
                    await Navigation.PopAsync();
                    break;
                default:
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    break;
            }   
        }

        private async void OnEverythingFromThisDemoClicked(object sender, EventArgs eventArgs)
        {
            await Navigation.PushModalAsync(new Posters(_demonstrationId, _deviceId, _postersCountInDemonstration));
        }


        public async void OnOtherDemonstrationsClicked(object sender, EventArgs eventArgs)
        {
            await Navigation.PushModalAsync(new DemonstrationsPage());
        }
        
        // protected override async void OnAppearing()
        // {
        //     base.OnAppearing();
        //     _ = GetAllActualPosters();
        // }
    }
}