// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using microservice_map_info.Protos;

static async Task Main(string[] args)
{
    var channel = GrpcChannel.ForAddress(new
        Uri("http://localhost:5001"));
    var client=new DistanceInfo.DistanceInfoClient(channel);
    var response = await client.GetDistanceAsync(new Cities
    {
        OriginCity = "Topeka,KS",
        DestinationCity = "Los Angeles,CA"
    });
    Console.WriteLine(response.Miles);
    Console.ReadLine();
}
