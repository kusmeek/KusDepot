namespace KusDepot.Data;

internal sealed partial class CoreCache
{
    private static class RedisAccess
    {
        private static IDatabase? db;

        private static ConnectionMultiplexer? mux;

        private static String? LastConnectionString;

        private static readonly SemaphoreSlim Sync = new(1,1);

        private static Func<String?>? ConnectionStringProvider;

        private static Action<ConfigurationOptions>? ConfigureOptions;

        private static readonly TimeSpan SyncTime = TimeSpan.FromSeconds(30);

        public static async Task CloseAsync()
        {
            await Sync.WaitAsync(SyncTime);

            try
            {
                db = null; LastConnectionString = null;

                try { if(mux is not null) { await mux.CloseAsync(false); await mux.DisposeAsync(); } }

                catch ( Exception _ ) { Log.Error(_,RedisCloseFail); }
            }
            finally { Sync.Release(); }
        }

        public static async Task<Boolean> ConnectAsync()
        {
            try
            {
                var cs = ConnectionStringProvider?.Invoke();

                if(String.IsNullOrWhiteSpace(cs))
                {
                    Log.Error(RedisConnectFail); return false;
                }

                if(String.Equals(LastConnectionString,cs,Ordinal) &&
                   db is not null && mux is { IsConnected: true })
                { return true; }

                await Sync.WaitAsync(SyncTime);

                try
                {
                    if(String.Equals(LastConnectionString,cs,Ordinal) &&
                       db is not null && mux is { IsConnected: true })
                    { return true; }

                    Log.Information(RedisConnectStart);

                    try { db = null; if(mux is not null) { await mux.CloseAsync(false); await mux.DisposeAsync(); } } catch {}

                    mux = await ConnectionMultiplexer.ConnectAsync(BuildOptions(cs));

                    db = mux.GetDatabase(); LastConnectionString = cs;

                    Log.Information(RedisConnectSuccess);

                    return true;
                }
                finally { Sync.Release(); }
            }
            catch ( Exception _ ) { Log.Error(_,RedisConnectFail); db = null; mux = null; return false; }
        }

        private static ConfigurationOptions BuildOptions(String cs)
        {
            var opt = ConfigurationOptions.Parse(cs);

            opt.AbortOnConnectFail = false;
            opt.ConnectRetry = Math.Max(opt.ConnectRetry,4);
            opt.ReconnectRetryPolicy ??= new ExponentialRetry(1000);
            opt.KeepAlive = opt.KeepAlive == 0 ? 300 : opt.KeepAlive;

            ConfigureOptions?.Invoke(opt);

            return opt;
        }

        public static async Task<IDatabase> GetDatabaseAsync()
        {
            if(await ConnectAsync() is false) { throw new OperationFailedException(RedisGetDBFail); } return db!;
        }

        public static Boolean Initialize(Func<String?> connectionstringprovider , Action<ConfigurationOptions>? configureoptions = null)
        {
            if(connectionstringprovider is null) { return false; }

            ConnectionStringProvider = connectionstringprovider; ConfigureOptions = configureoptions;

            return true;
        }
    }
}