using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

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

        if (text.Length <= MaxTokenLength)
            downloadByteArray(text);
        else
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
        foreach (var str0 in text.Split(new[] { '!', '?', '.', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (str0.Length <= MaxTokenLength)
                downloadByteArray(str0);
            else
                foreach (var str1 in str0.Split(new[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (str1.Length <= MaxTokenLength)
                        downloadByteArray(str1);
                    else
                        foreach (var str2 in str1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (str2.Length <= MaxTokenLength)
                                downloadByteArray(str2);
                            else
                            {
                                var str3 = str2;

                                do
                                {
                                    downloadByteArray(str3.Substring(0, MaxTokenLength));
                                    str3 = str3.Remove(0, MaxTokenLength);
                                } while (str3.Length > MaxTokenLength);

                                downloadByteArray(str3);
                            }
                        }
                }
        }
    }
}