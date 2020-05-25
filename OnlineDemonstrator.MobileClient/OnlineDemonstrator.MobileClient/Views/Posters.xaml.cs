using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class Posters : ContentPage
    {
        private readonly INetwork _network;
        private readonly int _demonstrationId;
        private readonly Guid _currentDeviceId;
        private readonly int _postersCountInDemonstration;

        public Posters(int demonstrationId, Guid currentDeviceId, int postersCountInDemonstration)
        {
            _network = App.Container.Resolve<INetwork>();
            InitializeComponent();
            _demonstrationId = demonstrationId;
            _currentDeviceId = currentDeviceId;
            _postersCountInDemonstration = postersCountInDemonstration;
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnListViewItemSelected(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is PosterOut posterOut))
                return;

            PosterList.SelectedItem = null;
           
            await Navigation.PushModalAsync(new Poster(posterOut.DeviceId, posterOut.CreatedDate, _currentDeviceId));
        }

        private async Task LoadPostersByDemonstrationId(int demonstrationId, int postersCountInDemonstration)
        {
            var numberOfParticipants = $"{AppResources.NumberOfParticipants} {postersCountInDemonstration}";
            PostersCountInDemonstrationLabel.Text = numberOfParticipants;
            var fullUrl = $"{Url.GetPostersByDemonstrationId}{demonstrationId}";
            var (baseResult, content) = await _network.LoadDataGetAsync(fullUrl, null);

            switch (baseResult.Result)
            {
                case StatusCode.Ok:
                    try
                    {
                        var posters = JsonConvert.DeserializeObject<List<PosterOut>>(content);
                        if (posters.Any())
                        {
                            var demonstrationDate = posters.First().CreatedDate;
                            var currentDate = DateTime.UtcNow.Date;
                            var expDemonstrationInDays = (demonstrationDate.AddDays(7) - currentDate).Days;
                            var dayOrDaysStr = expDemonstrationInDays == 1 ? $"{AppResources.Day}" : expDemonstrationInDays < 5 ? $"{AppResources.Days}" : $"{AppResources.DaysMoreThenFour}";
                            ExpDateLabel.Text = $"{AppResources.ExpDemonstrationInDays} {expDemonstrationInDays} {dayOrDaysStr}";
                        }
                        PosterList.ItemsSource = posters;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(AppResources.NotificationError);
                    }
                  
                    break;
                default:
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    break;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPostersByDemonstrationId(_demonstrationId, _postersCountInDemonstration);
        }
    }
}