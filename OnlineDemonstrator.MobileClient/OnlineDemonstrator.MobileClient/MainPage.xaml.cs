using System;
using OnlineDemonstrator.MobileClient.Localization;
using OnlineDemonstrator.MobileClient.Views;
using Xamarin.Forms;

namespace OnlineDemonstrator.MobileClient
{
    public partial class MainPage : TabbedPage
    {
        public MainPage(Guid deviceId)
        {
            var tab1Page = new MapPage(deviceId) { Title = AppResources.MapTab, IconImageSource = "map.png"};
            var tab2Page = new SettingsPage() { Title = AppResources.SettingsTab, IconImageSource = "settings.png"};

            Children.Add(tab1Page);
            Children.Add(tab2Page);
            InitializeComponent();
        }
    }
}