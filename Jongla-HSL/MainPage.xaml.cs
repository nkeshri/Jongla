using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Jongla_HSL
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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
            await App.VehicleViewModel.LoadVehicleDetails();
            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = null;
            try
            {
                geoposition = await geolocator.GetGeopositionAsync();
            }
            catch (Exception ex)
            {
                // Handle errors like unauthorized access to location
                // services or no Internet access.
            }
            hslMapControl.Center = geoposition.Coordinate.Point;
            hslMapControl.ZoomLevel = 12;
            for (int i = 0; i < App.VehicleViewModel.VehicleItems.Count; i++)
            {
                MapIcon mapIcon = new MapIcon();
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(
                  new Uri("ms-appx:///Assets/pin_map_down.png"));
                mapIcon.NormalizedAnchorPoint = new Point(0.25, 0.9);
                BasicGeoposition queryHint = new BasicGeoposition();
                queryHint.Latitude = App.VehicleViewModel.VehicleItems[i].Latitude;
                queryHint.Longitude = App.VehicleViewModel.VehicleItems[i].Longitude;
                Geopoint hintPoint = new Geopoint(queryHint);
                mapIcon.Location = hintPoint;
                mapIcon.Title = App.VehicleViewModel.VehicleItems[i].LineRef;
                hslMapControl.MapElements.Add(mapIcon);
            }
        }
    }
}
