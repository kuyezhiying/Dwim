using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
                return Ok(new
                {
                    text = result
                });
            }
        }

        [HttpGet]
        [Route("api/dwimApi/getbingentities/")]
        [ResponseType(typeof(object))]
        public IHttpActionResult GetBingEntities(string query)
        {
            string urlRequest = @"http://co3.ah-debug.binginternal.com:84/answerstla.aspx?q={0}[Workflow:%22QueryProbe.bingfirstpageresults%22|VariantConstraint:%22source:probetools%26mkt:en-US%26Workflow:QueryProbe.bingfirstpageresults%26TCMFTARGET:TC2%22|userAugmentation:%22[contact:qpbel+source:probetools][dbg:DRMatchInfo][wa:expandqueryaliases][dbg:TLARerankInfo]%22|augments:%22[WebCommon%20FcsAdultSetting=\%222\%22%20FcsResultBase=\%220\%22%20FcsResultCount=\%2210\%22%20IndexName=\%22web-prodexp\%22%20RequestCommandType=\%22FcsQueryRequestCommand\%22]%20[FcsCaptionOptions%20CaptionExtendedDebugMask=\%221\%22]%20[FcsFcsOptions%20FcsDebug=\%221\%22]%20[TLADebugOptions%20TLADebugAll=\%221\%22]%20[XapQuServiceAnswer%20location=\%22\%22]%20[APlusAnswer%20CrankerDebugEnabled=\%221\%22%20Rstar=\%221\%22%20CPSHistorySourceScenarioName=\%22sessionmemoryrequest\%22%20SquareSignalSourceScenarioName=\%22sessionmemoryrequest\%22%20CrankerCurrentTime=\%22130451895911147968\%22]%20%20%20[WebAnswer%20ResponseFormat=\%22both\%22]%22]";
            string strResult = "";
            //"SatoriId" : "540f179c-63f8-76f6-563a-6aff4f726993"
            WebResponse objResponse;
            WebRequest objRequest = System.Net.HttpWebRequest.Create(string.Format(urlRequest, query));
            objRequest.Credentials = CredentialCache.DefaultCredentials;
            List<string> entities = new List<string>();
            try
            {
                objResponse = objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
                entities = BingParseEntities(strResult);
            }
            catch (Exception e)
            {
                Console.WriteLine("Get bing entites failed.");
                return BadRequest(e.Message);
            }
            return Ok(new
            {
                entities = entities
            });
        }

        private List<string> BingParseEntities(string xml)
        {
            var entities = new List<string>();
            string cleaned = xml.Replace("\"", "").Replace("\\", "");
            string pattern = "SatoriId:([A-Za-z0-9\\-]+),";

            Match match = Regex.Match(cleaned, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                entities.Add(match.Groups[1].Value);
            }
            return entities;
        }

        [HttpGet]
        [Route("api/dwimApi/recognizebingentities/")]
        [ResponseType(typeof(object))]
        public IHttpActionResult RecognizeBingEntitis(string sentence)
        {
            var text = sentence;
            string[] words = Regex.Replace(sentence, "/[^a-zA-Z]/g", "").Split(' ');
            string[] distinctWords = words.Distinct().ToArray();
            string[] keywords = RemoveStopWords(distinctWords);
            for (int i = 0; i < keywords.Length; i++)
            {
                var entity = GetBingEntities(keywords[i]);
                var linkEntity = "<a href='#' style='color:red'>" + keywords[i] + "</a>";
                text.Replace(keywords[i], linkEntity);
            }
            return Ok(new
            {
                text = text
            });
        }

        private string[] RemoveStopWords(string[] words)
        {
            string stopWordsFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "stopwords.txt");
            List<string> lines = new List<string>();
            if (File.Exists(stopWordsFilePath))
            {
                lines = File.ReadAllLines(stopWordsFilePath).ToList();
            }
            List<string> keywords = new List<string>();
            for(int i = 0; i < words.Length; i++)
            {
                if(!lines.Contains(words[i]))
                {
                    keywords.Add(words[i]);
                }
            }
            return keywords.ToArray();
        }

    }
}