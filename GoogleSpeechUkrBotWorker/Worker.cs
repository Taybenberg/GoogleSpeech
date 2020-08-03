using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GoogleSpeechUkrBotWorker
{
    public class Worker : IHostedService
    {
        private GoogleSpeechUkrBot.GoogleSpeechUkrBot bot;

        public Worker()
        {
            /*  
             *  Хостинг AppHarbor записує конфігураційні змінні до файлу .config, 
             *  який більше не використовується в .Net Core
             *  Через це доводиться діставати токен за допомогою Regex-виразу
             */

            var text = File.ReadAllText("GoogleSpeechUkrBotWorker.dll.config");

            var match = Regex.Match(text, "\"GoogleApplicationCredentials\" value=\"({.+})\"");

            string path = Path.Combine(Path.GetTempPath(), "Google.json");

            File.WriteAllText(path, match.Groups[1].Value);

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            match = Regex.Match(text, "\"TelegramBotApiToken\" value=\"(.+)\"");

            bot = new GoogleSpeechUkrBot.GoogleSpeechUkrBot(match.Groups[1].Value);
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
