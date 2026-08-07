namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="DisposeCore"]/*'/>*/
    private void DisposeCore()
    {
        this.Auth.SetBearerToken("null");

        try
        {
            this.Client.Dispose();
        }
        finally
        {
            if(this.NotificationClient is IAsyncDisposable asyncDisposable)
            {
                asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
        }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="DisposeAsyncCore"]/*'/>*/
    private async ValueTask DisposeAsyncCore()
    {
        this.Auth.SetBearerToken("null");

        try
        {
            this.Client.Dispose();
        }
        finally
        {
            if(this.NotificationClient is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        this.DisposeCore();

        GC.SuppressFinalize(this);
    }

    ///<inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        await this.DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }
}