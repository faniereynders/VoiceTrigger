using Microsoft.AspNetCore.SignalR;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public class TriggerRecognizer
    {
        private readonly SpeechConfig speechConfig;
        private readonly ILogger logger;
        private readonly TriggerDispatcher triggerDispatcher;
 
        public TriggerRecognizer(TriggerDispatcher triggerDispatcher, IConfiguration configuration, ILoggerFactory logger)
        {
            this.speechConfig = SpeechConfig.FromSubscription(configuration["MsCog:SubscriptionId"], configuration["MsCog:Region"]);
            this.logger = logger.CreateLogger<TriggerRecognizer>();
            this.triggerDispatcher = triggerDispatcher;
        }
        public async Task SpeechContinuousRecognitionAsync()
        {
            using (var recognizer = new SpeechRecognizer(speechConfig))
            {
                recognizer.Recognizing += async (s, e) => {
                    triggerDispatcher.Dispatch(e.Result.Text);
                    logger.LogInformation($"RECOGNIZING: Text={e.Result.Text}");
                    //
                };

                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                while (true)
                {
                    Thread.Sleep(1000);
                }

                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }
        }
    }
}
