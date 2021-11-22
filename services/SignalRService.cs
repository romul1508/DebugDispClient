using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.models;
using DebugOmgDispClient.models.decorator.user;
using DebugOmgDispClient.rtp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HubConnection = Microsoft.AspNetCore.SignalR.Client.HubConnection;

namespace DebugOmgDispClient.services
{
    /// <summary>
    /// The class provides access to the SignalR client functionality
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 27.10.2021 
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class SignalRService: ISignalRService
    {
        private const string TAG = "SignalrService";
                
        private HubConnection hubConnection;

        public static volatile bool isRunRX = false;

        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;


        /// <summary>
        /// initialize the artifacts necessary for communication and work with the server (connection, logger and event)
        /// </summary>
        /// <returns></returns>
        public async Task SignalRServiceInit()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "SignalRService class: SignalRServiceInit method";

            logger.Write($"{Tag}; threadId = {threadId}; state: started...\n");           

            try
            {
                StringBuilder url = new StringBuilder("ws");

                url.Append(GlobalConstants.IP_SERVER);
                url.Append("mainHub");


                logger.Write($"Class SignalRService; method SignalRServiceInit(), threadId = {threadId}, { url } .");           

                GlobalObjects.Token = await GetToken();     
                              
                logger.Write($"Class SignalRService; method SignalRServiceInit(): threadId = {threadId} : {TAG} init: {url.ToString()} .");     
                //-----------------------
                hubConnection = new HubConnectionBuilder()
                .WithUrl(url.ToString(), options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(GlobalObjects.Token);
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddDebug();
                    logging.AddConsole();
                    // logging.AddEventLog();
                })
                .Build();

                logger.Write($"Class SignalRService; method SignalRServiceInit(): threadId = {threadId} : Build complected .");         

                hubConnection.Closed += async (msg) =>
                {                    
                    logger.Write($"Class SignalRService; method SignalRServiceInit(): {TAG} closed: threadId = {threadId} reason: {msg.Message}");                    
                };
            }catch(Exception e)
            {
                logger.Write($"Class SignalRService; method SignalRServiceInit(): threadId = {threadId}: {TAG} Failed:  [Error]: {e.ToString()} .");
            }                    

        }

        //------------------------------------
        /// <summary>
        /// provides a connection to the server
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectAsync()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                await hubConnection.StartAsync();
                GlobalObjects.ConnectionId = hubConnection.ConnectionId;
                
                logger.Write($"Class SignalRService; method ConnectAsync(); threadId = {threadId}: {TAG} connect: start: connected to hub .");

                logger.Write($"Class SignalRService; method ConnectAsync(); threadId = {threadId}: hubConnection.ConnectionId = {hubConnection.ConnectionId}.");
                return true;
            }
            catch(Exception e)
            {
                logger.Write($"Class SignalRService; method ConnectAsync(); threadId = {threadId}: {TAG} connect: {"failed to connect"} .");
                return false;
            }
        }

        //------------------------------------
        /// <summary>
        /// This method is called on the server
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task define(string methodName)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "SignalRService class: async define method";

            logger.Write($"{Tag}; threadId = {threadId}; state: started...\n");       


            if (methodName == "StartAudioReceive")      
            {
                logger.Write($"{Tag}; threadId = {threadId}; methodName = <StartAudioReceive>.\n");       

                define_rx(methodName);
            }
            else if (methodName == "EndAudio")    
            {
                logger.Write($"{Tag}; threadId = {threadId}; methodName = <EndAudio>.\n");   

                define_sta(methodName);
            } 
            else if (methodName == "UpdateUsers")
            {
                logger.Write($"{Tag}; threadId = {threadId}; methodName = <UpdateUsers>.\n");

                define_us(methodName);
            }
        }
               

        /// <summary>
        /// completion of transmission of RTP packets
        /// </summary>
        /// <param name="methodName">("EndAudio")</param>
        /// <returns></returns>
        protected async Task define_sta(string methodName)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            logger.Write($"Class SignalRService; method define_sta(string methodName): {TAG}, threadId = {threadId}; define: methodName: {methodName} .");       
            try
            {
                hubConnection.On(methodName, () =>
                {
                    // completion of transmission of RTP packets

                    logger.Write($"Class SignalRService; method define(string methodName); threadId = {threadId}: {TAG} define: no parameters");

                    ProvCommunicServer.voiceReleased();
                    ProvCommunicServer.stopAudioProxy();                    

                });
            }
            catch (Exception e)
            {
                logger.Write($"Class SignalRService; method define(string methodName): threadId = {threadId}: {TAG} define: Failed: {e.Message} .");
            }
        }


        /// <summary>
        /// client method to start receiving RTP packets
        /// </summary>
        /// <param name="methodName"> ("StartAudioReceive") </param>
        /// <returns></returns>
        protected async Task define_rx(string methodName)
        {
            // client method to start receiving RTP packets ("StartAudioReceive")

            if (isRunRX == false)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                logger.Write($"\n Class SignalRService;  {TAG}, define_rx: threadId = {threadId}: methodName: {methodName} .");     

                try
                {                    
                    hubConnection.On<int>(methodName, (port) =>
                    {                                 
                        logger.Write($"Class SignalRService;  {TAG} define_rx: threadId = {threadId}: port: {port} .");

                        GlobalObjects.destinServerPort = port;
                       
                        ProvCommunicServer.startAudioProxy();
                        ProvCommunicServer.isRunRX = true;
                        ProvCommunicServer.rx = true;                                            

                });
                }
                catch (Exception e)
                {
                   logger.Write($"Class SignalRService; { TAG } define_rx:  threadId = {threadId}: Failed: {e.Message} .");
                }

                isRunRX = true;

            }
        }

        /// <summary>
        /// client method for transferring RTP packets 
        /// </summary>
        /// <param name="methodName"> ("StartAudioSend") </param>
        /// <returns></returns>
        protected async Task define_tx(string methodName)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            logger.Write($"\n Class SignalRService; {TAG}, define_tx: threadId = {threadId}: methodName: {methodName} .");

            try
            {
                hubConnection.On<int>(methodName, (port) =>
                {
                    logger.Write($"Class SignalRService; {TAG} define_tx: threadId = {threadId}: port: {port} .");

                    GlobalObjects.destinServerPort = port;
                    
                    ProvCommunicServer.startAudioProxy();
                    ProvCommunicServer.isRunTX = true;
                    
                    ProvCommunicServer.tx = true;                   

                });
            }
            catch (Exception e)
            {                
                logger.Write($"{TAG} define_tx: threadId = {threadId}: Failed: {e.Message} .");
            }
        }

        /// <summary>
        /// to update the list of available system users
        /// </summary>
        /// <param name="methodName"> ("UpdateUsers") </param>
        /// <returns></returns>
        protected async Task define_us(string methodName)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            // to update the list of available system users
            logger.Write($"{TAG}, define_us: threadId = {threadId}: methodName: {methodName} .");

            try
            {
                
                hubConnection.On<IEnumerable<models.HubConnectEntity>>(methodName, (users) =>                
                {
                    
                    logger.Write($"{TAG} define_us: threadId = {threadId}: invoked: users .");

                    GlobalObjects.HubConnects.Clear();

                    foreach (models.HubConnectEntity user in users)
                    {
                        if(user.ConnectId != GlobalObjects.ConnectionId)
                        {
                            GlobalObjects.HubConnects.Add(user);
                        }

                    }

                    if (GlobalObjects.HubConnects.Count > 0)
                    {
                        logger.Write($"{TAG} define_us: threadId = {threadId}: u r alone :");                                            
                    }                  

                });
            }
            catch (Exception e)
            {
                logger.Write($"{TAG} define_us: threadId = {threadId}: Failed: {e.Message} .");
            }
        }        

        //----------------------------------
        public async Task invoke(string methodName, string userConnID)
        {
            // int threadId = Thread.CurrentThread.ManagedThreadId;
            
            // logger.Write($"{TAG} invoke: threadId = {threadId}: started voice: {methodName} .");
            

            if (methodName == "StartVoice")
                invoke_start(methodName, userConnID );
            else if (methodName == "StopVoice")
                invoke_stop(methodName, userConnID);            
        }

        //----------------------------------
        /// <summary>
        /// To call methods in a hub
        /// </summary>
        /// <param name="methodName"> method name </param>
        /// <param name="userConnID"> custom connection id </param>
        /// <returns></returns>
        protected async Task invoke_start(string methodName, string userConnID)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            
            logger.Write($"{TAG} invoke: threadId = {threadId}: started voice: {methodName} .");
            try
            {
                await hubConnection.InvokeAsync(methodName, userConnID);
            }
            catch (Exception e) {
                
                logger.Write($"{TAG} invoke: threadId = {threadId}: started voice: Failed: {methodName} .");
            }
        }

        /// <summary>
        /// Aborts a method in a hub
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="userConnID"></param>
        /// <returns></returns>
        protected async Task invoke_stop(string methodName, string userConnID)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            
            logger.Write($"{TAG} invoke: threadId = {threadId}: stopped voice: {methodName} .");
            try
            {
                await hubConnection.InvokeAsync(methodName, userConnID);
            }
            catch (Exception e)
            {
                logger.Write($"{TAG} invoke: stopped voice: Failed: {methodName} .");
            }
        }

        /// <summary>
        /// Terminating the connection to the server
        /// </summary>
        /// <returns></returns>
        public async Task disconnect()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            
            logger.Write($"{TAG}: threadId = {threadId}: disconnect .");
            try
            {
                await hubConnection.StopAsync();
            }
            catch (Exception e) {
                logger.Write($"{TAG}: threadId = {threadId}: disconnect:  Failed...  {e.Message} .");
            }
         }

        /// <summary>
        /// Allows you to get a client token 
        /// </summary>
        /// <returns></returns>
        async Task<string> GetToken()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            var httpClient = new HttpClient();

            var json = new StringContent(JsonConvert.SerializeObject(
                new { Username = "user1", Password = "P@ssword" }),
                Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync($"http{GlobalConstants.IP_SERVER}simpletestsinglrserver/auth", json);

            var authResponse = await result.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse != null)
            {
                if (authResponse.Error == null) 
                {
                    var token = authResponse.Token;
                    return token;
                }
            }
            return "";
        }
    }
}
