CREATE PROCEDURE CreateNotesTable @capacity INT

AS

BEGIN

    DECLARE @i INT = 0;

    DECLARE @sql NVARCHAR(MAX) = N'CREATE TABLE Notes (ID UNIQUEIDENTIFIER PRIMARY KEY NONCLUSTERED';

    WHILE @i < @capacity

        BEGIN

            SET @sql += N', Note' + CAST(@i AS NVARCHAR) + N' NVARCHAR(MAX) NULL';

            SET @i += 1;

        END

    SET @sql += N');';

    EXEC (@sql);

END;