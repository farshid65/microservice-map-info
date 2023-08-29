using GoogleMapInfo;
using Grpc.Core;
using microservice_map_info.Protos;
using Prometheus;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace microservice_map_info.Services
{
    public class DistanceInfoService: DistanceInfo.DistanceInfoBase
    {
        private static readonly ActivitySource activitySource =
            new ActivitySource("microservice-map-info.DistanceInfoService");
        private static readonly Counter googleApiCount =
            Metrics.CreateCounter(
                "google_api_calls_total",
                "Number of times Google geolocation api is called");
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _googleDistanceApi;

        public DistanceInfoService(IHttpClientFactory clientFactory,IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _googleDistanceApi = configuration["googleDistanceApi:apiKey"];
        }
        public async Task <GoogleDistanceData> GetMapDistanceAsync(string originCity,string destinationCity)
        {
            if (string.IsNullOrWhiteSpace(originCity)||
                string.IsNullOrWhiteSpace(destinationCity)||
                string.IsNullOrWhiteSpace(_googleDistanceApi)) 
            {
                return new GoogleDistanceData();
            }

            using var activity = activitySource.StartActivity("GoogleMapsAPI");
            activity.SetTag("google.originCity", originCity);
            activity.SetTag("gooogle.destinationCity", originCity);

            var googleUrl = $"?units=origintcity={originCity}" +
                $"&destinationcity={destinationCity}" +
                $"&key={_googleDistanceApi}";

            using var client= _clientFactory.CreateClient("googleApi");

            var request = new HttpRequestMessage(HttpMethod.Get, googleUrl);

            var response= await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            await using var data = await response.Content.ReadAsStreamAsync();

            var distanceInfo= await JsonSerializer.DeserializeAsync<GoogleDistanceData>(data);
            activity?.SetTag("google.status", distanceInfo?.status ?? "null");

            return distanceInfo;


        }
      public override async Task<DistanceData> GetDistance(Cities cities,ServerCallContext context)
        {
            
            googleApiCount.Inc();
            var totalMiles = "0";
            var distanceData=await GetMapDistanceAsync(cities.OriginCity,cities.DestinationCity);
            foreach (var distanceDataRow in distanceData.rows)
            {
                foreach (var element in distanceDataRow.elements)
                {
                    totalMiles = element.distance.text;
                }               
            }
            return new DistanceData { Miles = totalMiles };
        }
        }
    }

