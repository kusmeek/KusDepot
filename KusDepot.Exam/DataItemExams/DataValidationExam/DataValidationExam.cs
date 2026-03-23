namespace KusDepot.Exams.DataItems;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class DataValidationExam
{
    [Test]
    public void IsDataTypeChangeAllowed_KnownItems_Expected()
    {
        Assert.That(IsDataTypeChangeAllowed(new MSBuildItem()),Is.False);
        Assert.That(IsDataTypeChangeAllowed(new GuidReferenceItem()),Is.False);
        Assert.That(IsDataTypeChangeAllowed(new KeySet()),Is.False);
        Assert.That(IsDataTypeChangeAllowed(new DataSetItem()),Is.True);
        Assert.That(IsDataTypeChangeAllowed(new GenericItem()),Is.True);
        Assert.That(IsDataTypeChangeAllowed(new BinaryItem()),Is.True);
        Assert.That(IsDataTypeChangeAllowed(new CodeItem()),Is.True);
        Assert.That(IsDataTypeChangeAllowed(new TextItem()),Is.True);
        Assert.That(IsDataTypeChangeAllowed(new MultiMediaItem()),Is.True);
    }

    [Test]
    public void IsValidBinaryDataType_Behavior()
    {
        Assert.That(IsValidBinaryDataType(DataTypes.EXE),Is.True);
        Assert.That(IsValidBinaryDataType(DataTypes.CS),Is.False);
        Assert.That(IsValidBinaryDataType(null),Is.True);
        Assert.That(IsValidBinaryDataType(DataTypes.NONE),Is.True);
    }

    [Test]
    public void IsValidCodeDataType_Behavior()
    {
        Assert.That(IsValidCodeDataType(DataTypes.CS),Is.True);
        Assert.That(IsValidCodeDataType(DataTypes.EXE),Is.False);
        Assert.That(IsValidCodeDataType(null),Is.False);
    }

    [Test]
    public void IsValidTextDataType_Behavior()
    {
        Assert.That(IsValidTextDataType(DataTypes.XML),Is.True);
        Assert.That(IsValidTextDataType(DataTypes.EXE),Is.False);
        Assert.That(IsValidTextDataType(null),Is.False);
    }

    [Test]
    public void IsValidMultiMediaDataType_Behavior()
    {
        Assert.That(IsValidMultiMediaDataType(DataTypes.MP4),Is.True);
        Assert.That(IsValidMultiMediaDataType(DataTypes.XML),Is.False);
        Assert.That(IsValidMultiMediaDataType(null),Is.False);
    }

    [Test]
    public void IsValid_Dispatch_ByItemType()
    {
        var code = new CodeItem();
        Assert.That(IsValid(code,DataTypes.CS),Is.True);
        Assert.That(IsValid(code,DataTypes.EXE),Is.False);

        var text = new TextItem();
        Assert.That(IsValid(text,DataTypes.XML),Is.True);
        Assert.That(IsValid(text,DataTypes.MP3),Is.False);

        var bin = new BinaryItem();
        Assert.That(IsValid(bin,DataTypes.EXE),Is.True);
        Assert.That(IsValid(bin,DataTypes.CS),Is.False);

        var mm = new MultiMediaItem();
        Assert.That(IsValid(mm,DataTypes.MP4),Is.True);
        Assert.That(IsValid(mm,DataTypes.JS),Is.False);

        Assert.That(IsValid(new GuidReferenceItem(),DataTypes.GUID),Is.True);
        Assert.That(IsValid(new MSBuildItem(),DataTypes.MSB),Is.True);
        Assert.That(IsValid(new KeySet(),DataTypes.KEYSET),Is.True);

        Assert.That(IsValid(new DataSetItem(),"anything"),Is.True);
        Assert.That(IsValid(new GenericItem(),"anything"),Is.True);
    }

    private static readonly Random Rng = new();

    private static String PickValid(ImmutableArray<String?> arr)
    {
        for(Int32 i = 0; i < arr.Length; i++)
        {
            var v = arr[i];
            if (v is not null) { return v; }
        }
        return String.Empty;
    }

    private static String PickInvalid(ImmutableArray<String?> target,params ImmutableArray<String?>[] others)
    {
        var targetSet = new HashSet<String>(StringComparer.Ordinal);
        for(Int32 i = 0; i < target.Length; i++)
        {
            var v = target[i];
            if (v is not null) { targetSet.Add(v); }
        }
        var pool = new List<String>();
        for(Int32 o = 0; o < others.Length; o++)
        {
            var arr = others[o];
            for(Int32 i =0; i < arr.Length; i++)
            {
                var v = arr[i];
                if(v is null) { continue; }
                if(!targetSet.Contains(v) && !pool.Contains(v)) { pool.Add(v); }
            }
        }
        if(pool.Count == 0) { throw new Exception(); }
        return pool[Rng.Next(pool.Count)];
    }

    [Test]
    public void TextItem_SetType()
    {
        var item = new TextItem();
        var valid = PickValid(TextItemValidDataTypes);
        var invalid = PickInvalid(
            TextItemValidDataTypes,
            BinaryItemValidDataTypes,
            CodeItemValidDataTypes,
            MultiMediaItemValidDataTypes);
        Assert.That(item.SetDataType(valid),Is.True);
        Assert.That(item.SetDataType(invalid),Is.False);
    }

    [Test]
    public void CodeItem_SetType()
    {
        var item = new CodeItem();
        var valid = PickValid(CodeItemValidDataTypes);
        var invalid = PickInvalid(
            CodeItemValidDataTypes,
            BinaryItemValidDataTypes,
            TextItemValidDataTypes,
            MultiMediaItemValidDataTypes);
        Assert.That(item.SetDataType(valid),Is.True);
        Assert.That(item.SetDataType(invalid),Is.False);
    }

    [Test]
    public void BinaryItem_SetType()
    {
        var item = new BinaryItem();
        var valid = PickValid(BinaryItemValidDataTypes);
        var invalid = PickInvalid(
            BinaryItemValidDataTypes,
            CodeItemValidDataTypes,
            TextItemValidDataTypes,
            MultiMediaItemValidDataTypes);
        Assert.That(item.SetDataType(valid),Is.True);
        Assert.That(item.SetDataType(invalid),Is.False);
    }

    [Test]
    public void MultiMediaItem_SetType()
    {
        var item = new MultiMediaItem();
        var valid = PickValid(MultiMediaItemValidDataTypes);
        var invalid = PickInvalid(
            MultiMediaItemValidDataTypes,
            BinaryItemValidDataTypes,
            CodeItemValidDataTypes,
            TextItemValidDataTypes);
        Assert.That(item.SetDataType(valid),Is.True);
        Assert.That(item.SetDataType(invalid),Is.False);
    }

    [Test]
    public void GenericItem_SetType()
    {
        var item = new GenericItem();
        Assert.That(item.SetDataType("ANY_GENERIC_TYPE"),Is.True);
    }

    [Test]
    public void DataSetItem_SetType()
    {
        var item = new DataSetItem();
        Assert.That(item.SetDataType("ANY_DATASET_TYPE"),Is.True);
    }

    [Test]
    public void GuidReferenceItem_SetType()
    {
        var item = new GuidReferenceItem(System.Guid.NewGuid());
        Assert.That(item.SetDataType(DataTypes.GUID),Is.False);
        Check.That(item.GetDataType()).IsEqualTo(DataTypes.GUID);
    }

    [Test]
    public void MSBuildItem_SetType()
    {
        var item = new MSBuildItem();
        Assert.That(item.SetDataType(DataTypes.MSB),Is.False);
        Check.That(item.GetDataType()).IsEqualTo(DataTypes.MSB);
    }

    [Test]
    public void KeySet_SetType()
    {
        var item = new KeySet();
        Assert.That(item.SetDataType(DataTypes.KEYSET),Is.False);
        Check.That(item.GetDataType()).IsEqualTo(DataTypes.KEYSET);
    }
}