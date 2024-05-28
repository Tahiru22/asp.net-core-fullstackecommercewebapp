using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace fullstackecommercewebapp.Services
{
    public class PaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public PaystackService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _secretKey = configuration["Paystack:SecretKey"];
        }

        public async Task<string> InitializeTransactionAsync(decimal amount, string email, string callbackUrl)
        {
            var requestContent = new
            {
                email,
                amount = (int)(amount * 100), // Convert to kobo
                callback_url = callbackUrl
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

            var response = await _httpClient.PostAsync("https://api.paystack.co/transaction/initialize", content);
            response.EnsureSuccessStatusCode(); // This line ensures that an exception is thrown for unsuccessful responses

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseBody);

            return result.data.authorization_url;
        }

        public async Task<PaystackVerificationResponse> VerifyTransaction(string reference)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
            var response = await _httpClient.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PaystackVerificationResponse>(responseBody);

            return result;
        }
    }

    public class PaystackVerificationResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public VerificationData Data { get; set; }
    }

    public class VerificationData
    {
        public string Status { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
    }

}
