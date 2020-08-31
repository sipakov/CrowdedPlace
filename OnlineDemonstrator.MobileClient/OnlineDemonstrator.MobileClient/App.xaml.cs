using System;
using Autofac;
using OnlineDemonstrator.Libraries.Network.Implementations;
using OnlineDemonstrator.Libraries.Network.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace OnlineDemonstrator.MobileClient
{
    public partial class App : Application
    {
        private readonly Guid _deviceId;
        public static IContainer Container{ get; set; }

        static App()
        {
            InitializeIocContainer();  
        }
        public App(Guid deviceId)
        {
            InitializeComponent();
            _deviceId = deviceId;
            if (Device.RuntimePlatform == Device.Android)
            { 
                _deviceId = GetCurrentDeviceId(_deviceId);
            }
            MainPage = new MainPage(_deviceId);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        
        private static void InitializeIocContainer() 
        {    
            var builder = new ContainerBuilder();    
            builder.RegisterType<Network>().As<INetwork>();
            Container = builder.Build();

            // Don't stick view models in the container unless you need them globally, which is almost never true !!!

            // Don't Resolve the service variable until you need it !!!! }
        }

        private static Guid GetCurrentDeviceId(Guid deviceId)
        {
            Guid targetDeviceId;
            
                var isSuccess =
                    Xamarin.Forms.Application.Current.Properties.TryGetValue("deviceIdOnlineDemonstrator",
                        out var deviceIdObj);
                if (isSuccess)
                {
                    var isValidGuid = Guid.TryParse(deviceIdObj.ToString(), out var currentDeviceId);
                    if (isValidGuid && currentDeviceId != Guid.Empty)
                    {
                        targetDeviceId = currentDeviceId;
                    }
                    else
                    {
                        targetDeviceId = Guid.NewGuid();
                        Xamarin.Forms.Application.Current.Properties.Remove("deviceIdOnlineDemonstrator");
                        Xamarin.Forms.Application.Current.Properties.Add("deviceIdOnlineDemonstrator", targetDeviceId);
                    }
                }
                else
                {
                    targetDeviceId = Guid.NewGuid();
                    Xamarin.Forms.Application.Current.Properties.Add("deviceIdOnlineDemonstrator", targetDeviceId);
                }     
                
            return targetDeviceId;
        }
    }
}