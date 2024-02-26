using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Context;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Supabase.Realtime;
using Supabase.Realtime.Models;

namespace Icomm
{
    public enum BroadcastEventType
    {
        GetNextJob,
        AssignJob,
    }

    public enum AgentRequestEventType
    {
        Dispose = -1,
        GetNewWorkersRequest,
        GetNewWorkerResponse,
    }


    public class ProxyServerDto
    {
        public long Id { get; set; }

        /// <summary>
        /// Loại Proxy
        /// </summary>
        public string Type { get; set; }

        public string Host { get; set; }

        public int Port { get; set; } = 80;

        /// <summary>
        /// Authen
        /// </summary>
        public string AuthUsername { get; set; }

        /// <summary>
        /// Authen
        /// </summary>
        public string AuthPassword { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// Khả dụng của Proxy
        /// </summary>
        public bool? IsOnline { get; set; }

        /// <summary>
        /// Thời gian lần cuối check
        /// </summary>
        public DateTime? LastCheckTime { get; set; }

        /// <summary>
        /// Message lần cuối check
        /// </summary>
        public string LastCheckMessage { get; set; }

        /// <summary>
        /// Khoảng thời gian hoạt động của proxy cron
        /// </summary>
        public string ActiveTimesCron { get; set; }

        /// <summary>
        /// Quốc gia
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Lấy url kết nối theo Http
        /// </summary>
        /// <returns></returns>
        public string ToUrl()
        {
            return $"http://{this.Host}:{this.Port}";
        }
    }


    public class SocialAccountEntity
    {
        //public Guid Id { get; set; }
        public string AccountId { get; set; }

        private string _UserName;

        public string UserName
        {
            get
            {
                return this._UserName == null ? this.AccountId : _UserName;
            }
            set
            {
                this._UserName = value;
            }
        }

        public string Password { get; set; }
        public string DisplayName { get; set; }
        public int Status { get; set; }
        public string Cookie { get; set; }
        public string Token { get; set; }
        public DateTime? LastUpdateCookie { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Secret2FA { get; set; }
        public string Gender { get; set; }
        public int? ProxyServerId { get; set; }
        public ProxyServerDto proxy { get; set; }
        public string Mail { get; set; }
        public string PassMail { get; set; }
        public string PhoneNumber { get; set; }
        public string Domain { get; set; }
        public int Point { get; set; }
        public string ParentId { get; set; }

        [Obsolete("Sử dụng PlatformAlias")] public int PlatformId { get; set; }

        public string PlatformAlias { get; set; }
        public int InDependent { get; set; } = 0;

        public string AppId { get; set; }

        public long DeviceId { get; set; }

        public string DeviceIpAddress { get; set; }

        public string AutoModes { get; set; }

        public int StatusOnDevice { get; set; }

        public bool IsKeepOpenProfile { get; set; }
    }

    public class ActionScheduleSupabaseDto
    {
        [System.ComponentModel.DataAnnotations.Schema.Column("id")]
        public long Id { get; set; }
        [Column("created_time")]
        public DateTime CreatedTime { get; set; }
        [Column("action")]
        public string Action { get; set; }
        [Column("schedule_time")]
        public DateTime ScheduleTime { get; set; }
        [Column("status")]
        public int Status { get; set; }
        [Column("set_id")]
        public long? SetId { get; set; }
        [Column("extra_data")]
        public string ExtraData { get; set; }
        [Column("action_method")]
        public string ActionMethod { get; set; }
        [Column("priority")]
        public int Priority { get; set; }
        [Column("platform_alias")]
        public string PlatformAlias { get; set; }
        [Column("account_id")]
        public string AccountId { get; set; }
        [Column("device_id")]
        public long? DeviceId { get; set; }
        [Column("action_schedule_group_id")]
        public Guid? ActionScheduleGroupId { get; set; }
        [Column("done_time")]
        public DateTime? DoneTime { get; set; }
        [Column("start_time")]
        public DateTime? StartTime { get; set; }
        [Column("resolver_id")]
        public long? ResolverId { get; set; }
        [Column("referrer_module")]
        public string ReferrerModule { get; set; }
    }


