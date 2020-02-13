using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SatBox
{
    partial class FBB500Model: ISatSystem
    {
        public FBB500Model(string ipAddress)
        {
            IpAddress = ipAddress;
        }
        public bool QuerySystemStatus()
        {
            try
            {
                using (var client = new WebClient())
                {
                    //we need to load the maintenance page first
                    //because sometimes the webservices get 'stuck' and won't be accessible
                    //without loading the entire page
                    var url = "http://" + IpAddress + "/index.lua?pageID=Maintenance&langID=english";
                    client.DownloadString(url);
                    url = "http://" + IpAddress + "/index.lua/mt_gps_status?rs=rs";
                    GpsResponse = client.DownloadString(url).Split('$');
                    url = "http://" + IpAddress + "/index.lua/mt_sat_status?rs=rs";
                    SatResponse = client.DownloadString(url).Split('$');
                    url = "http://" + IpAddress + "/index.lua/get_cn0_normalized?rs=rs";
                    SignalStrength = client.DownloadString(url).Replace("+:","");
                    url = "http://" + IpAddress + "/index.lua/mt_iai2_status?rs=rs";
                    IaiResponse = client.DownloadString(url).Split('$');
                }
                if (ValidateResponse())
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                Program.LogError(GetType().Name, "FBB 500", "Communication with FBB failed", ex.Message);
                return false;
            }

        }

        private string[] GpsResponse { get; set; }
        private string[] SatResponse { get; set; }
        private string[] IaiResponse { get; set; }
        private string SignalStrength { get; set; }
        public string IpAddress { get; set; }

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
                Program.LogError(GetType().Name, "FBB 500", "Error while parsing response from FBB500: " + failedParam, "Received response: " + FBB500ResponseDump());
                return false;
            }
        }

        private string FBB500ResponseDump()
        {
            /*
             private string[] GpsResponse { get; set; }
             private string[] SatResponse { get; set; }
             private string[] IaiResponse { get; set; }
             private string SignalStrength { get; set; }
             */
            string returnVal = "GPS: ";
            foreach (var s in GpsResponse)
            {
                returnVal += s + "$";
            }
            returnVal += "SAT: ";
            foreach (var s in SatResponse)
            {
                returnVal += s + "$";
            }
            returnVal += "IAI: ";
            foreach (var s in IaiResponse)
            {
                returnVal += s + "$";
            }
            returnVal += "SIG: ";
            returnVal += SignalStrength;

            return returnVal;
        }

        private static double ToSignedDecimal(string value, string unit)
        {

            var ddmmss = (Convert.ToDouble(value) / 100);

            var degrees = (int)ddmmss;

            var minutesseconds = ((ddmmss - degrees) * 100) / 60.0;

            //south and west are negative
            if (unit == "S" || unit == "W")
                return Math.Round((degrees + minutesseconds) * -1,4);

            return Math.Round(degrees + minutesseconds,4);
        }

        private static double GetAzimuth(double latitude, double longitude, double satLongitude)
        {
            var lon_delta = (longitude - satLongitude);
            if (lon_delta < -180) lon_delta = 360 + lon_delta;
            if (lon_delta > 180) lon_delta = 360 - lon_delta;
            
            var radian_lon = lon_delta / 57.29578;

            var comp_azumith = 180 + (57.29578 * Math.Atan(Math.Tan(radian_lon) / Math.Sin(latitude / 57.29578)));
            if (latitude < 0) 
                comp_azumith = comp_azumith - 180.0;
            if (comp_azumith < 0) 
                comp_azumith = comp_azumith + 360.0;
            if (lon_delta < -90) 
                comp_azumith = comp_azumith - 180.0;
            if (lon_delta > 90) 
                comp_azumith = comp_azumith + 180.0;
            
            comp_azumith = (Math.Round(comp_azumith,2));
            return comp_azumith;


        }

        /*
         function azimuth(satLongitude,latitude,longitude) {
	if (satLongitude==longitude && latitude==0) return 0;
	lon_delta=(longitude-satLongitude);
	if (lon_delta<-180) lon_delta=360+lon_delta;
	if (lon_delta>180) lon_delta=360-lon_delta;
	radian_lon=lon_delta/57.29578;
	comp_azumith=180+(57.29578*Math.atan(Math.tan(radian_lon)/Math.sin(latitude/57.29578)));
	if (latitude<0) comp_azumith=comp_azumith-180.0;
	if (comp_azumith<0) comp_azumith=comp_azumith+360.0;
	if (lon_delta<-90) comp_azumith=comp_azumith-180.0;
	if (lon_delta>90) comp_azumith=comp_azumith+180.0;
	comp_azumith=(Math.round(comp_azumith));
	return comp_azumith;
         * 
         * function elevation(satLongitude,latitude,longitude) {
	radian_lon=(longitude-satLongitude)/57.29578;
	radian_lat=latitude/57.29578;
	d=1+35786/6378.16;
	a=d*Math.cos(radian_lat)*Math.cos(radian_lon)-1;
	b=d*Math.sqrt(1-Math.cos(radian_lat)*Math.cos(radian_lat)*Math.cos(radian_lon)*Math.cos(radian_lon));
	interim_elev=57.29578*Math.atan(a/b);
	if (interim_elev<30)
		comp_elevation=(interim_elev+Math.sqrt(interim_elev*interim_elev+4.132))/2
			else
				comp_elevation=interim_elev;
	comp_elevation=(Math.round(comp_elevation));
//	if (comp_elevation<5) comp_elevation="< 5";
	return comp_elevation;
}
}
         */

        public double EbNo
        {
            get
            {
                return double.Parse(IaiResponse[4]);
            }
        }

        public double Lat
        {
            get
            {
                //HTML formatted lat and long....uggh
                var raw = GpsResponse[1];
                var br = raw.IndexOf("<br>");
                raw = raw.Substring(0, br);
                var degIndex = raw.IndexOf("&deg;");
                var aposIndex = raw.IndexOf("'");
                var spaceIndex = raw.IndexOf(" ");
                var dd = raw.Substring(0, degIndex);
                var mm = raw.Substring(degIndex + 5, aposIndex - (degIndex + 5));
                var ss = raw.Substring(aposIndex + 1, spaceIndex - (aposIndex + 1));
                var unit = raw[raw.Length - 1].ToString();
                ss = ss.Replace("\"", "");
                ss = ss.Replace(",", ".");
                double rtn = double.Parse(ss) / 6;
                rtn = (rtn / 10) + double.Parse(mm);
                string navsec = dd + rtn.ToString("00.000");
                var p = ToSignedDecimal(navsec, unit);
                return p;
            }
        }

        public double Lon
        {
            get
            {
                //HTML formatted lat and long....uggh
                var raw = GpsResponse[1];
                var br = raw.IndexOf("<br>");
                raw = raw.Substring(br + 4, raw.Length - (br + 4));
                var degIndex = raw.IndexOf("&deg;");
                var aposIndex = raw.IndexOf("'");
                var spaceIndex = raw.IndexOf(" ");
                var dd = raw.Substring(0, degIndex);
                var mm = raw.Substring(degIndex + 5, aposIndex - (degIndex + 5));
                var ss = raw.Substring(aposIndex + 1, spaceIndex - (aposIndex + 1));
                var unit = raw[raw.Length - 1].ToString();
                ss = ss.Replace("\"", "");
                ss = ss.Replace(",", ".");
                double rtn = double.Parse(ss) / 6;
                rtn = (rtn / 10) + double.Parse(mm);
                string navsec = dd + rtn.ToString("00.000");
                var p = ToSignedDecimal(navsec, unit);
                return p;
            }
        }

        public double Hdg
        {
            get
            {
                return double.Parse(GpsResponse[4]);
            }
        }

        public double Rel 
        {
            get
            {
                try
                {
                    return 0;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public double SatLon
        {
            get
            {
                var raw = SatResponse[1];
                var spaceIndex = raw.IndexOf(' ');
                var atIndex = raw.IndexOf('&');
                var sl = raw.Substring(spaceIndex + 1, atIndex - (spaceIndex + 1));
                var d = double.Parse(sl);
                if(Lon < 0)
                    d = d * -1;

                return d;
            }
        }

        public double Azimuth
        {
            get
            {
                try
                {
                    var p = GetAzimuth(Lat, Lon, SatLon);
                    return p;
                }
                catch
                {
                    return 0;
                }
                
            }
        }

        public double Elevation
        {
            get
            {
                return double.Parse(SatResponse[2]);
            }
        }

        public double CrossPol
        {
            get
            {
                return 0;
            }
        }

        public string SatName
        {
            get
            {
                return SatResponse[0].Replace("+:","");
            }
        }

        public double SigStrength
        {
            get
            {
                return double.Parse(SignalStrength);
            }
        }

        public string ConStatus
        {
            get
            {
                return IaiResponse[0].Replace("+:", "") + '-' + IaiResponse[1];
            }
        }
    }

    
}
