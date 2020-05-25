using System;
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
using Xamarin.Forms.Xaml;

namespace OnlineDemonstrator.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LicensePage : ContentPage
    {
        private readonly INetwork _network;
        private readonly Guid _deviceId;
        public LicensePage(bool isModal, Guid deviceId)
        {
            InitializeComponent();
            _network = App.Container.Resolve<INetwork>();
            _deviceId = deviceId;
            OnButtonAgree.IsEnabled = false;
            CloseButton.IsVisible = isModal;
            OnButtonAgree.IsVisible = !isModal;
        }
        
        private async void OnButtonAgreeClicked(object sender, EventArgs e)
        {
            await AddDeviceAsync(_deviceId);
        }

        private async Task AddDeviceAsync(Guid deviceId)
        {
            var fullUrl = $"{Url.AddDevice}";

            var device = new DeviceIn
            {
                DeviceId = deviceId,
                IsLicenseActivated = true
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
                    await Navigation.PopAsync();
                    await DisplayAlert(AppResources.DearUsers, AppResources.PrivacyDisplayMessage, AppResources.Ok);

                    break;
                default:
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    break;
            }   
        }

        private void OnScrolled(object sender, ScrolledEventArgs e)
        {
            var scrollingSpace = scrollView.ContentSize.Height - scrollView.Height;
            if (scrollingSpace <= e.ScrollY + 50)
                OnButtonAgree.IsEnabled = true;
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Navigation.PopModalAsync(true);
        }
    }
}