using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public class GenericSpeechRecognitionResult
    {
        public GenericSpeechRecognitionResult(string text)
        {
            this.Text = text;
        }
        public string Text { get; }
    }
    public class GenericSpeechRecognitionEventArgs
    {
        public GenericSpeechRecognitionResult Result { get; set; }
    }


    public class TriggerRecognizerBackgroundService : BackgroundService
    {
        private readonly ITriggerDispatcher triggerDispatcher;
        private readonly ISpeechRecognitionProvider speechRecognitionProvider;
        private readonly ILogger<TriggerRecognizerBackgroundService> logger;

        public TriggerRecognizerBackgroundService(ITriggerDispatcher triggerDispatcher, IConfiguration configuration, ILogger<TriggerRecognizerBackgroundService> logger, ISpeechRecognitionProvider speechRecognitionProvider)
        {
            this.triggerDispatcher = triggerDispatcher;
            this.speechRecognitionProvider = speechRecognitionProvider;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            //speechRecognitionProvider.Initialize((s, t) =>
            //{
            //    await triggerDispatcher.Dispatch(e.Result.Text);
            //    logger.LogInformation($"RECOGNIZING: Text={e.Result.Text}");
            //};


            speechRecognitionProvider.Initialize(async (result) =>
            {
                await triggerDispatcher.Dispatch(result.Text);
                logger.LogInformation($"RECOGNIZING: Text={result.Text}");
            });
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await speechRecognitionProvider.StartRecognitionAsync().ConfigureAwait(false);
                Thread.Sleep(1000);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await speechRecognitionProvider.StopRecognitionAsync().ConfigureAwait(false);
            await base.StopAsync(cancellationToken);
        }
    }

}
