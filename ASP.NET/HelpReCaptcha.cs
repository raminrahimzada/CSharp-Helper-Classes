#define  RECAPTCHA
using System.Collections.Generic;
namespace raminrahimzada
{
    public class HelpReCaptcha
    {
        /// <summary>
        /// Your reCAPTCHA Secret Code Here
        /// </summary>
        private static string SecretCode
        {
            get { return ""; }
        }
        /// <summary>
        /// Your reCAPTCHA Public Code Here
        /// </summary>
        public static string PublicCode
        {
            get { return ""; }
        }
        public bool Success { get; set; }
        public List<string> ErrorCodes { get; set; }
        public static bool Validate(string encodedResponse)
        {
#if RECAPTCHA
            if (string.IsNullOrEmpty(encodedResponse)) return false;
            var client = new System.Net.WebClient();
            if (string.IsNullOrEmpty(SecretCode)) return false;
            var googleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", SecretCode, encodedResponse));
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var reCaptcha = serializer.Deserialize<HelpReCaptcha>(googleReply);
            return reCaptcha.Success;
#else
            return true;
#endif
        }
        public static string RenderReCaptcha(double scale = 1.0)
        {
#if RECAPTCHA
            return string
                .Format(
                    "<div style=\"transform:scale({0});-webkit-transform:scale({0});transform-origin:0 0;" +
                    "-webkit-transform-origin:0 0; data-theme=\"light\" class=\"g-recaptcha\" data-sitekey=\"{1}\"></div>",
                    scale, PublicCode);
#else
            return "";
#endif
        }
    }
    public static class CaptchaExtensions
    {
        public static bool IsReCaptchaValid(this System.Web.UI.Page page)
        {
#if RECAPTCHA
            var encodedResponse = page.Request.Form["g-Recaptcha-Response"];
            var isCaptchaValid = HelpReCaptcha.Validate(encodedResponse);
            return isCaptchaValid;
#else
            return true;
#endif
        }
    }
}
