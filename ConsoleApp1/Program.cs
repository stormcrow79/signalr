
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

            connection.On("Callback", () => Console.WriteLine("LongRequest called back"));

            await connection.StartAsync();

            var result = await connection.InvokeAsync<string[]>(
                "GetSampleValues", 3);
            foreach (var item in result)
                Console.WriteLine(item);

            Console.ReadLine();

            await connection.SendAsync("LongRequestWithCallback");
            Console.WriteLine("LongRequest sent ...");

            Console.ReadLine();

        }
    }
}
