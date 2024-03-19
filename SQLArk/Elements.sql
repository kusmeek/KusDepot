CREATE TABLE Elements
(
    Application        NVARCHAR(MAX) NULL,
    ApplicationVersion NVARCHAR(MAX) NULL,
    BornOn             NVARCHAR(MAX) NULL,
    DistinguishedName  NVARCHAR(MAX) NULL,
    ID                 UNIQUEIDENTIFIER PRIMARY KEY NONCLUSTERED,
    Modified           NVARCHAR(MAX) NULL,
    Name               NVARCHAR(MAX) NULL,
    ObjectType         NVARCHAR(MAX) NULL,
    ServiceVersion     NVARCHAR(MAX) NULL,
    Type               NVARCHAR(MAX) NULL,
    Version            NVARCHAR(MAX) NULL
);