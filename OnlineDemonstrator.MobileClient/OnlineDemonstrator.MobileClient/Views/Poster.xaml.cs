using System;
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
    public partial class Poster : ContentPage
    {
        private readonly INetwork _network;

        private readonly Guid _deviceId;
        private readonly DateTime _createdDate;
        private readonly Guid _currentDeviceId;

        public Poster(Guid deviceId, DateTime createdDate, Guid currentDeviceId)
        {
            _network = App.Container.Resolve<INetwork>();
            InitializeComponent();
            _deviceId = deviceId;
            _createdDate = createdDate;
            _currentDeviceId = currentDeviceId;
            OnReportButton.IsVisible = deviceId != currentDeviceId;
        }
        
        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        
        private async void OnReportClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ObjectionableReasonsPage(_currentDeviceId,_deviceId, _createdDate));
        }
        private async Task LoadPosterById(Guid deviceId, DateTime createdDate)
        {
            var fullUrl = $"{Url.GetPosterById}";
            var targetPoster = new PosterOut
            {
                DeviceId = deviceId,
                CreatedDate = createdDate
            };
            var serializedObj = JsonConvert.SerializeObject(targetPoster);
            var (baseResult, content) = await _network.LoadDataPostAsync(fullUrl, serializedObj, null);

            switch (baseResult.Result)
            {
                case StatusCode.Ok:
                    var poster = JsonConvert.DeserializeObject<PosterOut>(content);
                    NameLabel.Text = poster.Name;
                    TitleLabel.Text = poster.Title;
                    MessageLabel.Text = poster.Message;
                    break;
                default:
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    await Navigation.PopModalAsync(true);
                    break;
            }
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPosterById(_deviceId, _createdDate);
        }
    }
}