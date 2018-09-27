using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AndroidBeacon.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AndroidBeacon
{
    using Plugin.Permissions;

    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Location);
            MainPage = new MainPage();
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
    }
}
