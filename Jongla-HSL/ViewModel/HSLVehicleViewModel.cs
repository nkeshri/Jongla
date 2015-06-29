using Jongla_HSL.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;

namespace Jongla_HSL.ViewModel
{
    public class HSLVehicleViewModel
    {
        public HSLVehicleViewModel()
        {
            this.VehicleItems = new ObservableCollection<HSLVehicle>();
        }
        public ObservableCollection<HSLVehicle> VehicleItems { get; private set; }

        public async Task LoadVehicleDetails()
        {
            //Clear the list of items if it is not empty
            if (VehicleItems != null)
                VehicleItems.Clear();

            //Client request to get the data from the HSL server 
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 512000;
            HttpResponseMessage response = await httpClient.GetAsync(" http://dev.hsl.fi/siriaccess/vm/json?operatorRef=HSL");
            try
            {
                response.EnsureSuccessStatusCode();
                Stream StreamResponse = await response.Content.ReadAsStreamAsync();
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RootObject));
                RootObject returnedData = (RootObject)s.ReadObject(StreamResponse);
                if (returnedData.Siri.ServiceDelivery.VehicleMonitoringDelivery.Count == 1)
                {
                    try
                    {
                        CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            //Get the list of vehicles from the returned data
                            var AllVehicle = from m in returnedData.Siri.ServiceDelivery.VehicleMonitoringDelivery[0].VehicleActivity
                                             select m;
                            foreach (VehicleActivity singleVehicle in AllVehicle)
                            {
                                HSLVehicle hslVehicle = new HSLVehicle();
                                hslVehicle.LineRef = singleVehicle.MonitoredVehicleJourney.LineRef.value;
                                hslVehicle.VehicleRef = singleVehicle.MonitoredVehicleJourney.VehicleRef.value;

                                hslVehicle.Latitude = singleVehicle.MonitoredVehicleJourney.VehicleLocation.Latitude;
                                hslVehicle.Longitude = singleVehicle.MonitoredVehicleJourney.VehicleLocation.Longitude;

                                //Convert latitude and longitude to Geopoint
                                BasicGeoposition queryHint = new BasicGeoposition();
                                queryHint.Latitude = hslVehicle.Latitude;
                                queryHint.Longitude = hslVehicle.Longitude;
                                hslVehicle.Location = new Geopoint(queryHint);

                                //Add items to the observable collection
                                VehicleItems.Add(hslVehicle);
                            }
                        });
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            response.Dispose();
            httpClient.Dispose();
        }
    }
}