    public class JobDetail : ActionScheduleSupabaseDto
    {
        public string AppId { get; set; }

        public JobDetailActionInfo ActionInfo { get; set; }

        /// <summary>
        /// Nội dung của hành động (nếu có) VD: Nội dung comment, nội dung tin nhắn
        /// Bóc ra đây để phục vụ lọc trùng
        /// </summary>
        public string? ExtraDataContent { get; set; }

        /// <summary>
        /// Trạng thái của tài khoản trên thiết bị
        /// </summary>
        public int? AutoFarmDeviceAccountStatus { get; set; }

        public string? AutoModes { get; set; }

        public bool IsKeepOpenProfile { get; set; }
    }

    public class JobDetailActionInfo
    {
        public const string METHOD_NORMAL = "normal";
        public const string METHOD_PSEUDO = "pseudo";

        public string Action { get; set; }

        /// <summary>
        /// Loại thực thi hành động<br></br>
        /// <list type="bullet">
        /// <item><see cref="METHOD_NORMAL"/></item>
        /// <item><see cref="METHOD_PSEUDO"/></item>
        /// </list>
        /// Default: <see cref="METHOD_NORMAL"/>
        /// </summary>
        public string Method { get; set; } = METHOD_NORMAL;

        /// <summary>
        /// Dữ liệu Pseudo
        /// </summary>
        public JToken? PseudoJson { get; set; }
    }



    public class AgentJobDetail
    {
        /// <summary>
        /// Lịch thực thi
        /// </summary>
        public JobDetail Action { get; set; }

        /// <summary>
        /// Thông tin tài khoản
        /// </summary>
        public SocialAccountEntity? Account { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int? ErrorStatus { get; set; }
    }



    public class AgentChannelBroadcastMessage : BaseBroadcast
    {
        public AgentRequestEventType EventType { get; set; }
        public IEnumerable<string> WorkerChannelIds { get; set; }
    }

    public class BroadcastMessage : BaseBroadcast
    {
        public BroadcastEventType EventType { get; set; }

        public AgentJobDetail Data { get; set; }
    }



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
            var channel = client.Realtime.Channel("ICAVA_agent_channel_10245");
            var broadCast = channel.Register<AgentChannelBroadcastMessage>();
            broadCast.AddBroadcastEventHandler(async (sender, baseBroadcast) =>
            {
                var response = broadCast.Current();
                Log.Information("Received {workers} workers", response.WorkerChannelIds.Count());
                List<RealtimeBroadcast<BroadcastMessage>> workerBroadCasts = new List<RealtimeBroadcast<BroadcastMessage>>();
                foreach (var workerChannelId in response.WorkerChannelIds)
                {
                    Console.WriteLine($"Worker Channel Id: {workerChannelId}");
                    var workerChannel = client.Realtime.Channel(workerChannelId);
                    var workerBroadCast = workerChannel.Register<BroadcastMessage>();
                    workerBroadCast.AddBroadcastEventHandler(async (s, b) =>
                    {
                        var workerResponse = workerBroadCast.Current();
                        Console.WriteLine("Received message id: " + workerResponse.Data.Action.Id + " from worker: " + workerChannelId);
                        Console.WriteLine(JsonConvert.SerializeObject(workerResponse.Data));
                        await workerBroadCast.Send(null, new BroadcastMessage()
                        {
                            EventType = BroadcastEventType.GetNextJob,
                        });

                    });
                    await workerChannel.Subscribe();
                    Log.Information("Subscribe to worker channel");
                    workerBroadCasts.Add(workerBroadCast);
                }

                await Task.WhenAll(workerBroadCasts.Select(s =>
                {
                    Log.Information("Get next job");
                    return s.Send(null, new BroadcastMessage()
                    {
                        EventType = BroadcastEventType.GetNextJob,
                    });
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
