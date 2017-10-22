using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Todo.Models;

namespace Todo.Services
{
    public class TodoItemService
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly string baseUrl;

        public TodoItemService(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        #region Implementation

        public async Task<ICollection<TodoItem>> GetAsync()
        {
            var uri = new Uri($"{baseUrl}/items");
            return await Execute<TodoItem[]>(HttpMethod.Get, uri);
        }

        public async Task<TodoItem> GetAsync(int id)
        {
            var uri = new Uri($"{baseUrl}/items/{id}");
            return await Execute<TodoItem>(HttpMethod.Get, uri);
        }

        public async Task<ICollection<TodoItem>> GetNotDoneAsync()
        {
            // Create necessary api on server side
            var items = await GetAsync();
            return items.Where(i => !i.Done).ToArray();
        }

        public async Task UpdateAsync(TodoItem item)
        {
            var uri = new Uri($"{baseUrl}/items/{item.Id}");
            await Execute(HttpMethod.Put, uri, item);
        }

        public async Task InsertAsync(TodoItem item)
        {
            var uri = new Uri($"{baseUrl}/items");
            await Execute(HttpMethod.Post, uri, item);
        }

        public async Task SaveAsync(TodoItem item)
        {
            if (item.Id == default(int))
            {
                await InsertAsync(item);
            }
            else
            {
                await UpdateAsync(item);
            }
        }

        public async Task DeleteAsync(int itemId)
        {
            var uri = new Uri($"{baseUrl}/items/{itemId}");
            await Execute(HttpMethod.Delete, uri);
        }

        #endregion

        private async Task<T> Execute<T>(HttpMethod method, Uri uri, object content = null)
        {
            var message = new HttpRequestMessage(method, uri);
            SetHeaders(message);
            if (content != null)
            {
                message.Content = GetJsonContent(content);
            }

            return await ProcessResponse<T, FailedResponse>(message);
        }
        private async Task Execute(HttpMethod method, Uri uri, object content = null)
        {
            var message = new HttpRequestMessage(method, uri);
            SetHeaders(message);
            if (content != null)
            {
                message.Content = GetJsonContent(content);
            }

            await SendRequest<FailedResponse>(message);
        }

        private static void SetHeaders(HttpRequestMessage message)
        {
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        private static HttpContent GetJsonContent(object content)
        {
            var stringContent = JToken.FromObject(content, Serializer).ToString();
            var httpContent = new StringContent(stringContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpContent;
        }

        private async Task SendRequest<TErrorResponse>(HttpRequestMessage message)
            where TErrorResponse : IFailedResponse
        {
            var response = await GetResponse(message);
            if (!response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jReader = new JsonTextReader(reader))
                {
                    var failedResponseObj = Serializer.Deserialize<TErrorResponse>(jReader);
                    if (failedResponseObj != null && !string.IsNullOrEmpty(failedResponseObj.ErrorMessage))
                    {
                        throw new Exception(failedResponseObj.ErrorMessage);
                    }

                    throw new Exception(GetCommonHttpErrorMessage(message));
                }
            }
        }

        private async Task<TResponse> ProcessResponse<TResponse, TErrorResponse>(HttpRequestMessage message)
            where TErrorResponse : IFailedResponse
        {
            var response = await GetResponse(message);
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var jReader = new JsonTextReader(reader))
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return default(TResponse);
                }

                if (response.IsSuccessStatusCode)
                {
                    var respObj = Serializer.Deserialize<TResponse>(jReader);
                    if (respObj == null)
                    {
                        throw new Exception(GetCommonHttpErrorMessage(message));
                    }

                    return respObj;
                }

                var failedResponseObj = Serializer.Deserialize<TErrorResponse>(jReader);
                if (failedResponseObj != null)
                {
                    throw new Exception(failedResponseObj.ErrorMessage);
                }

                throw new Exception(GetCommonHttpErrorMessage(message));
            }
        }

        private async Task<HttpResponseMessage> GetResponse(HttpRequestMessage message)
        {
            try
            {
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = cookies;
                HttpClient client = new HttpClient(handler);
                return await client.SendAsync(message);
            }
            catch (Exception e)
            {
                throw new Exception(GetCommonHttpErrorMessage(message), e);
            }
        }

        protected static string GetCommonHttpErrorMessage(HttpRequestMessage message)
        {
            return $"Unknown response object received for service {message.RequestUri}.";
        }
    }
}
