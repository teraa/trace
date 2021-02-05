using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Twitch.PubSub;

namespace TwitchLogger.Services
{
    class PubSubClientConfig
    {
        public IReadOnlyList<Topic> Topics { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    class PubSubClientService : IHostedService
    {
        private readonly TwitchPubSubClient _client;
        private readonly ILogger<PubSubClientService> _logger;
        private readonly PubSubClientConfig _config;

        public PubSubClientService(
            TwitchPubSubClient client,
            ILogger<PubSubClientService> logger,
            PubSubClientConfig config)
        {
            _client = client;
            _logger = logger;
            _config = config;

            _client.Connected += ConnectedAsync;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.ConnectAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
        }

        private async Task ConnectedAsync()
        {
            var tasks = new List<Task<PubSubMessage?>>();
            foreach (var topic in _config.Topics)
                tasks.Add(_client.ListenAsync(topic, _config.Token));

            var responses = await Task.WhenAll(tasks);

            foreach (var (topic, response) in _config.Topics.Zip(responses))
            {
                if (response is null)
                    _logger.LogError($"No listen response received for {topic}.");
                else if (response.Error is { Length: > 0 })
                    _logger.LogError($"Error listening {topic}: {response.Error}.");
                else
                    _logger.LogInformation($"Successfully listened {topic}.");
            }
        }
    }
}
