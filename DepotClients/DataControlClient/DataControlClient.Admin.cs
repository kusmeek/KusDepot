namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    ///<inheritdoc/>
    public async Task<DataControlResponse<PublicationRecoveryFootprint>> InspectPublication(InspectPublicationRequest request , String? token = null , CancellationToken cancel = default)
    {
        try { return await SendAsync<InspectPublicationRequest,PublicationRecoveryFootprint>(HttpMethod.Post,"/InspectPublication",request,ResolveToken(token),cancel).ConfigureAwait(false); }

        catch ( Exception _ ) { KusDepotLog.Error(_,InspectPublicationFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<PurgePublicationResponse>> PurgePublication(PurgePublicationRequest request , String? token = null , CancellationToken cancel = default)
    {
        try { return await SendAsync<PurgePublicationRequest,PurgePublicationResponse>(HttpMethod.Post,"/PurgePublication",request,ResolveToken(token),cancel).ConfigureAwait(false); }

        catch ( Exception _ ) { KusDepotLog.Error(_,PurgePublicationFail); throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlResponse<DataControlServerSessionInfo[]>> GetServerSessions(GetTransferSessionsRequest? request = null , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            return await SendAsync<GetTransferSessionsRequest,DataControlServerSessionInfo[]>(HttpMethod.Post,"/GetTransferSessions",request ?? new GetTransferSessionsRequest(),ResolveToken(token),cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetServerSessionsFail); throw; }
    }
}
