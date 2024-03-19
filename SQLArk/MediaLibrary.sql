CREATE TABLE MediaLibrary
(
    Album             NVARCHAR(MAX) NULL,
    Application       NVARCHAR(MAX) NULL,
    Artist            NVARCHAR(MAX) NULL,
    BornOn            NVARCHAR(MAX) NULL,
    DistinguishedName NVARCHAR(MAX) NULL,
    ID                UNIQUEIDENTIFIER PRIMARY KEY NONCLUSTERED,
    Modified          NVARCHAR(MAX) NULL,
    Name              NVARCHAR(MAX) NULL,
    Title             NVARCHAR(MAX) NULL,
    Type              NVARCHAR(MAX) NULL,
    Year              NVARCHAR(MAX) NULL
);