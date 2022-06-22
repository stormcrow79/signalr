namespace WebApplication1
{
    using Microsoft.AspNetCore.SignalR;

    public class SampleHub : Hub
    {
        private readonly ILogger<SampleHub> _logger;

        public SampleHub(
            ILogger<SampleHub> logger)
        {
            _logger = logger;
        }

        public string[] GetSampleValues(int count)
        {
            _logger.LogDebug($"GetSampleValues {{ count = ${count} }}");

            return Enumerable.Range(1, count)
                .Select(i => Guid.NewGuid().ToString())
                .ToArray();
        }

        public async void LongRequestWithCallback()
        {
            _logger.LogDebug($"LongRequestWithCallback");
         
            await Task.Delay(TimeSpan.FromSeconds(15));

            await Clients.Caller.SendAsync("Callback");
        }
    }
}
