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
            string path = $"{Path.GetTempFileName()}.oga";

            using (var client = new WebClient())
            {
                client.DownloadFile(speechURI, path);
            }

            int sampleRateHertz = TagLib.File.Create(path).Properties.AudioSampleRate;

            var speech = SpeechClient.Create();
            
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = audioEncoding,
                SampleRateHertz = sampleRateHertz,
                LanguageCode = languageCode,
                EnableAutomaticPunctuation = true,
            }, RecognitionAudio.FromFile(path));

            File.Delete(path);

            Result = string.Empty;

            foreach (var result in response.Results)
                foreach (var alternative in result.Alternatives)
                    Result += alternative.Transcript;

            if (string.IsNullOrEmpty(Result))
                throw new ArgumentNullException();
        }
    }
}