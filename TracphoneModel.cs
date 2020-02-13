using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Net;

namespace SatBox
{
    partial class TracphoneModel: ISatSystem
    {

        public TracphoneModel(string ipAddress)
        {
            IpAddress = ipAddress;
        }

        public bool QuerySystemStatus()
        {
            try
            {
                using (var client = new WebClient())
                {
                    const string requestString = "<ipacu_request><message name=\"antenna_status\"></message></ipacu_request>";
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/xml");
                    var response = client.UploadString(new Uri("http://" + IpAddress + "/antservice.php"), "POST", requestString);
                    XmlResponse = XDocument.Parse(response);
                }
                if (ValidateResponse())
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Program.LogError(GetType().Name, "KVH CommBox", "Communication with CommBox failed", ex.Message);
                return false;
            }

        }

        public bool ValidateResponse()
        {
            string failedParam = string.Empty;
            try
            {
                failedParam = "EbNo";
                var a = EbNo;
                failedParam = "Lat";
                var b = Lat;
                failedParam = "Lon";
                var c = Lon;
                failedParam = "Hdg";
                var d = Hdg;
                failedParam = "Rel";
                var e = Rel;
                failedParam = "SatLon";
                var f = SatLon;
                failedParam = "Azimuth";
                var g = Azimuth;
                failedParam = "Elevation";
                var h = Elevation;
                failedParam = "CrossPol";
                var i = CrossPol;
                failedParam = "SigStrength";
                var j = SigStrength;
                failedParam = "SatName";
                var k = SatName;
                failedParam = "ConStatus";
                var l = ConStatus ;
                return true;
            }
            catch
            {
                Program.LogError(GetType().Name, "KVH CommBox", "Error while parsing response from CommBox: " + failedParam, "Received response: " + XmlResponse.ToString());
                return false;
            }
        }

        private XDocument XmlResponse { get; set; }
        public string IpAddress { get; set; }


        public double EbNo
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//ebno").Value);
            }
        }

        public double Lat
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//gps/lat").Value);
            }
        }

        public double Lon
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//gps/lon").Value);
            }
        }

        public double Hdg
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//brst/hdg").Value);
            }
        }

        public double Rel
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//brst/az_bow").Value);
            }
        }

        public double SatLon
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//satellite/lon").Value);
            }
        }

        public double Azimuth
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//brst/az").Value);
            }
        }

        public double Elevation
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//brst/el").Value);
            }
        }

        public double CrossPol
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//brst/tilt").Value);
            }
        }

        public string SatName
        {
            get
            {
                var name = XmlResponse.XPathSelectElement("//satellite/name").Value;
                var beam = XmlResponse.XPathSelectElement("//satellite/beam").Value;
                if(string.IsNullOrEmpty(name))
                    return beam;
                return name + " " + beam;
            }
        }

        public double SigStrength
        {
            get
            {
                return double.Parse(XmlResponse.XPathSelectElement("//rf/rssi").Value);
            }
        }

        public string ConStatus
        {
            get
            {
                return XmlResponse.XPathSelectElement("//modem/state").Value;
            }
        }

        
    }
}
