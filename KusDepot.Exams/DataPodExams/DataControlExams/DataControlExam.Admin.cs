namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test]
    public async Task PublicationRecoveryAdminInspectsAndPurgesStagingOnly()
    {
        using DataControlClient client = new(this.EndpointLocator!);

        OpenUploadTransferRequest open = CreateOpenUploadRequest(streamLength:32);
        DataControlResponse<OpenUploadTransferResponse> opened = await client.OpenUploadTransfer(open,this.WriteToken);

        Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlResponse<PublicationRecoveryFootprint> denied = await client.InspectPublication(new InspectPublicationRequest { SessionID = open.SessionID });
        Check.That((Int32)denied.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        DataControlResponse<PublicationRecoveryFootprint> inspected = await client.InspectPublication(new InspectPublicationRequest { SessionID = open.SessionID },this.AdminToken);
        Check.That((Int32)inspected.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(inspected.Data).IsNotNull();
        Check.That(inspected.Data!.ItemID).IsEqualTo(open.ItemID);
        Check.That(inspected.Data.SessionStateVersion).IsEqualTo(0);
        Check.That(inspected.Data.ExternalArtifactExistenceChecked).IsFalse();
        Check.That(inspected.Data.StagingArtifacts.Any(item => item.Kind == PublicationRecoveryArtifactKinds.StagedObject && item.Length == open.ObjectPayload.LongLength)).IsTrue();
        Check.That(inspected.Data.FinalArtifacts.Any(item => item.Kind == PublicationRecoveryArtifactKinds.FinalObjectCandidate && item.BlobName.StartsWith("instances/",StringComparison.Ordinal) && item.BlobName.EndsWith("/published/object",StringComparison.Ordinal))).IsTrue();

        PurgePublicationRequest purge = new()
        {
            SessionID = open.SessionID,
            ItemID = open.ItemID,
            CatalogName = inspected.Data.CatalogName,
            ExpectedSessionStateVersion = inspected.Data.SessionStateVersion,
            ConfirmPurge = true,
        };

        DataControlResponse<PurgePublicationResponse> stale = await client.PurgePublication(purge with { ExpectedSessionStateVersion = purge.ExpectedSessionStateVersion + 1 },this.AdminToken);
        Check.That((Int32)stale.StatusCode).IsEqualTo(StatusCodes.Status409Conflict);

        DataControlResponse<PurgePublicationResponse> purged = await client.PurgePublication(purge,this.AdminToken);
        Check.That((Int32)purged.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        Check.That(purged.Data).IsNotNull();
        Check.That(purged.Data!.Purged).IsTrue();
        Check.That(purged.Data.StagingArtifactsDeleted).IsTrue();
        Check.That(purged.Data.DatabaseStateDeleted).IsTrue();
        Check.That(purged.Data.FinalArtifactsDeleted).IsFalse();
        Check.That(purged.Data.CatalogDataDeleted).IsFalse();

        DataControlResponse<PublicationRecoveryFootprint> missing = await client.InspectPublication(new InspectPublicationRequest { SessionID = open.SessionID },this.AdminToken);
        Check.That((Int32)missing.StatusCode).IsEqualTo(StatusCodes.Status404NotFound);
    }
}
