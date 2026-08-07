namespace KusDepot.Data.Clients;

/**<include file='DataControlClientSessionManager.xml' path='DataControlClientSessionManager/class[@name="DataControlClientSessionManager"]/main/*'/>*/
internal sealed class DataControlClientSessionManager
{
    /**<include file='DataControlClientSessionManager.xml' path='DataControlClientSessionManager/class[@name="DataControlClientSessionManager"]/field[@name="storage"]/*'/>*/
    private readonly DataControlClientWorkingDirectoryStorage storage;

    /**<include file='DataControlClientSessionManager.xml' path='DataControlClientSessionManager/class[@name="DataControlClientSessionManager"]/constructor[@name="Constructor"]/*'/>*/
    public DataControlClientSessionManager(DataControlClientWorkingDirectoryOptions? options = null)
    {
        this.storage = new DataControlClientWorkingDirectoryStorage(options);
    }

    /**<include file='DataControlClientSessionManager.xml' path='DataControlClientSessionManager/class[@name="DataControlClientSessionManager"]/method[@name="GetLocalSession"]/*'/>*/
    public async Task<DataControlTransferSessionInfo?> GetLocalSession(Guid sessionid , CancellationToken cancel = default)
    {
        DataControlTransferSessionInfo? segmented = await this.storage.LoadSessionInfo(sessionid,cancel).ConfigureAwait(false);

        if(segmented is not null) { return segmented; }

        return await this.storage.LoadStreamSessionInfo(sessionid,cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClientSessionManager.xml' path='DataControlClientSessionManager/class[@name="DataControlClientSessionManager"]/method[@name="ListLocalSessions"]/*'/>*/
    public async Task<IReadOnlyList<DataControlTransferSessionInfo>> ListLocalSessions(CancellationToken cancel = default)
    {
        List<DataControlTransferSessionInfo> sessions = new();

        foreach(Guid sessionId in this.storage.EnumerateSegmentedSessionIDs())
        {
            DataControlTransferSessionInfo? session = await this.storage.LoadSessionInfo(sessionId,cancel).ConfigureAwait(false);

            if(session is not null) { sessions.Add(session); }
        }

        foreach(Guid sessionId in this.storage.EnumerateStreamSessionIDs())
        {
            DataControlTransferSessionInfo? session = await this.storage.LoadStreamSessionInfo(sessionId,cancel).ConfigureAwait(false);

            if(session is not null) { sessions.Add(session); }
        }

        return sessions
            .OrderByDescending(session => session.StreamState?.Updated ?? session.State.Updated)
            .ToArray();
    }

    /**<include file='DataControlClientSessionManager.xml' path='DataControlClientSessionManager/class[@name="DataControlClientSessionManager"]/method[@name="DeleteLocalSession"]/*'/>*/
    public async Task<Boolean> DeleteLocalSession(Guid sessionid , CancellationToken cancel = default)
    {
        DataControlTransferSessionInfo? session = await this.GetLocalSession(sessionid,cancel).ConfigureAwait(false);

        if(session is null) { return false; }

        return session.TransferFamily is DataControlTransferFamily.Stream
            ? await this.storage.DeleteStreamSession(sessionid,cancel).ConfigureAwait(false)
            : await this.storage.DeleteSession(sessionid,cancel).ConfigureAwait(false);


    }
}