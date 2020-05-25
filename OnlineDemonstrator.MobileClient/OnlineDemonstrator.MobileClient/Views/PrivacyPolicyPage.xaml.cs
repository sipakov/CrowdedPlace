using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OnlineDemonstrator.MobileClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrivacyPolicyPage : ContentPage
    {
        public PrivacyPolicyPage()
        {
            InitializeComponent();
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