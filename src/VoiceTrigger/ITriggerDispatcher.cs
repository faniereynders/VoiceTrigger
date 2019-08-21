using System.Threading.Tasks;

namespace VoiceTrigger
{
    public interface ITriggerDispatcher
    {
        Task Dispatch(string text);
    }
}