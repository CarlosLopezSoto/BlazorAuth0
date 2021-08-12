using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TokenLibrary.Token;


namespace BalzorAuth0.Data
{
    public class WeatherForecastService
    {
        //private readonly HttpClient http;
        private readonly TokenProvider tokenProvider;

        public WeatherForecastService(
            TokenProvider tokenProvider)
        {
            //http = clientFactory.CreateClient();
            this.tokenProvider = tokenProvider;
        }
        public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var token = tokenProvider.AccessToken;
            var client3 = new RestClient("https://localhost:44339/WeatherForecast");
            var request3 = new RestRequest(Method.GET);
            request3.AddHeader("authorization", $"Bearer {token}");
            IRestResponse response3 = client3.Execute(request3);
            return JsonConvert.DeserializeObject<WeatherForecast[]>(response3.Content);
        }
    }
}
