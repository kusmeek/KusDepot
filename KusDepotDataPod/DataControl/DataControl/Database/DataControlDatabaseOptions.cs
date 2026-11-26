namespace DataPodServices.DataControl;

public sealed class DataControlDatabaseOptions
{
    private const String DefaultName = "datacontrol";

    public String? ConnectionString { get; set; }

    public String? Name { get; set; }

    public String Address { get; set; } = "127.0.0.1";

    public Int32 Port { get; set; } = 5432;

    public String? PGUSER { get; set; }

    public String? PGPASSWORD { get; set; }

    public String GetEffectiveDatabaseName()
    {
        String? connectionStringName = String.IsNullOrWhiteSpace(this.ConnectionString)
            ? null
            : new NpgsqlConnectionStringBuilder(this.ConnectionString).Database;

        if(String.IsNullOrWhiteSpace(this.Name) is false && String.IsNullOrWhiteSpace(connectionStringName) is false
            && String.Equals(this.Name,connectionStringName,Ordinal) is false)
        {
            throw new InvalidOperationException($"DataControl database Name '{this.Name}' does not match the database '{connectionStringName}' specified by ConnectionString.");
        }

        return String.IsNullOrWhiteSpace(this.Name) is false
            ? this.Name
            : String.IsNullOrWhiteSpace(connectionStringName) is false ? connectionStringName : DefaultName;
    }

    public String BuildConnectionString(String? databaseName = null)
    {
        String effectiveName = this.GetEffectiveDatabaseName();

        NpgsqlConnectionStringBuilder builder;

        if(String.IsNullOrWhiteSpace(this.ConnectionString) is false)
        {
            builder = new NpgsqlConnectionStringBuilder(this.ConnectionString);
        }
        else
        {
            builder = new NpgsqlConnectionStringBuilder
            {
                Host = this.Address,
                Port = this.Port,
                Database = effectiveName,
            };

            if(String.IsNullOrWhiteSpace(this.PGUSER) is false) { builder.Username = this.PGUSER; }
            if(String.IsNullOrWhiteSpace(this.PGPASSWORD) is false) { builder.Password = this.PGPASSWORD; }
        }

        builder.Database = effectiveName;

        if(String.IsNullOrWhiteSpace(databaseName) is false) { builder.Database = databaseName; }

        return builder.ConnectionString;
    }
}
