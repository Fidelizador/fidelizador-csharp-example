
using System.Text.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;

namespace FidelizadorApiClient {
    public class FidelizadorAuthenticator : AuthenticatorBase {
        readonly string _baseUrl = null!;
        readonly string _clientId = null!;
        readonly string _clientSecret = null!;

        public FidelizadorAuthenticator(string baseUrl, string clientId, string clientSecret) : base("") {
            _baseUrl      = baseUrl;
            _clientId     = clientId;
            _clientSecret = clientSecret;
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken) {
            var token = string.IsNullOrEmpty(Token) ? await GetToken() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, token);
        }

        async Task<string> GetToken() {
            var options = new RestClientOptions(_baseUrl);
            using var client = new RestClient(options);

            var request = new RestRequest("oauth/v2/token");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            var response = await client.GetAsync<TokenResponse>(request);
            return $"Bearer {response!.AccessToken}";
        }
        record TokenResponse
        {
            [JsonPropertyName("token_type")]
            public string TokenType { get; init; } = null!;
            [JsonPropertyName("access_token")]
            public string AccessToken { get; init; } = null!;

        }
    }

    public class FidelizadorClient {
        readonly RestClient _client = null!;

        readonly string _clientSlug = null!;

        //
        // Summary:
        //     Default constructor
        public FidelizadorClient(string apiKey, string apiKeySecret, string clientSlug) {
            var options = new RestClientOptions("https://api.fidelizador.com/");

            _clientSlug = clientSlug;
            _client = new RestClient(options) {
                Authenticator = new FidelizadorAuthenticator("https://api.fidelizador.com", apiKey, apiKeySecret)
            };
        }

        public async Task<Lists?> GetLists() {
            var request = new RestRequest("1.0/list.json");
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddHeader("X-Client-Slug", _clientSlug);

            var response = await _client.GetAsync<Lists>(request);
            return response;
        }

        public async Task<List?> CreateList(string name) {
            var request = new RestRequest("1.0/list.json", Method.Post);
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddHeader("X-Client-Slug", _clientSlug);
            request.AddBody(new {
                name = name,
                fields = new string[1] { "FIRSTNAME" }
            });

            var response = await _client.PostAsync<List>(request);
            return response;
        }

        public async Task<string?> StartImport(int listId, string filePath) {
            var request = new RestRequest("1.0/list/{listId}/import.json", Method.Post);
            request.AddUrlSegment("listId", listId);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("X-Client-Slug", _clientSlug);
            request.AddParameter("fields[EMAIL]", "0");
            request.AddParameter("fields[FIRSTNAME]", "1");
            request.AddFile("file", filePath, "text/csv");

            var response = await _client.ExecutePostAsync(request);
            return response.Content;
        }

        public async Task<string?> CreateCampaign(string name, int campaignType, int listId, int categoryId, string subject, string toName, string replyTo, string fromEmail) {
            var request = new RestRequest("1.0/campaign.json", Method.Post);
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddHeader("X-Client-Slug", _clientSlug);

            request.AddBody(new {
                name = name,
                type = campaignType,
                list_id = listId,
                category_id = categoryId,
                subject = subject,
                to_name = toName,
                reply_to = replyTo,
                from_email = fromEmail
            });

            var response = await _client.PostAsync(request);
            return response.Content;
        }

        public async Task<string?> ScheduleCampaign(int campaignId, int sendNow) {
            var request = new RestRequest("1.0/campaign/{campaignId}/schedule.json", Method.Post);
            request.AddUrlSegment("campaignId", campaignId);
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddHeader("X-Client-Slug", _clientSlug);

            request.AddBody(new {
                send_now = sendNow
            });
            request.AddParameter("send_now", sendNow);

            var response = await _client.PostAsync(request);
            return response.Content;
        }

        public void Dispose() {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }

        public record Lists
        {
            [JsonPropertyName("lists")]
            public dynamic[] Data { get; init; } = null!;
        }

        public record List
        {
            [JsonPropertyName("list_id")]
            public int Id { get; init; } = 0!;
        }

    }

}
