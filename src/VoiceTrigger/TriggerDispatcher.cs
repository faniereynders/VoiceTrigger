using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public class TriggerDispatcher : ITriggerDispatcher
    {
        private Dictionary<string, TriggerOption> triggers;
        private static string lastText = string.Empty;
        private static bool playing = false;
        private readonly IHubContext<VoiceTriggerHub> hubContext;
        private readonly ILogger<TriggerDispatcher> logger;

        public TriggerDispatcher(IOptions<Dictionary<string, TriggerOption>> options, IHubContext<VoiceTriggerHub> hubContext, ILogger<TriggerDispatcher> logger)
        {
            this.triggers = options.Value;
            this.hubContext = hubContext;
            this.logger = logger;
        }

        public async Task Dispatch(string text)
        {
            try
            {
                if (lastText != string.Empty)
                {
                    text = text.Replace(lastText, string.Empty);
                }

                var trigger = triggers.SingleOrDefault(t => text.ToLower().Contains(t.Key.ToLower()));
                if (trigger.Value != null && !playing)
                {
                    logger.LogDebug(trigger.Key);
                    await hubContext.Clients.All.SendAsync("TriggerReceived", trigger.Key, trigger.Value);

                }
                lastText = text;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                
            }    
            
        }
    }
}
