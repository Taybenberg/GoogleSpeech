using System;
using System.IO;
using Google.Cloud.Speech.V1;

namespace GoogleSTT
{
    public class gSTT
    {
        public string Result { get; private set; }

        static gSTT()
        {
            string path = Path.GetTempPath() + @"\Google.json";
            File.WriteAllBytes(path, GoogleCloudJson.Google);

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        }

        public gSTT(string speechURI, int sampleRateHertz = 48000, string languageCode = "uk-UA", RecognitionConfig.Types.AudioEncoding audioEncoding = RecognitionConfig.Types.AudioEncoding.OggOpus)
        {
            var speech = SpeechClient.Create();

            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = audioEncoding,
                SampleRateHertz = sampleRateHertz,
                LanguageCode = languageCode,
                EnableAutomaticPunctuation = true,
            }, RecognitionAudio.FetchFromUri(speechURI));

            Result = string.Empty;

            foreach (var result in response.Results)
                foreach (var alternative in result.Alternatives)
                    Result += alternative.Transcript;

            if (string.IsNullOrEmpty(Result))
                throw new ArgumentNullException();
        }
    }
}