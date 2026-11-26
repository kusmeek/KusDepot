using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KusDepotDataPod.DataControl.DataControl.Database.Migrations;

/// <inheritdoc />
public partial class _20261124233607_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DeletionJobs",
            columns: table => new
            {
                CatalogName = table.Column<string>(type: "text", nullable: false),
                ItemID = table.Column<Guid>(type: "uuid", nullable: false),
                ItemInstanceToken = table.Column<Guid>(type: "uuid", nullable: false),
                State = table.Column<int>(type: "integer", nullable: false),
                LastError = table.Column<string>(type: "text", nullable: true),
                AttemptCount = table.Column<int>(type: "integer", nullable: false),
                LastAttempt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                NextAttempt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeletionJobs", x => new { x.CatalogName, x.ItemID, x.ItemInstanceToken });
                table.CheckConstraint("CK_DeletionJobs_AttemptCount_NonNegative", "\"AttemptCount\" >= 0");
            });

        migrationBuilder.CreateTable(
            name: "PublicationJournals",
            columns: table => new
            {
                SessionID = table.Column<Guid>(type: "uuid", nullable: false),
                ItemID = table.Column<Guid>(type: "uuid", nullable: false),
                ItemInstanceToken = table.Column<Guid>(type: "uuid", nullable: false),
                OperationID = table.Column<Guid>(type: "uuid", nullable: false),
                Family = table.Column<int>(type: "integer", nullable: false),
                Phase = table.Column<int>(type: "integer", nullable: false),
                ExpectedStateVersion = table.Column<long>(type: "bigint", nullable: false),
                CatalogName = table.Column<string>(type: "text", nullable: false),
                FailureCount = table.Column<int>(type: "integer", nullable: false),
                LastError = table.Column<string>(type: "text", nullable: true),
                FinalObjectBlobName = table.Column<string>(type: "text", nullable: true),
                FinalObjectLength = table.Column<long>(type: "bigint", nullable: true),
                FinalObjectSHA512 = table.Column<byte[]>(type: "bytea", nullable: true),
                FinalStreamLength = table.Column<long>(type: "bigint", nullable: true),
                IntegrityAlgorithm = table.Column<string>(type: "text", nullable: true),
                IntegrityDigest = table.Column<byte[]>(type: "bytea", nullable: true),
                IntegrityLength = table.Column<long>(type: "bigint", nullable: true),
                IntegritySegmentCount = table.Column<long>(type: "bigint", nullable: true),
                Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                JournalJson = table.Column<string>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PublicationJournals", x => x.SessionID);
            });

        migrationBuilder.CreateTable(
            name: "TransferSessions",
            columns: table => new
            {
                SessionID = table.Column<Guid>(type: "uuid", nullable: false),
                ItemID = table.Column<Guid>(type: "uuid", nullable: false),
                ItemInstanceToken = table.Column<Guid>(type: "uuid", nullable: false),
                CatalogName = table.Column<string>(type: "text", nullable: false),
                SourceSessionID = table.Column<Guid>(type: "uuid", nullable: true),
                Family = table.Column<int>(type: "integer", nullable: false),
                Mode = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                StateVersion = table.Column<long>(type: "bigint", nullable: false),
                PlannedBlockCount = table.Column<long>(type: "bigint", nullable: false),
                CoveredBytes = table.Column<long>(type: "bigint", nullable: false),
                Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                LastActivity = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ManifestJson = table.Column<string>(type: "jsonb", nullable: false),
                ObjectBlobName = table.Column<string>(type: "text", nullable: true),
                ObjectLength = table.Column<long>(type: "bigint", nullable: true),
                ObjectSHA512 = table.Column<byte[]>(type: "bytea", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TransferSessions", x => x.SessionID);
            });

        migrationBuilder.CreateTable(
            name: "SegmentedCoverage",
            columns: table => new
            {
                SessionID = table.Column<Guid>(type: "uuid", nullable: false),
                Offset = table.Column<long>(type: "bigint", nullable: false),
                Length = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SegmentedCoverage", x => new { x.SessionID, x.Offset });
                table.CheckConstraint("CK_SegmentedCoverage_ValidRange", "\"Offset\" >= 0 AND \"Length\" > 0 AND \"Offset\" <= 9223372036854775807 - \"Length\"");
                table.ForeignKey(
                    name: "FK_SegmentedCoverage_TransferSessions_SessionID",
                    column: x => x.SessionID,
                    principalTable: "TransferSessions",
                    principalColumn: "SessionID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SegmentedRanges",
            columns: table => new
            {
                SessionID = table.Column<Guid>(type: "uuid", nullable: false),
                Offset = table.Column<long>(type: "bigint", nullable: false),
                Length = table.Column<long>(type: "bigint", nullable: false),
                BlobName = table.Column<string>(type: "text", nullable: false),
                SHA512 = table.Column<byte[]>(type: "bytea", nullable: false),
                Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Ordinal = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SegmentedRanges", x => new { x.SessionID, x.Offset });
                table.ForeignKey(
                    name: "FK_SegmentedRanges_TransferSessions_SessionID",
                    column: x => x.SessionID,
                    principalTable: "TransferSessions",
                    principalColumn: "SessionID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "StreamChunks",
            columns: table => new
            {
                SessionID = table.Column<Guid>(type: "uuid", nullable: false),
                Offset = table.Column<long>(type: "bigint", nullable: false),
                OperationID = table.Column<Guid>(type: "uuid", nullable: false),
                Length = table.Column<long>(type: "bigint", nullable: false),
                BlobName = table.Column<string>(type: "text", nullable: false),
                SHA512 = table.Column<byte[]>(type: "bytea", nullable: false),
                Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Ordinal = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StreamChunks", x => new { x.SessionID, x.Offset });
                table.ForeignKey(
                    name: "FK_StreamChunks_TransferSessions_SessionID",
                    column: x => x.SessionID,
                    principalTable: "TransferSessions",
                    principalColumn: "SessionID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DeletionJobs_State_NextAttempt_Updated",
            table: "DeletionJobs",
            columns: new[] { "State", "NextAttempt", "Updated" });

        migrationBuilder.CreateIndex(
            name: "IX_PublicationJournals_OperationID",
            table: "PublicationJournals",
            column: "OperationID",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PublicationJournals_Phase_Updated",
            table: "PublicationJournals",
            columns: new[] { "Phase", "Updated" });

        migrationBuilder.CreateIndex(
            name: "IX_SegmentedRanges_SessionID_Offset_Length",
            table: "SegmentedRanges",
            columns: new[] { "SessionID", "Offset", "Length" });

        migrationBuilder.CreateIndex(
            name: "IX_SegmentedRanges_SessionID_Ordinal",
            table: "SegmentedRanges",
            columns: new[] { "SessionID", "Ordinal" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_StreamChunks_OperationID",
            table: "StreamChunks",
            column: "OperationID",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_StreamChunks_SessionID_Ordinal",
            table: "StreamChunks",
            columns: new[] { "SessionID", "Ordinal" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TransferSessions_CatalogName_LastActivity_SessionID",
            table: "TransferSessions",
            columns: new[] { "CatalogName", "LastActivity", "SessionID" });

        migrationBuilder.CreateIndex(
            name: "IX_TransferSessions_ItemID_Status",
            table: "TransferSessions",
            columns: new[] { "ItemID", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_TransferSessions_SourceSessionID_CatalogName",
            table: "TransferSessions",
            columns: new[] { "SourceSessionID", "CatalogName" });

        migrationBuilder.CreateIndex(
            name: "IX_TransferSessions_Updated",
            table: "TransferSessions",
            column: "Updated");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DeletionJobs");

        migrationBuilder.DropTable(
            name: "PublicationJournals");

        migrationBuilder.DropTable(
            name: "SegmentedCoverage");

        migrationBuilder.DropTable(
            name: "SegmentedRanges");

        migrationBuilder.DropTable(
            name: "StreamChunks");

        migrationBuilder.DropTable(
            name: "TransferSessions");
    }
}
