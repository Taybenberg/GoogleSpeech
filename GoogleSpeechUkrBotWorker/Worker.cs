using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GoogleSpeechUkrBotWorker
{
    public class Worker : IHostedService
    {
        private GoogleSpeechUkrBot.GoogleSpeechUkrBot bot;

        public Worker(string telegramBotApiToken)
        {
            bot = new GoogleSpeechUkrBot.GoogleSpeechUkrBot(telegramBotApiToken);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            bot.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            bot.Stop();

            return Task.CompletedTask;
        }
    }
}
