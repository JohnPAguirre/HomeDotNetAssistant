using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using AutomaticSwitchTimer.ResponseDTO;
using Serilog;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace AutomaticSwitchTimer
{
    public class HomeAssistantConnection
    {
        private CancellationToken cancellationToken;
        private Uri endpoint;
        private ClientWebSocket socket;
        private object lockObject = new object();
        private bool ranSetup = false;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private bool currentlyAuthenticating;
        private bool isAuthenticated;
        private int currentID = 1;
        private BufferBlock<JObject> currentMessageQueue;

        public HomeAssistantConnection(Uri endpoint, CancellationToken cancellationToken = new System.Threading.CancellationToken())
        {
            this.cancellationToken = cancellationToken;
            this.endpoint = endpoint;
            socket = new ClientWebSocket();
            currentlyAuthenticating = false;
            currentMessageQueue = new BufferBlock<JObject>();
        }

        private async Task Setup()
        {
            //should only be ran once
            if (ranSetup)
                return;
            await semaphoreSlim.WaitAsync();
            if (ranSetup)
                return;

            currentlyAuthenticating = true;

            await socket.ConnectAsync(endpoint, cancellationToken);
            var authenticationInitialResult = await ReceiveFromWebSocketToJson();
            if (WhatEventPassedIn.TypeOfObject(authenticationInitialResult) != EventType.InitialAuthenticationResponseNoPasswordNeeded)
                throw new ApplicationException("Could not authenticate for some reason");
            var authentication = authenticationInitialResult.ToObject<InitialAuthenticationDTO>();
            if (authentication.type == "auth_ok")
                isAuthenticated = true;
            else
                throw new NotImplementedException("Work on authentication");
            await Send(new EventDTO()
            {
                id = currentID,
                type = "subscribe_events",
                event_type = "state_changed"
            });
            currentID = currentID + 1;
            //while (socket.State == WebSocketState.Open)
            //{
            //    var b = await Receive(false);
            //    Console.Write(b.ToString());
            //}
            var eh = new Thread(delegate () {
                while (socket.State == WebSocketState.Open)
                {
                    var b = ReceiveFromWebSocketToJson().Result;
                    currentMessageQueue.SendAsync(b);
                    object a = null;
                    //TODO: Deseliazie into a good object if we can
                    if (WhatEventPassedIn.TypeOfObject(b) == EventType.SwitchChangedEvent)
                        Console.WriteLine("STOP");
                    Log.Information("Got the following message: {@b}", b.ToString(Formatting.None));
                    Console.Write(b.ToString());
                }
            });
            eh.Start();
            while (socket.State == WebSocketState.Open)
            {
                await Send(new ServiceCallDTO()
                {
                    id = currentID,
                    domain = "homeassistant",
                    type = "call_service",
                    service = "turn_on",
                    service_data = new ServiceData()
                    {
                        entity_id = "switch.__switch_2_0"
                    }
                });
                currentID = currentID + 1;
                Thread.Sleep(10000);
                await Send(new ServiceCallDTO()
                {
                     id = currentID,
                    domain = "homeassistant",
                    type = "call_service",
                    service = "turn_off",
                    service_data = new ServiceData()
                    {
                        entity_id = "switch.__switch_2_0"
                    }
                });
                currentID = currentID + 1;
                Thread.Sleep(10000);
            }
        }

        public async Task<bool> Authenticate(string ApiPassword = null)
        {
            await Setup();
            return isAuthenticated;
        }
        public async Task Send(object toSend)
        {
            var serializedToSend = JsonConvert.SerializeObject(toSend);
            var buffer = Encoding.Default.GetBytes(serializedToSend);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }

        private async Task<JObject> ReceiveFromWebSocketToJson()
        {
            if (!currentlyAuthenticating)
                await Setup();
            var buffer = new byte[1024];
            var gotSomething = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            if (gotSomething.MessageType == WebSocketMessageType.Close)
                throw new Exception("Was Closed");
            List<byte> entireBuffer = new List<byte>(buffer.Take(gotSomething.Count));
            while (gotSomething.EndOfMessage != true)
            {
                gotSomething = await socket.ReceiveAsync(buffer, cancellationToken);
                entireBuffer.AddRange(buffer.Take(gotSomething.Count));
            }            
            using (var memoryStream = new MemoryStream(entireBuffer.ToArray()))
            using (var streamReader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                return JObject.FromObject(serializer.Deserialize(jsonReader));
            }
        }


    }
}
