using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OnlineDemonstrator.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void OnLicenseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LicensePage(true, Guid.Empty));
        }

        private async void OnPrivacyPolicyButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PrivacyPolicyPage());
        }
    }
}