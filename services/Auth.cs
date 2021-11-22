using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DebugOmgDispClient.services
{
    /// <summary>
    /// Auth provider
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 22.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class Auth
    {
        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;

        static async Task<string> GetToken(StringBuilder url)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "EventSink class: OnDispConsolePTTClicked method: ";

            logger.Write($"Class Auth:  method GetToken: threadId = {threadId}:  started...\n");

            logger.Write("method SignalRService(), { 0url } .\n");            

            var httpClient = new HttpClient();
            var json = new StringContent(JsonConvert.SerializeObject(
                new { Username = "demouser", Password = "P@ssword" }),
                Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url.ToString(), json);
            var authResponse = await result.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse != null)
            {
                var token = authResponse.Token;
                GlobalObjects.Token = token;
                return token;
            }
            return "";
        }
    }
}
