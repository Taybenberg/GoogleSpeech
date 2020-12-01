using System;
using System.IO;
using System.Net;
using Google.Cloud.Speech.V1;

namespace GoogleSTT
{
    public class gSTT
    {
        public string Result { get; private set; }

        public gSTT(string speechURI, string languageCode = LanguageCodes.Ukrainian.Ukraine, RecognitionConfig.Types.AudioEncoding audioEncoding = RecognitionConfig.Types.AudioEncoding.OggOpus)
        {
            byte[] data;

            using (var client = new WebClient())
            {
                data = client.DownloadData(speechURI);
            }

            var response = SpeechClient.Create().Recognize(new RecognitionConfig()
            {
                Encoding = audioEncoding,
                SampleRateHertz = new TagLibFileAbstraction(speechURI, data).AudioSampleRate,
                LanguageCode = languageCode,
                EnableAutomaticPunctuation = true,
            }, RecognitionAudio.FromBytes(data));

            Result = string.Empty;

            foreach (var result in response.Results)
                foreach (var alternative in result.Alternatives)
                    Result += alternative.Transcript;

            if (string.IsNullOrEmpty(Result))
                throw new ArgumentNullException();
        }
    }
}