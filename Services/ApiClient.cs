
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PeerReview.MvcHotel.Services
{
    public class ApiClient
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _cfg;
        private readonly IHttpContextAccessor _ctx;

        public ApiClient(IHttpClientFactory httpFactory, IConfiguration cfg, IHttpContextAccessor ctx)
        {
            _httpFactory = httpFactory; _cfg = cfg; _ctx = ctx;
        }

        private HttpClient Create()
        {
            var client = _httpFactory.CreateClient();
            client.BaseAddress = new Uri(_cfg["ApiBaseUrl"] ?? "http://localhost:5215");
            var token = _ctx.HttpContext?.Session.GetString("jwt");
            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<T?> Get<T>(string url)
        {
            var http = Create();
            var res = await http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<HttpResponseMessage> Post<T>(string url, T body)
        {
            var http = Create();
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await http.PostAsync(url, content);
        }

        public async Task<HttpResponseMessage> Put<T>(string url, T body)
        {
            var http = Create();
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await http.PutAsync(url, content);
        }

        public async Task<HttpResponseMessage> Delete(string url)
        {
            var http = Create();
            return await http.DeleteAsync(url);
        }

        public async Task<HttpResponseMessage> PostMultipart(string url, Dictionary<string,string> fields, (string name, string fileName, byte[] bytes, string contentType)? file = null)
        {
            var http = Create();
            using var content = new MultipartFormDataContent();
            foreach (var kv in fields) content.Add(new StringContent(kv.Value), kv.Key);
            if (file != null)
            {
                var b = new ByteArrayContent(file.Value.bytes);
                b.Headers.ContentType = new MediaTypeHeaderValue(file.Value.contentType);
                content.Add(b, file.Value.name, file.Value.fileName);
            }
            return await http.PostAsync(url, content);
        }
    }
}
