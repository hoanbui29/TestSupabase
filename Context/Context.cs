using Infrastructure.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Supabase;

namespace Context
{
    public class SupabaseClient : ISupabaseClient
    {
        private readonly IOptions<SupabaseSettings> _supabaseSettings;

        public SupabaseClient(IOptions<SupabaseSettings> supabaseSettings)
        {
            _supabaseSettings = supabaseSettings;
        }

        public Client Client
        {
            get
            {
                var client = new Supabase.Client(_supabaseSettings.Value.Url, _supabaseSettings.Value.Key, new SupabaseOptions() { AutoConnectRealtime = true });
                client.InitializeAsync().Wait();
                return client;
            }
        }
    }
}
