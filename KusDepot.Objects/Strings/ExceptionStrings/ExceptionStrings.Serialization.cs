namespace KusDepot.Strings;

public static partial class ExceptionStrings
{
    #pragma warning disable CS1591

    public const String DeserializeFail    = @"Deserialize Failed";
    public const String FromFileFail       = @"FromFile Failed";
    public const String ParseFail          = @"Parse Failed";
    public const String SerializeFail      = @"Serialize Failed";
    public const String ToFileFail         = @"ToFile Failed [{@ObjectType}] [{@ObjectID}]";
    public const String ToStringFail       = @"ToString Failed [{@ObjectType}] [{@ObjectID}]";
    public const String TryDeserializeFail = @"TryDeserialize Failed";
    public const String TryParseFail       = @"TryParse Failed";
    public const String WriteIndentedFail  = @"WriteIndented Failed [{@ObjectType}] ";
}