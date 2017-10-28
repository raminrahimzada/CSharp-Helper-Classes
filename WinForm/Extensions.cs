public static class Extensions
{
    void GetBatteryLevel()
      {

          //Use this code in next build
          //System.Windows.Forms.PowerStatus status = System.Windows.Forms.SystemInformation.PowerStatus;

          double batteryLevel;
          ManagementObjectSearcher mos = new ManagementObjectSearcher("select EstimatedChargeRemaining from Win32_Battery");
          ManagementObjectCollection collection = mos.Get();
          MessageBox.Show(collection.ToString());
          foreach (ManagementObject mo in collection)
          {
              UInt16 remainingPercent = (UInt16)mo["EstimatedChargeRemaining"];
              batteryLevel = (double)remainingPercent;
              batteryLevel = batteryLevel / 100;
              batteryLevel = 1.0 - batteryLevel;
              //use from this
          }
      }
        public string GetPublicIP()
        {
            String direction = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                direction = stream.ReadToEnd();
            }
            MessageBox.Show(direction);
            //Search for the ip in the html
            int first = direction.IndexOf("Address: ") + 9;
            int last = direction.LastIndexOf("</body>");
            direction = direction.Substring(first, last - first);

            return direction;
        }

        public string TranslateText(string input, string languagePair)
        {
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            string result = webClient.DownloadString(url);
            result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
            result = result.Substring(result.IndexOf(">") + 1);
            result = result.Substring(0, result.IndexOf("</span>"));
            return result.Trim();
        }
}
