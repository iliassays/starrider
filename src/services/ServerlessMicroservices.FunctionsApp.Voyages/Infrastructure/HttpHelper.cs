﻿namespace ServerlessMicroservices.Voyages.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class EventPublisher
    {
        public static async Task TriggerEventGridTopic<T>(HttpClient httpClient,
            T request, string eventType,
            string eventSubject,
            string eventGridTopicUrl,
            string eventGridTopicApiKey)
        {
            var error = "";

            try
            {
                if (string.IsNullOrEmpty(eventType) || string.IsNullOrEmpty(eventSubject) || string.IsNullOrEmpty(eventGridTopicUrl) || string.IsNullOrEmpty(eventGridTopicApiKey))
                    return;

                var events = new List<dynamic>
                {
                    new
                    {
                        EventType = eventType,
                        EventTime = DateTime.UtcNow,
                        Id = Guid.NewGuid().ToString(),
                        Subject = eventSubject,
                        Data = request
                    }
                };

                var headers = new Dictionary<string, string>() {
                    { "aeg-sas-key", eventGridTopicApiKey }
                };

                await Post<dynamic, dynamic>(httpClient, events, eventGridTopicUrl, headers);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                throw ex;
            }
        }

        public static async Task<TResponse> Post<TRequest, TResponse>(HttpClient httpClient, TRequest requestObject, string url, Dictionary<string, string> headers, string userId = null, string password = null)
        {
            var error = "";
            HttpClient client = httpClient;
            TResponse responseObject;
            bool isDispose = httpClient == null ? true : false;

            try
            {
                if (string.IsNullOrEmpty(url))
                    throw new Exception("No URL provided!");

                var postData = JsonConvert.SerializeObject(requestObject,
                                                      new JsonSerializerSettings()
                                                      {
                                                          NullValueHandling = NullValueHandling.Ignore,
                                                          Formatting = Formatting.Indented, // for readability, change to None for compactness
                                                          ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                          DateTimeZoneHandling = DateTimeZoneHandling.Utc
                                                      });

                HttpContent httpContent = new StringContent(postData, Encoding.UTF8, "application/json");

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

                HttpResponseMessage response = await client.PostAsync(url, httpContent);

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
