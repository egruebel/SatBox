using System;
using System.Net;


namespace SatBox
{
    partial class SeatelMxpModel
    {
        public SeatelMxpModel(string ipAddress, string parentSystemName)
        {
            this.IpAddress = ipAddress;
            this.SystemName = parentSystemName;
        }

        private string SystemName { get; set; }

        public bool RequestAntennaStatus()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var requestString = "";
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/text");
                    var response = client.UploadString(new Uri("http://192.168.2.27/cgi-bin/getSysStatus"), "POST", requestString);
                    MxpResponse = response.Split(';');

                }
                return true;
            }
            catch(Exception ex)
            {
                Program.LogError(SystemName,"Sea Tel MXP", "Communication with MXP failed", ex.Message);
                return false;
            }
        }

        

        private string[] MxpResponse { get; set; }
        public string MxpResponseDump()
        {
            string returnVal = string.Empty;
            foreach (var s in MxpResponse)
            {
                returnVal += s + ";";
            }
            return returnVal;
        }
        public string IpAddress { get; set; }

        public string GetMxpValue(string title)
        {
            foreach (var s in MxpResponse)
            {
                if (s.StartsWith(title))
                {
                    var eq = s.IndexOf('=');
                    return s.Substring(eq + 1, s.Length - (eq + 1));
                }
            }
            return string.Empty;
        }

        
    }
}
