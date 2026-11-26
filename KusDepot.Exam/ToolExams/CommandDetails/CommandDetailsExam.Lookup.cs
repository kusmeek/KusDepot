namespace KusDepot.Exams.Tools;

public partial class CommandDetailsExam
{
    [Test]
    public void GetArgument_ReturnsStoredNullValue()
    {
        CommandDetails details = new(arguments: new Dictionary<String,Object?> { ["NullValue"] = null }, handle: "H", id: Guid.NewGuid());

        Check.That(details.GetArgument("NullValue")).IsNull();
        Check.That(details.GetArgument<String?>("NullValue")).IsNull();
    }

    [Test]
    public void SetArgument_DoesNotOverwriteExistingValue_UnlessRequested()
    {
        CommandDetails details = new(arguments: new Dictionary<String,Object?> { ["Value"] = "Original" }, handle: "H", id: Guid.NewGuid());

        details.SetArgument("Value", "Ignored");
        Check.That(details.GetArgument<String?>("Value")).IsEqualTo("Original");

        details.SetArgument("Value", "Updated", overwrite: true);
        Check.That(details.GetArgument<String?>("Value")).IsEqualTo("Updated");
    }
}
