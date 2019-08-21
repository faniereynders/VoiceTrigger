using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoiceTrigger
{
    public class TriggerDispatcher
    {
        private Dictionary<string, string> options;
        public TriggerDispatcher(IOptions<Dictionary<string, string>> options, IHubContext<VoiceTriggerHub> hubContext)
        {
            this.options = options.Value;
            this.hubContext = hubContext;
        }

        private static string lastText = string.Empty;
        private static bool playing = false;
        private readonly IHubContext<VoiceTriggerHub> hubContext;

        public async Task Dispatch(string text)
        {

            if (lastText != string.Empty)
            {
                text = text.Replace(lastText, string.Empty);

                //Console.WriteLine("Last: " + lastText);
                //Console.WriteLine("new: " + text);
            }

            
            var trigger =options.SingleOrDefault(t => text.ToLower().Contains(t.Key.ToLower()));
            if (trigger.Value != null && !playing)
            {
                Console.WriteLine(trigger.Key);
                await hubContext.Clients.All.SendAsync("TriggerReceived", trigger.Key, trigger.Value);

            }
            lastText = text;
            // lastText = string.Empty;
        }

        //private static async Task Play(string file)
        //{

        //    using (var audioFile = new AudioFileReader(file))
        //    using (var outputDevice = new WaveOutEvent())
        //    {
        //        outputDevice.Init(audioFile);
        //        outputDevice.Play();
        //        while (outputDevice.PlaybackState == PlaybackState.Playing)
        //        {
        //            playing = true;
        //            await Task.Delay(1000);
        //        }
        //    }
        //    playing = false;
        //    // return Task.CompletedTask;
        //}
    }
}
