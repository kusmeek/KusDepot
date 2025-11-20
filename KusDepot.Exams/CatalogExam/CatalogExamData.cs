namespace KusDepot.FabricExams.Data;

public class CatalogExamData
{
    public static IEnumerable<Descriptor> Build()
    {
        var list = new List<Descriptor>();

        var d01 = new BinaryItem(type: DataType.CER);                        list.Add(d01.GetDescriptor()!);
        var d02 = new CodeItem(name: "Deploy-OS" , type: DataType.PS1);      list.Add(d02.GetDescriptor()!);
        var d03 = new GenericItem(type: DataType.JSON);                      list.Add(d03.GetDescriptor()!);
        var d04 = new GuidReferenceItem();                                   list.Add(d04.GetDescriptor()!);
        var d05 = new MSBuildItem(); d05.SetVersion(new Version("1.0.1.0")); list.Add(d05.GetDescriptor()!);
        var d06 = new MultiMediaItem(type: DataType.PNG);                    list.Add(d06.GetDescriptor()!);
        var d07 = new TextItem(name: "Config1" , type: DataType.XML);        list.Add(d07.GetDescriptor()!);
        var d08 = new TextItem(name: "Config2" , type: DataType.XML);        list.Add(d08.GetDescriptor()!);
        var d09 = new KeySet();                                              list.Add(d09.GetDescriptor()!);

        var d10 = new MultiMediaItem(title: "Cosmos" , artists: new HashSet<String>(){"Sagan"} , year:1980); list.Add(d10.GetDescriptor()!);
        var d11 = new MultiMediaItem(title: "Cosmos" , artists: new HashSet<String>(){"Tyson"} , year:2014); list.Add(d11.GetDescriptor()!);
        var d12 = new MultiMediaItem(title: "Cosmos" , artists: new HashSet<String>(){"Tyson"} , year:2020); list.Add(d12.GetDescriptor()!);

        var d13 = new GuidReferenceItem(notes: new HashSet<String>(){"Note100","Note200","Note300"}); list.Add(d13.GetDescriptor()!);
        var d14 = new GuidReferenceItem(notes: new HashSet<String>(){"Note200","Note300","Note400"}); list.Add(d14.GetDescriptor()!);
        var d15 = new GuidReferenceItem(notes: new HashSet<String>(){"Note300","Note500","Note600"}); list.Add(d15.GetDescriptor()!);

        var d16 = new GuidReferenceItem(tags: new HashSet<String>(){"Tag100","Tag200","Tag300"}); list.Add(d16.GetDescriptor()!);
        var d17 = new GuidReferenceItem(tags: new HashSet<String>(){"Tag200","Tag300","Tag400"}); list.Add(d17.GetDescriptor()!);
        var d18 = new GuidReferenceItem(tags: new HashSet<String>(){"Tag300","Tag500","Tag600"}); list.Add(d18.GetDescriptor()!);

        var d19 = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin"));
        if(d19 is null) { throw new Exception(); } else { list.Add(d19.GetDescriptor()!); }

        var d20 = new BinaryItem(); d20.SetFILE(Assembly.GetExecutingAssembly().Location); d20.SetContentStreamed(true);
        if(d20.GetContentStreamed() is not true) { throw new Exception(); }
        list.Add(d20.GetDescriptor()!);

        return list;
    }
}
