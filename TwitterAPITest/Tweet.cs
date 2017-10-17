using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TwitterAPITest
{
    class Tweet : ITweet
    {
        //        public async void GetTweetsAsync(string searchText)
        //        {
        //            HttpClient client = new HttpClient();

        //            string auHeader = @"
        //            Authorization:
        //            OAuth oauth_consumer_key = "xvz1evFS4wEEPTGEFPHBog",
        //            oauth_nonce = "kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg",
        //            oauth_signature = "tnnArxj06cWHq44gCs1OSKk%2FjLY%3D",
        //            oauth_signature_method = "HMAC-SHA1",
        //            oauth_timestamp = "1318622958",
        //            oauth_token = "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb",
        //            oauth_version = "1.0"
        //Content - Length: 76
        //Host: api.twitter.com"

        //            client.DefaultRequestHeaders.Add()
        //           HttpResponseMessage resp= await client.GetAsync("https://api.twitter.com/1.1/search/tweets.json?" +
        //               "q=datascience&result_type=mixed&count=4");
        //            Console.WriteLine(resp.Content.ToString());
        //            Console.ReadKey();

        //        }

        private string EncodeCharacters(string data)
        {
            //as per OAuth Core 1.0 Characters in the unreserved character set MUST NOT be encoded
            //unreserved = ALPHA, DIGIT, '-', '.', '_', '~'
            if (data.Contains("!"))
                data = data.Replace("!", "%21");
            if (data.Contains("'"))
                data = data.Replace("'", "%27");
            if (data.Contains("("))
                data = data.Replace("(", "%28");
            if (data.Contains(")"))
                data = data.Replace(")", "%29");
            if (data.Contains("*"))
                data = data.Replace("*", "%2A");
            if (data.Contains(","))
                data = data.Replace(",", "%2C");
            return data;
        }

            public void GetTweetsAsync(string searchText)
        {
            string baseAddress = "https://api.twitter.com/1.1/search/";
            string url = "tweets.json?q="+ searchText;
            string url1 = "https://api.twitter.com/1.1/search/tweets.json?q=" + searchText;
            string oauthconsumerkey = "QLRM5Khvsisof5TvtVO7Crzqe";
            string oauthtoken = "136903911-pevT5f8LoxIHvTjyF2auBA6Ghx8mvc79Aq5tSfUd";
            string oauthconsumersecret = "I7h6CI02xXvtBHfql5iMHB1iQRVQWDI132v0OejUczfeG0nx8V";
            string oauthtokensecret = "XA74exvRbuC31Kg3NCFUG4olnPiZrPSELCIANqZQmsnLK";
            string oauthsignaturemethod = "HMAC-SHA1";
            string oauthversion = "1.0";
            string oauthnonce = Convert.ToBase64String(
              new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauthtimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
            SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string>();
            basestringParameters.Add("q",searchText);
            basestringParameters.Add("oauth_version", oauthversion);
            basestringParameters.Add("oauth_consumer_key", oauthconsumerkey);
            basestringParameters.Add("oauth_nonce", oauthnonce);
            basestringParameters.Add("oauth_signature_method", oauthsignaturemethod);
            basestringParameters.Add("oauth_timestamp", oauthtimestamp);
            basestringParameters.Add("oauth_token", oauthtoken);
            //Build the signature string
            StringBuilder baseString = new StringBuilder();
            baseString.Append("GET" + "&");
            baseString.Append(EncodeCharacters(Uri.EscapeDataString(url1.Split('?')[0]) + "&"));
            foreach (KeyValuePair<string, string> entry in basestringParameters)
            {
                baseString.Append(EncodeCharacters(Uri.EscapeDataString(entry.Key + "=" + entry.Value + "&")));
            }

            //Remove the trailing ambersand char last 3 chars - %26
            string finalBaseString = baseString.ToString().Substring(0, baseString.Length - 3);

            //Build the signing key
            string signingKey = EncodeCharacters(Uri.EscapeDataString(oauthconsumersecret)) + "&" +
            EncodeCharacters(Uri.EscapeDataString(oauthtokensecret));

            //Sign the request
            HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
            string oauthsignature = Convert.ToBase64String(
              hasher.ComputeHash(new ASCIIEncoding().GetBytes(finalBaseString)));

            //Tell Twitter we don't do the 100 continue thing
            // ServicePointManager.Expect100Continue = false;

            //authorization header
            HttpClient webRequest = new HttpClient();
            webRequest.BaseAddress = new Uri(@baseAddress);
            StringBuilder authorizationHeaderParams = new StringBuilder();
            authorizationHeaderParams.Append("OAuth ");
            authorizationHeaderParams.Append("oauth_consumer_key=" + "\"" + Uri.EscapeDataString(oauthconsumerkey) + "\",");
            authorizationHeaderParams.Append("oauth_nonce=" + "\"" + Uri.EscapeDataString(oauthnonce) + "\",");
            authorizationHeaderParams.Append("oauth_signature=" + "\"" + Uri.EscapeDataString(oauthsignature) + "\",");
            authorizationHeaderParams.Append("oauth_signature_method=" + "\"" + Uri.EscapeDataString(oauthsignaturemethod) + "\",");
            authorizationHeaderParams.Append("oauth_timestamp=" + "\"" + Uri.EscapeDataString(oauthtimestamp) + "\",");
            
            if (!string.IsNullOrEmpty(oauthtoken))
                authorizationHeaderParams.Append("oauth_token=" + "\"" + Uri.EscapeDataString(oauthtoken) + "\",");
           
            authorizationHeaderParams.Append("oauth_version=" + "\"" + Uri.EscapeDataString(oauthversion) + "\"");

            webRequest.DefaultRequestHeaders.Add("Authorization", authorizationHeaderParams.ToString());

            //webRequest.Get
            //webRequest.ContentType = "application/x-www-form-urlencoded";

            //Allow us a reasonable timeout in case Twitter's busy
           // webRequest.ContinueTimeout = 3 * 60 * 1000;
            try
            {
                //Proxy settings
                // webRequest.Proxy = new WebProxy("enter proxy details/address");

                Stream dataStream = webRequest.GetStreamAsync(@url).Result;
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                JObject res = JObject.Parse(responseFromServer);
                res.SelectToken("statuses").SelectTokens("")
                




                string quote =(string) res.SelectToken("text");
                string id = (string)res.SelectToken("id_str");
                
                
            }
            catch (Exception ex)
            {
            }
        }
        }
    }
