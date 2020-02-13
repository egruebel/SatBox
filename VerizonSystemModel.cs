using System;

namespace SatBox
{
    partial class VerizonSystemModel: ISatSystem
    {
        private SeatelMxpModel Mxp { get; set; }
        private IDirectModemModel Modem {get;set;}

        public VerizonSystemModel(string mxpIp, string modemIp)
        {
            Mxp = new SeatelMxpModel(mxpIp,GetType().Name);
            Modem = new IDirectModemModel(modemIp, GetType().Name);
        }

        public bool QuerySystemStatus()
        {
            try
            {
                if (Mxp.RequestAntennaStatus() && Modem.RequestModemStatus())
                {
                    if (ValidateResponse())
                    {
                        return true;
                    }
                    return false;
                }
                    
            }
            catch
            {
                return false;
            }
            return false;
        }

        private bool ValidateMxpResponse()
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
                Program.LogError(GetType().Name, "Seatel MXP", "Error while parsing response from MXP: " + failedParam, "Received response: " + Mxp.MxpResponseDump());
                return false;
            }
        }

        public bool ValidateResponse()
        {
            if (ValidateMxpResponse() && ValidateModemResponse())
            {
                return true;
            }
            return false;
        }

        private bool ValidateModemResponse()
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
                return true;
            }
            catch
            {
                Program.LogError(GetType().Name, "HiSeasnet Modem", "Error while parsing response from modem: " + failedParam, "Received response: " + Mxp.MxpResponseDump());
                return false;
            }
        }

        public String IpAddress
        {
            get
            {
                return Mxp.IpAddress;
            }
        }

        
        public double EbNo
        {
            get { return double.Parse(Modem.GetModemValue("snr")); }
        }
        
        public double Lat
        {
            get
            {
                var lat = Mxp.GetMxpValue("lat");
                if (lat.EndsWith("N"))
                {
                    lat = lat.Substring(0, lat.Length - 2);
                    return double.Parse(lat);
                }
                else
                {
                    //south
                    lat = lat.Substring(0, lat.Length - 2);
                    return double.Parse(lat) * -1;
                }
            }
        }

        public double Lon
        {
            get
            {
                var lon = Mxp.GetMxpValue("lon");
                if (lon.EndsWith("E"))
                {
                    lon = lon.Substring(0, lon.Length - 2);
                    return double.Parse(lon);
                }
                else
                {
                    lon = lon.Substring(0, lon.Length - 2);
                    return double.Parse(lon) * -1;
                }
            }
        }

        public double Hdg
        {
            get
            {
                var val = Mxp.GetMxpValue("hdg");
                return double.Parse(val);
            }
        }
        
        public double Rel
        {
            get
            {
                return double.Parse(Mxp.GetMxpValue("rel"));
            }
        }


        public double SatLon
        {
            get
            {
                var ew = Mxp.GetMxpValue("ssp");
                var slon = Mxp.GetMxpValue("slon");
                if (ew == "W")
                {
                    return double.Parse(slon) * -1;
                }
                return double.Parse(slon);
            }
        }

        public double Azimuth
        {
            get
            {
                var val = Mxp.GetMxpValue("az");
                double d;
                double.TryParse(val, out d);
                return d;
            }
        }

        public double Elevation
        {
            get
            {
                return double.Parse(Mxp.GetMxpValue("el"));
            }
        }

        public double CrossPol
        {
            get
            {
                return double.Parse(Mxp.GetMxpValue("cl"));
            }
        }

        

        public string SatName
        {
            get
            {
                return Mxp.GetMxpValue("snm");
            }
        }

        public double SigStrength
        {
            get
            {
                return double.Parse(Mxp.GetMxpValue("agc"));
            }
        }

        public string ConStatus
        {
            get { return string.Empty; }
        }
    }
}
