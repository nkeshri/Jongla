using Jongla_HSL.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Jongla_HSL.ViewModel
{
    public class HSLVehicleViewModel
    {
        public HSLVehicleViewModel()
        {
            this.VehicleItems = new ObservableCollection<HSLVehicle>();
        }
        public ObservableCollection<HSLVehicle> VehicleItems { get; private set; }

        public async void LoadVehicleDetails()
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(" http://dev.hsl.fi/siriaccess/vm/json?operatorRef=HSL");
            try
            {
                response.EnsureSuccessStatusCode();
                Stream StreamResponse = await response.Content.ReadAsStreamAsync();
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RootObject));
                RootObject returnedData = (RootObject)s.ReadObject(StreamResponse);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
