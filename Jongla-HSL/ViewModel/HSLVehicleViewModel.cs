using Jongla_HSL.Model;
using System;
using System.Collections.Generic;
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
        private List<HSLVehicle> listOfNewVehicles;
        public async Task LoadVehicleDetails()
        {
            listOfNewVehicles = new List<HSLVehicle>();

            //Client request to get the data from the HSL server 
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 512000;
            string uri = "http://dev.hsl.fi/siriaccess/vm/json?operatorRef=HSL&" + DateTime.Now.Ticks.ToString();
            HttpResponseMessage response = await httpClient.GetAsync(uri);
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
                                listOfNewVehicles.Add(hslVehicle);
                                //VehicleItems.Add(hslVehicle);
                            }
                            updateObservableCollectionVehicles(listOfNewVehicles);
                        });
                    }
                    catch (Exception ex)
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

        private void updateObservableCollectionVehicles(List<HSLVehicle> listOfNewVehicles)
        {
            List<HSLVehicle> updateNewItems = new List<HSLVehicle>();
            List<HSLVehicle> updateOldItems = new List<HSLVehicle>();
            List<HSLVehicle> removeOldItems = new List<HSLVehicle>();
            if (VehicleItems.Count == 0)
            {
                foreach (HSLVehicle item in listOfNewVehicles)
                {
                    VehicleItems.Add(item);
                }
            }
            if (VehicleItems.Count > 0)
            {
                foreach (HSLVehicle item in listOfNewVehicles)
                {
                    var oldlocalList  = VehicleItems.Where(i => i.VehicleRef == item.VehicleRef);
                    if(!VehicleItems.Any(i => i.VehicleRef == item.VehicleRef))
                    {
                        updateNewItems.Add(item);
                    }
                    if (oldlocalList != null)
                    {
                        foreach (HSLVehicle updateItem in oldlocalList)
                        {
                            updateOldItems.Add(updateItem);
                        }
                    }
                }
                foreach(HSLVehicle olditem in VehicleItems)
                {
                    if(!listOfNewVehicles.Any(i => i.VehicleRef == olditem.VehicleRef))
                    {
                        removeOldItems.Add(olditem);
                    }
                }
                if (updateNewItems.Count > 0)
                {
                    foreach(HSLVehicle item in updateNewItems)
                    {
                        VehicleItems.Add(item);
                    }
                }
                if (removeOldItems.Count > 0)
                {
                    for (int i = 0; i < removeOldItems.Count; i++)
                    {
                        for (int j = 0; j < VehicleItems.Count; j++)
                        {
                            if (removeOldItems[i] == VehicleItems[j])
                            {
                                VehicleItems.RemoveAt(j);
                            }
                        }
                    }
                }
                if (updateOldItems.Count > 0)
                {
                    foreach (HSLVehicle item in updateOldItems)
                    {
                        foreach (HSLVehicle olditem in VehicleItems)
                        {
                            if (item.VehicleRef == olditem.VehicleRef)
                            {
                                if (item.LineRef != olditem.LineRef)
                                {
                                    olditem.LineRef = item.LineRef;
                                }
                                if (item.Latitude != olditem.Latitude)
                                {
                                    olditem.Latitude = item.Latitude;
                                }
                                if (item.Longitude != olditem.Longitude)
                                {
                                    olditem.Longitude = item.Longitude;
                                }
                                if (item.Location != olditem.Location)
                                {
                                    olditem.Location = item.Location;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}