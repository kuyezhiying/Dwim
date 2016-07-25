using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Dwim.Controllers
{
    public class DwimApiController : ApiController
    {
        [HttpGet]
        [Route("api/dwimApi/correctsentence/")]
        [ResponseType(typeof(object))]
        public IHttpActionResult SendRequestToMemory(string request)
        {
            string appId = "dwimbot";
            string appSecret = "c58b7465f87049b5ad1d03d4b4aa207b";
            String dwimUrl = "http://dwimbot.azurewebsites.net/api/messages";
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(dwimUrl);

            webRequest.Headers.Add("Ocp-Apim-Subscription-Key", appSecret);
            var byteArray = Encoding.ASCII.GetBytes($"{appId}:{appSecret}");
            string auth = "Basic=" + Convert.ToBase64String(byteArray);
            webRequest.Headers.Add("Authorization", auth);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";

            string a =
                "{\"type\": \"Message\",  \"id\": \"123\",\"conversationId\": \"123\",\"language\": \"enus\",\"text\": \"" + "dwimoutlook " +
                request + "\",\"from\": {\"id\": \"123\"},\"channelConversationId\": \"123\"}";

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(a);
            webRequest.ContentLength = bytes.Length;
            using (System.IO.Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            System.Net.WebResponse webResponse = webRequest.GetResponse();
            using (Stream stream = webResponse.GetResponseStream())
            {
                StreamReader myStreamReader = new StreamReader(stream, Encoding.UTF8);
                string tmp = myStreamReader.ReadToEnd();
                JObject o = JObject.Parse(tmp);
                string result = o["text"].ToString();
                return Ok(new {
                    text = result
                });
            }
        }

    }
}