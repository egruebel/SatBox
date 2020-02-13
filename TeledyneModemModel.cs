using System;
using System.Text;
using System.Net;

namespace SatBox
{
    class TeledyneModemModel
    {
        public TeledyneModemModel(string ipAddress, string parentSystemName)
        {
            IpAddress = ipAddress;
            SystemName = parentSystemName;
        }

        private string SystemName { get; set; }

        public bool RequestModemStatus()
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Set(HttpRequestHeader.Accept, "text/html");
                    client.Headers.Add(HttpRequestHeader.Accept, "application/xhtml+xml");
                    client.Headers.Add(HttpRequestHeader.Accept, "application/xml;q=0.9,*/*;q=0.8");
                    client.Headers.Add(HttpRequestHeader.Accept, "application/xml");
                    client.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                    client.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                    var userName = "user";
                    var passWord = "paradise";
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + passWord));
                    client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

                    var t = client.DownloadString("http://" + IpAddress + "/maintabs.php?TB=1");

                    //get the variable names from the js
                    var startNames = t.IndexOf("var MCPNames");
                    var endNames = t.IndexOf("];", startNames);
                    ModemResponse = t.Substring(startNames + 15, (endNames - startNames) - 15).Split(',');

                    var startValues = t.IndexOf("var MCPValues");
                    var endValues = t.IndexOf(';', startValues);
                    ModemValues = t.Substring(startValues + 17, (endValues - startValues) - 17).Split(',');
                }
                return true;
            }
            catch (Exception ex)
            {
                Program.LogError(SystemName, "Teledyne Paradise Modem", "Communication with modem failed", ex.Message);
                return false;
            }
        }

        private string[] ModemResponse { get; set; }
        private string[] ModemValues { get; set; }
        public string ModemResponseDump()
        {
            string returnVal = string.Empty;
            for (int i = 0; i < ModemResponse.Length; i++)
            {
                returnVal += ModemResponse[i] + "=" + ModemValues[i];
            }
            return returnVal;
        }
        public string IpAddress { get; set; }

        public string GetModemValue(string index)
        {
            for (int i = 0; i < ModemResponse.Length; i++)
            {
                if (ModemResponse[i] == index)
                {
                    return ModemValues[i].Replace('\'',' ').Trim();
                }
            }
              
            return string.Empty;
        }
    }
}
