using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public class TriggerRecognizerBackgroundService : BackgroundService
    {
        private readonly ITriggerDispatcher triggerDispatcher;
        private readonly SpeechRecognizer speechRecognizer;
        private readonly ILogger<TriggerRecognizerBackgroundService> logger;

        public TriggerRecognizerBackgroundService(ITriggerDispatcher triggerDispatcher, IConfiguration configuration, ILogger<TriggerRecognizerBackgroundService> logger)
        {
            this.triggerDispatcher = triggerDispatcher;
            var config = SpeechConfig.FromSubscription(configuration["MsCog:SubscriptionId"], configuration["MsCog:Region"]);
            this.speechRecognizer = new SpeechRecognizer(config);
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            speechRecognizer.Recognizing += async (s, e) =>
            {
                await triggerDispatcher.Dispatch(e.Result.Text);
                logger.LogInformation($"RECOGNIZING: Text={e.Result.Text}");
            };
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await speechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                Thread.Sleep(1000);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await speechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            await base.StopAsync(cancellationToken);
        }
    }

}
