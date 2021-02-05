using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IrcMessageParser;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Twitch.Irc;

namespace TwitchLogger.Services
{
    class ChatClientConfig
    {
        public IReadOnlyList<string> Channels { get; set; } = null!;
    }

    class ChatClientService : IHostedService
    {
        private readonly TwitchIrcClient _client;
        private readonly ILogger<ChatClientService> _logger;
        private readonly ChatClientConfig _config;

        public ChatClientService(TwitchIrcClient client, ILogger<ChatClientService> logger, ChatClientConfig config)
        {
            _client = client;
            _logger = logger;
            _config = config;

            _client.Ready += ReadyAsync;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.ConnectAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
        }

        private async Task ReadyAsync()
        {
            var tasks = new List<Task>();
            foreach (var channel in _config.Channels)
                tasks.Add(_client.SendAsync(new IrcMessage { Command = IrcCommand.JOIN, Content = new($"#{channel}") }));

            await Task.WhenAll(tasks);

            _logger.LogInformation($"Joined {_config.Channels.Count} channels: {string.Join(", ", _config.Channels)}");
        }
    }
}
