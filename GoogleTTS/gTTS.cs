using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace GoogleTTS
{
    public class gTTS
    {
        private const ushort TokenLength = 175;
        private const string TtsUrl = "https://translate.google.com/translate_tts";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.17763";

        private List<byte[]> byteList = new List<byte[]>();

        public gTTS(string text, string lang)
        {
            var strings = Regex.Split(text, "[!?.\n]");

            foreach (var str in strings)
            {
                if (str.Length < TokenLength)
                    downloadByteArray(str, lang);
                else
                {
                    var tmpStr = str;

                    do
                    {
                        ushort index = TokenLength;

                        for (ushort i = TokenLength; i > 0; i--)
                            if (tmpStr[i] == ',')
                            {
                                index = i;
                                break;
                            }

                        downloadByteArray(tmpStr.Substring(0, index), lang);
                        tmpStr = tmpStr.Remove(0, index);
                    } while (tmpStr.Length > TokenLength);

                    downloadByteArray(tmpStr, lang);
                }
            }
        }

        private void downloadByteArray(string text, string lang)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                return;

            System.Console.WriteLine(text);

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
    }
}