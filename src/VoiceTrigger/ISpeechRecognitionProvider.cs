using System;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public interface ISpeechRecognitionProvider
    {
        //event EventHandler<T> Recognizing;

        Task Initialize(Action<GenericSpeechRecognitionResult> action);

        Task StartRecognitionAsync();
        Task StopRecognitionAsync();
    }
}