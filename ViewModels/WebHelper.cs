using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    /// <summary>
    /// Allows for downloading internet content without worrying about
    /// socket exhaustion and disposing resources stuff.
    /// </summary>
    public static class WebHelper
    {
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Downloads content from given url as byte array.
        /// </summary>
        /// <param name="url">Target url - source of bytes.</param>
        public static async Task<byte[]> DownloadUrlContentAsByteArray(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.Add(new("PulseApp", "1.0"));
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            using Stream stream = await response.Content.ReadAsStreamAsync();
            using MemoryStream ms = new();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
