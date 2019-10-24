using Microsoft.Extensions.Configuration;
using System;
using System.Speech.Recognition;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public class SystemSpeechRecognitionProvider : ISpeechRecognitionProvider
    {
        private readonly SpeechRecognitionEngine speechRecognizer;
        private bool completed = false;
        public SystemSpeechRecognitionProvider(IConfiguration configuration)
        {
            speechRecognizer = new SpeechRecognitionEngine();
            var dictationGrammar = new DictationGrammar();
            speechRecognizer.LoadGrammar(dictationGrammar);
            speechRecognizer.SetInputToDefaultAudioDevice();
        }

        public async Task StartRecognitionAsync()
        {
            if (completed)
            {
                speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            
            while (!completed)
            {
                await Task.Delay(1000);
            }
        }
        public async Task StopRecognitionAsync()
        {
            await Task.CompletedTask;
        }

        public Task Initialize(Action<GenericSpeechRecognitionResult> action)
        {
            void Completed(string text)
            {
                action(new GenericSpeechRecognitionResult(text));
                Console.WriteLine(text);
                completed = true;
            }
            void InProgess()
            {
                completed = false;
            }

            speechRecognizer.SpeechRecognized += (s,e)=> Completed(e.Result.Text);
            speechRecognizer.SpeechDetected += (s, e) => InProgess();
            speechRecognizer.SpeechHypothesized += (s, e) => Completed(e.Result.Text);
            speechRecognizer.SpeechRecognitionRejected += (s, e) => Completed(e.Result.Text);
            speechRecognizer.RecognizeCompleted += (s, e) => Completed(e.Result.Text);
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);

            return Task.CompletedTask;
        }
        
    }
}
