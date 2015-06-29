using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jongla_HSL.Model
{
    class HSLVehicle : INotifyPropertyChanged
    {
        private string _VehicleRef;
        public string VehicleRef
        {
            get { return _VehicleRef; }
            set
            {
                _VehicleRef = value;
                RaisePropertyChanged("VehicleRef");
            }
        }

        private string _LineRef;
        public string LineRef
        {
            get { return _LineRef; }
            set
            {
                _LineRef = value;
                RaisePropertyChanged("LineRef");
            }
        }

        private double _Latitude;
        public double Latitude
        {
            get { return _Latitude; }
            set
            {
                _Latitude = value;
                RaisePropertyChanged("Latitude");
            }
        }

        private double _Longitude;
        public double Longitude
        {
            get { return _Longitude; }
            set
            {
                _Longitude = value;
                RaisePropertyChanged("Longitude");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class ProducerRef
    {
        public string value { get; set; }
    }

    public class LineRef
    {
        public string value { get; set; }
    }

    public class DirectionRef
    {
        public string value { get; set; }
    }

    public class DataFrameRef
    {
        public string value { get; set; }
    }

    public class FramedVehicleJourneyRef
    {
        public DataFrameRef DataFrameRef { get; set; }
        public string DatedVehicleJourneyRef { get; set; }
    }

    public class OperatorRef
    {
        public string value { get; set; }
    }

    public class VehicleLocation
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

    public class MonitoredCall
    {
        public string StopPointRef { get; set; }
        public int? Order { get; set; }
    }

    public class VehicleRef
    {
        public string value { get; set; }
    }

    public class MonitoredVehicleJourney
    {
        public LineRef LineRef { get; set; }
        public DirectionRef DirectionRef { get; set; }
        public FramedVehicleJourneyRef FramedVehicleJourneyRef { get; set; }
        public OperatorRef OperatorRef { get; set; }
        public bool Monitored { get; set; }
        public VehicleLocation VehicleLocation { get; set; }
        public int Delay { get; set; }
        public MonitoredCall MonitoredCall { get; set; }
        public VehicleRef VehicleRef { get; set; }
        public int? Bearing { get; set; }
    }

    public class VehicleActivity
    {
        public object ValidUntilTime { get; set; }
        public object RecordedAtTime { get; set; }
        public MonitoredVehicleJourney MonitoredVehicleJourney { get; set; }
    }

    public class VehicleMonitoringDelivery
    {
        public string version { get; set; }
        public long ResponseTimestamp { get; set; }
        public bool Status { get; set; }
        public List<VehicleActivity> VehicleActivity { get; set; }
    }

    public class ServiceDelivery
    {
        public long ResponseTimestamp { get; set; }
        public ProducerRef ProducerRef { get; set; }
        public bool Status { get; set; }
        public bool MoreData { get; set; }
        public List<VehicleMonitoringDelivery> VehicleMonitoringDelivery { get; set; }
    }

    public class Siri
    {
        public string version { get; set; }
        public ServiceDelivery ServiceDelivery { get; set; }
    }

    public class RootObject
    {
        public Siri Siri { get; set; }
    }
}
