using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public class TriggerRecognizerBackgroundService : BackgroundService
    {
        private readonly TriggerRecognizer triggerRecognizer;

        public TriggerRecognizerBackgroundService(TriggerRecognizer triggerRecognizer)
        {
            this.triggerRecognizer = triggerRecognizer;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            await triggerRecognizer.SpeechContinuousRecognitionAsync();
        }
    }
}
