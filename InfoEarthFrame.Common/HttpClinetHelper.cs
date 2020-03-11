using SocketIOClient.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common
{
    public static class HttpClinetHelper
    {

        public static string GetResponseJson(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                   new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseJson = response.Content.ReadAsStringAsync().Result;
                    return responseJson;
                }
                else
                {
                    return "Error,StatusCode:" + response.StatusCode.ToString();
                }
            }
        }

        public static string PostResponseJson(string url, string requestJson)
        {
            using (HttpContent httpContent = new StringContent(requestJson))
            {
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpClient httpClient = new HttpClient())
                {

                    HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = response.Content.ReadAsStringAsync().Result;
                        return responseJson;
                    }
                    else
                    {
                        return "Error,StatusCode:" + response.StatusCode.ToString();
                    }
                }
            }
        }


        public static string PostResponse(string url, string requestStr)
        {
            using (HttpContent httpContent = new StringContent(requestStr))
            {
                using (HttpClient httpClient = new HttpClient())
                {

                    HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = response.Content.ReadAsStringAsync().Result;
                        return responseJson;
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            throw new Exception(response.Content.ReadAsStringAsync().Result);
                        }
                        else
                        {
                            return response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
        }
    }
}