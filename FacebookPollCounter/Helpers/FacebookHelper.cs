﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPollCounter.Helpers
{
    public static class FacebookHelper
    {
        static FacebookHelper()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseApiUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        private static HttpClient _client;
        private static string _baseApiUrl = "https://graph.facebook.com/v2.9/";

        public static async Task<PagedList<Comment>> GetComments(string token, string postId, string from, int limit = 1000)
        {
            var requestUrl = $"{postId}/comments?access_token={token}&total_count=1&order=chronological&filter=stream&summary=1&limit={limit}";

            var response = await _client.GetAsync(requestUrl).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PagedList<Comment>>(json);
            }

            return null;
        }

        public static async Task<string> GetPostIdFromUrl(string token, string url)
        {
            var parts = url.Replace("https://www.facebook.com/", string.Empty).Split('/');
            var pageName = parts.First();
            var postId = parts.Last().Split(new char[] { ':', '?' }).First();

            var requestUrl = $"{pageName}?access_token={token}";

            var response = await _client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var creator = JsonConvert.DeserializeObject<Item>(json);

                return $"{creator.Id}_{postId}";
            }

            return null;
        }
    }
}