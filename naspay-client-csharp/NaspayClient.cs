using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace Naspay
{
    public class NaspayClient
    {
        private const string userAgent = "naspay csharp client 0.1";

        private readonly string apiKey;
        private readonly string apiSecret;
        private readonly string baseUrl;
        private readonly bool debug;

        private string accessToken;

        public NaspayClient(string domain, string apiKey, string apiSecret, bool debug)
        {
            this.baseUrl = "https://" + domain;
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.debug = debug;
        }

        public void Authenticate()
        {
            string basicCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey + ":" + apiSecret));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/auth/token");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.UserAgent = userAgent;
            request.Headers.Add("Authorization", "Basic " + basicCredentials);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseBody = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();

            JavaScriptSerializer jss = new JavaScriptSerializer();
            Dictionary<string, object> obj = jss.Deserialize<Dictionary<string, object>>(responseBody);
            accessToken = (string)obj["access_token"];
            if (debug)
            {
                Console.WriteLine("Got access token " + accessToken);
            }
        }

        public Dictionary<string, object> DoApiRequest(string resource, HttpMethod method, object requestObject)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Not authenticated");
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "/api/v1" + resource);
            request.Method = method.Method;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.UserAgent = userAgent;
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            JavaScriptSerializer jss = new JavaScriptSerializer();

            if (requestObject != null)
            {
                string json = jss.Serialize(requestObject);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {                    
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseBody = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();

            if (debug)
            {
                Console.WriteLine("response: " + responseBody);
            }

            return jss.Deserialize<Dictionary<string, object>>(responseBody);
        }


    }
}
