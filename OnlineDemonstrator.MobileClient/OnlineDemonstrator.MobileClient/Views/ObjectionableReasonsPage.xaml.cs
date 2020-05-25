using System;
using System.Collections.Generic;
using Autofac;
using Newtonsoft.Json;
using OnlineDemonstrator.Libraries.Domain;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Enums;
using OnlineDemonstrator.Libraries.Domain.Url;
using OnlineDemonstrator.Libraries.Network.Interfaces;
using OnlineDemonstrator.MobileClient.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ObjectionableReason = OnlineDemonstrator.Libraries.Domain.Models.ObjectionableReason;

namespace OnlineDemonstrator.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ObjectionableReasonsPage : ContentPage
    {
        private readonly INetwork _network;

        private readonly Guid _deviceId;
        private readonly Guid _objectionableDeviceId;
        private readonly DateTime _objectionablePosterCreatedDate;

        public ObjectionableReasonsPage(Guid deviceId, Guid objectionableDeviceId,
            DateTime objectionablePosterCreatedDate)
        {
            _deviceId = deviceId;
            _objectionableDeviceId = objectionableDeviceId;
            _objectionablePosterCreatedDate = objectionablePosterCreatedDate;
            InitializeComponent();
            _network = App.Container.Resolve<INetwork>();
            FillObjectionableReasonList();
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }

        private async void OnListViewItemSelected(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is ObjectionableReason selectedObjectionableReason))
                return;

            ObjectionableReasonList.SelectedItem = null;
            var objectionableReasonId = selectedObjectionableReason.Id;
            var result = await DisplayAlert(AppResources.ConfirmAction, AppResources.AreYouSure,
                AppResources.ConfirmActionYes, AppResources.ConfirmActionNo);

            if (result)
            {
                var objectionableContent = new ObjectionableContent
                {
                    DeviceId = _deviceId,
                    ObjectionableReasonId = objectionableReasonId,
                    ObjectionableDeviceId = _objectionableDeviceId,
                    ObjectionablePosterCreatedDate = _objectionablePosterCreatedDate,
                    Comment = EditorCommentReason.Text
                };

                var serializableObj = JsonConvert.SerializeObject(objectionableContent);
                var fullUrl = Url.AddObjectionableReason;
                var (baseResult, content) = await _network.LoadDataPostAsync(fullUrl, serializableObj, null);

                switch (baseResult.Result)
                {
                    case StatusCode.Ok:
                        await DisplayAlert(AppResources.Notification, AppResources.YourReportHasBeenSentSuccessfully,
                            AppResources.Ok);
                        await Navigation.PopModalAsync(true);
                        break;
                    default:
                        await DisplayAlert(AppResources.Notification, baseResult.Message, AppResources.Ok);
                        await Navigation.PopModalAsync(true);
                        break;
                }
            }
        }

        private void FillObjectionableReasonList()
        {
            var list = new List<ObjectionableReason>
            {
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.NudityOrSexualActivity,
                    Title = AppResources.NudityOrSexualActivity
                },
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.HateSpeechOrSymbols,
                    Title = AppResources.HateSpeechOrSymbols
                },
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.ViolenceOrDangerousOrganizations,
                    Title = AppResources.ViolenceOrDangerousOrganizations
                },
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.SaleOfIllegalOrRegularGoods,
                    Title = AppResources.SaleOfIllegalOrRegularGoods
                },
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.BullyingOrHarassment,
                    Title = AppResources.BullyingOrHarassment
                },
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.IntellectualPropertyViolation,
                    Title = AppResources.IntellectualPropertyViolation
                },
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.SuicideOrSelfInjury,
                    Title = AppResources.SuicideOrSelfInjury
                },
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.ScamOrFraud,
                    Title = AppResources.ScamOrFraud
                },
                new ObjectionableReason
                {
                    Id = (int) ObjectionableReasons.FalseInformation,
                    Title = AppResources.FalseInformation
                }
            };
            ObjectionableReasonList.ItemsSource = list;
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Navigation.PopModalAsync(true);
        }
    }
}