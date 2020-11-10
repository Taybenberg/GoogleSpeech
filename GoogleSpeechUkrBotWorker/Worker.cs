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

            string path = Path.GetTempFileName();

            var text = File.ReadAllText("GoogleSpeechUkrBotWorker.dll.config");

            File.WriteAllText(path, Regex.Match(text, "\"GoogleApplicationCredentials\" value=\"({.+})\"").Groups[1].Value.Replace(@"&quot;", "\""));

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            bot = new GoogleSpeechUkrBot.GoogleSpeechUkrBot(Regex.Match(text, "\"TelegramBotApiToken\" value=\"(.+)\"").Groups[1].Value, Regex.Match(text, "\"SQLSERVER_CONNECTION_STRING\" value=\"(.+)\"").Groups[1].Value);
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
