using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace TelegramTest
{
    public class gTTS
    {
        private const string TtsUrl = "https://translate.google.com/translate_tts";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.17763";

        private const ushort MaxTokenLength = 175;

        private List<byte[]> byteList = new List<byte[]>();

        private string lang;

        public gTTS(string text, string language)
        {
            lang = language;
            tokenizer(text);
        }

        private void downloadByteArray(string text)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", UserAgent);
                webClient.QueryString.Add("ie", "UTF-8");
                webClient.QueryString.Add("client", "tw-ob");
                webClient.QueryString.Add("tl", lang);
                webClient.QueryString.Add("q", WebUtility.UrlEncode(text));
                webClient.UseDefaultCredentials = true;

                byteList.Add(webClient.DownloadData(TtsUrl));
            }
        }

        public void WriteFile(string path)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    foreach (var bytes in byteList)
                        binaryWriter.Write(bytes);

                    binaryWriter.Flush();
                    binaryWriter.Close();

                    fileStream.Close();
                }
            }
        }

        private void tokenizer(string text)
        {
            var strings = text.Split(new[] { '!', '?', '.', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var str in strings)
            {
                if (str.Length <= MaxTokenLength)
                    downloadByteArray(str);
                else
                    tokenizer2ndLevel(str);
            }
        }

        private void tokenizer2ndLevel(string text)
        {
            var strings = text.Split(new[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var str in strings)
            {
                if (str.Length <= MaxTokenLength)
                    downloadByteArray(str);
                else
                    tokenizer3rdLevel(str);
            }
        }

        private void tokenizer3rdLevel(string text)
        {
            var strings = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var str in strings)
            {
                if (str.Length <= MaxTokenLength)
                    downloadByteArray(str);
                else
                    tokenizer4thLevel(str);
            }
        }

        private void tokenizer4thLevel(string text)
        {
            string tmpStr = String.Copy(text);

            do
            {
                downloadByteArray(tmpStr.Substring(0, MaxTokenLength));
                tmpStr = tmpStr.Remove(0, MaxTokenLength);
            } while (tmpStr.Length > MaxTokenLength);

            downloadByteArray(tmpStr);
        }
    }
}