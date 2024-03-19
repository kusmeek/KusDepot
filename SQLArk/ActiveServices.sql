CREATE TABLE ActiveServices
(
    ActorID            UNIQUEIDENTIFIER NULL,
    ActorNameID        NVARCHAR(MAX) NULL,
    Application        NVARCHAR(MAX) NULL,
    ApplicationVersion NVARCHAR(MAX) NULL,
    BornOn             NVARCHAR(MAX) NULL,
    DistinguishedName  NVARCHAR(MAX) NULL,
    ID                 UNIQUEIDENTIFIER PRIMARY KEY NONCLUSTERED,
    Interfaces         NVARCHAR(MAX) NULL,
    Modified           NVARCHAR(MAX) NULL,
    Name               NVARCHAR(MAX) NULL,
    Purpose            NVARCHAR(MAX) NULL,
    Registered         NVARCHAR(MAX) NULL,
    ServiceVersion     NVARCHAR(MAX) NULL,
    Url                NVARCHAR(MAX) NULL,
    Version            NVARCHAR(MAX) NULL
);