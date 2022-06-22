
namespace ConsoleApp1
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR.Client;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7228/hubs/sample")
                .Build();

            Console.WriteLine("Press enter to connect ...");
            Console.ReadLine();

            await connection.StartAsync();

            //connection.On<string>("ReceiveMessage", (message) =>
            //    this.Dispatcher.Invoke(() => ResponseLabel.Content = $"response from server after {(DateTime.Now - theTime).TotalMilliseconds:F0} s: {message}");

            var requestCount = 3;
            var request = new
            {
                RequestId = Guid.NewGuid(),
                Count = requestCount
            };

            var result = await connection.InvokeAsync<string[]>("GetSampleValues", requestCount);
            foreach (var item in result)
                Console.WriteLine(item);

            Console.ReadLine();

        }
    }
}
