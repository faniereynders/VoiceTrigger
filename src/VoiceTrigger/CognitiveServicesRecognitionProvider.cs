using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public class CognitiveServicesRecognitionProvider : ISpeechRecognitionProvider
    {
        private readonly SpeechRecognizer speechRecognizer;

        public CognitiveServicesRecognitionProvider(IConfiguration configuration)
        {
            var config = SpeechConfig.FromSubscription(configuration["MsCog:SubscriptionId"], configuration["MsCog:Region"]);
            this.speechRecognizer = new SpeechRecognizer(config);
        }
      
        public async Task StartRecognitionAsync()
        {
            await speechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
        }
        public async Task StopRecognitionAsync()
        {
            await speechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        }

        public Task Initialize(Action<GenericSpeechRecognitionResult> action) 
        {
            this.speechRecognizer.Recognizing += (s, e) =>
            {
                action(new GenericSpeechRecognitionResult(e.Result.Text));
            };

            return Task.CompletedTask;
        }
    }
}
