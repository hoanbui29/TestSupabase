using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Context;
using Entities;
using Microsoft.Extensions.Hosting;
using Serilog;
using Workers;

namespace Icomm
{
    public class ServiceMain : BackgroundService
    {
        private readonly ISupabaseClient _supabaseClient;

        public ServiceMain(ISupabaseClient supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = _supabaseClient.Client;
            var channel = client.Realtime.Channel("ICAVA_agent_channel_10295");
            var broadCast = channel.Register<AgentChannelBroadcastMessage>();
            broadCast.AddBroadcastEventHandler(async (sender, baseBroadcast) =>
            {
                var response = broadCast.Current();
                Log.Information("Received {workers} workers", response.WorkerChannelIds.Count());
                List<WorkerHandler> workers = new List<WorkerHandler>();
                foreach (var workerChannelId in response.WorkerChannelIds)
                {
                    Console.WriteLine($"Worker Channel Id: {workerChannelId}");
                    var workerChannel = client.Realtime.Channel(workerChannelId);
                    var workerBroadCast = workerChannel.Register<BroadcastMessage>();
                    var worker = new WorkerHandler(workerBroadCast, client);
                    workerBroadCast.AddBroadcastEventHandler(async (s, b) =>
                    {
                        Log.Information("Received worker response");
                        var workerResponse = workerBroadCast.Current();
                        await worker.ProcessAsync(workerResponse!.Data, stoppingToken);
                    });
                    await workerChannel.Subscribe();
                    Log.Information("Subscribe to worker channel");
                    workers.Add(worker);
                }

                await Task.WhenAll(workers.Select(s =>
                {
                    Log.Information("Get next job");
                    return s.GetNextTaskAsync(stoppingToken);
                }
                    ));

            });
            await channel.Subscribe();
            await broadCast.Send("Get new workers", new AgentChannelBroadcastMessage()
            {
                EventType = AgentRequestEventType.GetNewWorkersRequest,
            });
        }
    }
}
