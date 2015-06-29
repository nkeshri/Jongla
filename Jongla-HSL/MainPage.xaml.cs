using System;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// This application shows the list of vehicles returned 
// by Helsinki public transport link: http://dev.hsl.fi/siriaccess/vm/json?operatorRef=HSL
// It also shows the location of the vehicles in the map using the mapicon

namespace Jongla_HSL
{
    public sealed partial class MainPage : Page
    {
        //Timer used to refresh the HSL data every 5 seconds
        DispatcherTimer RefreshTimer;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            DataContext = App.VehicleViewModel;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Wait for the data to be loaded from the HSL webservice
            try
            {
                await App.VehicleViewModel.LoadVehicleDetails();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //Get the geolocation of the phone to display in the map
            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = null;
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            hslMapControl.Center = geoposition.Coordinate.Point;
            hslMapControl.ZoomLevel = 15;
            InitiateRefreshTimer();
        }

        private void InitiateRefreshTimer()
        {
            try
            {
                RefreshTimer = new DispatcherTimer();
                RefreshTimer.Interval = TimeSpan.FromSeconds(5);
                RefreshTimer.Tick += RefreshTimer_Tick;
                RefreshTimer.Start();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void RefreshTimer_Tick(object sender, object e)
        {
            try
            {
                await App.VehicleViewModel.LoadVehicleDetails();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }
}
