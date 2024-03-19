namespace KusDepot.FabricExams;

internal static class CatalogExamArk
{
    public const String ToolApp = "CatalogExamApp";

    public static readonly Guid ToolID = Guid.NewGuid();

    public static readonly DateTimeOffset ToolMod = DateTimeOffset.Now;

    public const String ToolName = "CatalogExamName";

    public static readonly Version ToolVersion = new Version("1.0.1.0");

    public static Ark GenerateCatalogExamArk()
    {
        Ark _ = new Ark(); Tool _0 = new Tool(); Tool _1 = new Tool();

        _0.SetApplication(ToolApp); _0.SetID(ToolID); _0.SetModified(ToolMod); _0.SetName(ToolName); _0.AddNotes(new HashSet<String>(){"Note01","Note02"}); _0.AddTags(new HashSet<String>(){"Tag03","Tag04"}); _0.SetVersion(ToolVersion);

        if(!_.AddUpdate(_0.ToString())) { throw new Exception(); }  if(!_.AddUpdate(_1.ToString())) { throw new Exception(); }

        BinaryItem _2 = new BinaryItem(type: DataType.CER);                 if(!_.AddUpdate(_2.ToString())) { throw new Exception(); }
        CodeItem _3 = new CodeItem(name: "Deploy-OS" , type: DataType.PS1); if(!_.AddUpdate(_3.ToString())) { throw new Exception(); }
        GenericItem _4 = new GenericItem(type: DataType.JSON);              if(!_.AddUpdate(_4.ToString())) { throw new Exception(); }
        GuidReferenceItem _5 = new GuidReferenceItem();                     if(!_.AddUpdate(_5.ToString())) { throw new Exception(); }
        MSBuildItem _6 = new MSBuildItem(); _6.SetVersion(ToolVersion);     if(!_.AddUpdate(_6.ToString())) { throw new Exception(); }
        MultiMediaItem _7 = new MultiMediaItem(type: DataType.PNG);         if(!_.AddUpdate(_7.ToString())) { throw new Exception(); }
        TextItem _8 = new TextItem(name: "Config1" , type: DataType.XML);   if(!_.AddUpdate(_8.ToString())) { throw new Exception(); }
        TextItem _9 = new TextItem(name: "Config2" , type: DataType.XML);   if(!_.AddUpdate(_9.ToString())) { throw new Exception(); }

        MultiMediaItem _10 = new MultiMediaItem(title: "Cosmos" , artists: new HashSet<String>(){"Sagan"} , year: 1980); if(!_.AddUpdate(_10.ToString())) { throw new Exception(); }
        MultiMediaItem _11 = new MultiMediaItem(title: "Cosmos" , artists: new HashSet<String>(){"Tyson"} , year: 2014); if(!_.AddUpdate(_11.ToString())) { throw new Exception(); }
        MultiMediaItem _12 = new MultiMediaItem(title: "Cosmos" , artists: new HashSet<String>(){"Tyson"} , year: 2020); if(!_.AddUpdate(_12.ToString())) { throw new Exception(); }

        GuidReferenceItem _13 = new GuidReferenceItem(notes: new HashSet<String>(){"Note100","Note200","Note300"}); if(!_.AddUpdate(_13.ToString())) { throw new Exception(); }
        GuidReferenceItem _14 = new GuidReferenceItem(notes: new HashSet<String>(){"Note200","Note300","Note400"}); if(!_.AddUpdate(_14.ToString())) { throw new Exception(); }
        GuidReferenceItem _15 = new GuidReferenceItem(notes: new HashSet<String>(){"Note300","Note500","Note600"}); if(!_.AddUpdate(_15.ToString())) { throw new Exception(); }

        GuidReferenceItem _16 = new GuidReferenceItem(tags: new HashSet<String>(){"Tag100","Tag200","Tag300"}); if(!_.AddUpdate(_16.ToString())) { throw new Exception(); }
        GuidReferenceItem _17 = new GuidReferenceItem(tags: new HashSet<String>(){"Tag200","Tag300","Tag400"}); if(!_.AddUpdate(_17.ToString())) { throw new Exception(); }
        GuidReferenceItem _18 = new GuidReferenceItem(tags: new HashSet<String>(){"Tag300","Tag500","Tag600"}); if(!_.AddUpdate(_18.ToString())) { throw new Exception(); }

        return _;
    }
}