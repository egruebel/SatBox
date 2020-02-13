using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace SatBox
{
    class IDirectModemModel
    {
        public IDirectModemModel(string ipAddress, string parentSystemName)
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
                    var userName = "admin";
                    var passWord = "P@55w0rd!";
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + passWord));
                    client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                    var t = client.DownloadString("https://" + IpAddress + @"/api.o.j/network%3Brxes%3Btx%3Bcpu%3Btemperature%3Blocation.remote");

                    //get the variable names from the js
                    var o = JsonConvert.DeserializeObject<Dictionary<string, object>>(t);
                    ModemValues = o;
                }
                return true;
            }
            catch (Exception ex)
            {
                Program.LogError(SystemName, "iDirectX7 Modem", "Communication with modem failed", ex.Message);
                return false;
            }
        }

        private Dictionary<string,object> ModemValues  { get; set; }

        public string ModemResponseDump()
        {
            string returnVal = string.Empty;
            foreach (var o in ModemValues)
            {
                returnVal += o.Key + ": " + o.Value.ToString();
            }
            return returnVal;
        }
        public string IpAddress { get; set; }

        public string GetModemValue(string valName)
        {
            try
            {
                return ModemValues[valName].ToString();
            }
            catch
            {
                return string.Empty;
            }
            
        }
    }
}
