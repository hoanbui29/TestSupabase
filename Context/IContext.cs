namespace Context
{
    public interface ISupabaseClient
    {
        Supabase.Client Client { get; }
    }
}
