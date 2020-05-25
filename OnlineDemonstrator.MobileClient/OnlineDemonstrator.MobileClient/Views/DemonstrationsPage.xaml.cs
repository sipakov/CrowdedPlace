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
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace OnlineDemonstrator.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DemonstrationsPage : ContentPage
    {
        private readonly INetwork _network;

        public DemonstrationsPage()
        {
            InitializeComponent();
            _network = App.Container.Resolve<INetwork>();
        }
        
        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }

        private async void OnListViewItemSelected(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is DemonstrationOut demonstrationOut))
                return;

            ActualDemonstrationList.SelectedItem = null;

            const double latitudeDegrees = 0.007;
            const double longitudeDegrees = 0.007;
            var mapSpan = new MapSpan(new Position(demonstrationOut.Latitude,demonstrationOut.Longitude), latitudeDegrees, longitudeDegrees);
            MessagingCenter.Send<DemonstrationsPage, MapSpan> (this, "MoveTo", mapSpan);

            await Navigation.PopModalAsync(true);
        }
        
        private async Task LoadActualDemonstrations()
        {
            var fullUrl = $"{Url.ActualDemonstrations}";
            var (baseResult, content) = await _network.LoadDataGetAsync(fullUrl,  null);

            switch (baseResult.Result)
            {
                case StatusCode.Ok:
                    var actualDemonstrations = JsonConvert.DeserializeObject<List<DemonstrationOut>>(content);
                    foreach (var actualDemonstration in actualDemonstrations)
                    {
                        var demonstrationDate = actualDemonstration.DemonstrationDate;
                        var currentDate = DateTime.UtcNow.Date;
                        var expDemonstrationInDays = (demonstrationDate.AddDays(7) - currentDate).Days;
                        var dayOrDaysStr = expDemonstrationInDays == 1 ? $"{AppResources.Day}" : expDemonstrationInDays < 5 ? $"{AppResources.Days}" : $"{AppResources.DaysMoreThenFour}";
                        actualDemonstration.ExpDays = $"{AppResources.ExpDemonstrationInDays} {expDemonstrationInDays} {dayOrDaysStr}";
                    }
                    ActualDemonstrationList.ItemsSource = actualDemonstrations;
                    NoDemonstrationsLabel.IsVisible = !actualDemonstrations.Any();
                    break;
                default:
                    await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                    break;
            }
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadActualDemonstrations();
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Navigation.PopModalAsync(true);
        }
    }
}