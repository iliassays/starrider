namespace ServerlessMicroservices.Voyages.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class HttpHelper
    {
        public static async Task<TResponse> Get<TResponse>(HttpClient httpClient, string url, Dictionary<string, string> headers, string userId = null, string password = null)
        {
            var error = "";
            HttpClient client = httpClient;
            TResponse responseObject;
            bool isDispose = httpClient == null ? true : false;

            try
            {
                if (string.IsNullOrEmpty(url))
                    throw new Exception("No URL provided!");

                // If the client is provided, we assume the headers are pre-set
                if (client == null)
                {
                    client = new HttpClient();

                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password))
                    {
                        var base64stuff = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userId}:{password}"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64stuff);
                    }
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (headers != null)
                    {
                        foreach (KeyValuePair<string, string> entry in headers)
                        {
                            client.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                        }
                    }
                }

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                    responseObject = JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
                else
                    throw new Exception($"Bad return code: {response.StatusCode} - detail: {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw ex;
            }
            finally
            {
                if (isDispose && client != null) client.Dispose();
            }

            return responseObject;
        }
    }
}
