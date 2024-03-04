using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using Supabase.Realtime;

namespace Workers
{
    public class WorkerHandler
    {
        private readonly RealtimeBroadcast<BroadcastMessage> _broadcast;
        private readonly Supabase.Client _client;

        public WorkerHandler(RealtimeBroadcast<BroadcastMessage> broadcast, Supabase.Client client)
        {
            _broadcast = broadcast;
            _client = client;
        }

        private List<int> _listStatuses = new List<int>()
        {
            2,-1,-700, -282
        };

        public async Task ProcessAsync(AgentJobDetail data, CancellationToken cancellationToken)
        {
            await _client.From<ActionScheduleSupabaseDto>().Where(w => w.Uuid == data.Action.Uuid).Set(x => x.Status, 1).Set(x => x.StartTime, DateTime.Now).Update();
            var r = new Random();
            await Task.Delay(r.Next(5000, 30000));
            await _client.From<ActionScheduleSupabaseDto>().Where(w => w.Uuid == data.Action.Uuid).Set(x => x.Status, _listStatuses[new Random().Next(0, _listStatuses.Count)]).Set(x => x.DoneTime, DateTime.Now).Update();
            await GetNextTaskAsync(cancellationToken);
        }

        public async Task GetNextTaskAsync(CancellationToken cancellationToken)
        {
            await _broadcast.Send(null, new BroadcastMessage()
            {
                EventType = BroadcastEventType.GetNextJob,
            });
        }
    }
}
