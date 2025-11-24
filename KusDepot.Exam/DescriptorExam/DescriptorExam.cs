namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class DescriptorExam
{
    [Test]
    public void DescriptorExtensionsIsBinaryItem()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsBinaryItem()).IsFalse();
        Check.That(new TextItem().GetDescriptor()!.IsBinaryItem()).IsFalse();
        Check.That(new CodeItem().GetDescriptor()!.IsBinaryItem()).IsFalse();
        Check.That(new BinaryItem().GetDescriptor()!.IsBinaryItem()).IsTrue();
        Check.That(new MultiMediaItem().GetDescriptor()!.IsBinaryItem()).IsFalse();
        Check.That(new MSBuildItem().GetDescriptor()!.IsBinaryItem()).IsFalse();
        Check.That(new GenericItem().GetDescriptor()!.IsBinaryItem()).IsFalse();
    }

    [Test]
    public void DescriptorExtensionsIsCodeItem()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsCodeItem()).IsFalse();
        Check.That(new TextItem().GetDescriptor()!.IsCodeItem()).IsFalse();
        Check.That(new CodeItem().GetDescriptor()!.IsCodeItem()).IsTrue();
        Check.That(new BinaryItem().GetDescriptor()!.IsCodeItem()).IsFalse();
        Check.That(new MultiMediaItem().GetDescriptor()!.IsCodeItem()).IsFalse();
        Check.That(new MSBuildItem().GetDescriptor()!.IsCodeItem()).IsFalse();
        Check.That(new GenericItem().GetDescriptor()!.IsCodeItem()).IsFalse();
    }

    [Test]
    public void DescriptorExtensionsIsCommand()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsCommand()).IsFalse();
        Check.That(new BinaryItem().GetDescriptor()!.IsCommand()).IsFalse();
        var b = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin"));
        Check.That(b!.GetDescriptor()!.IsCommand()).IsTrue();;
        Check.That(b.GetDescriptor()!.IsService()).IsTrue();;
    }

    [Test]
    public void DescriptorExtensionsIsDataItem()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsDataItem()).IsTrue();
        Check.That(new TextItem().GetDescriptor()!.IsDataItem()).IsTrue();
        Check.That(new CodeItem().GetDescriptor()!.IsDataItem()).IsTrue();
        Check.That(new BinaryItem().GetDescriptor()!.IsDataItem()).IsTrue();
        Check.That(new MultiMediaItem().GetDescriptor()!.IsDataItem()).IsTrue();
        Check.That(new MSBuildItem().GetDescriptor()!.IsDataItem()).IsTrue();
        Check.That(new GenericItem().GetDescriptor()!.IsDataItem()).IsTrue();
    }

    [Test]
    public void DescriptorExtensionsIsGenericItem()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsGenericItem()).IsFalse();
        Check.That(new TextItem().GetDescriptor()!.IsGenericItem()).IsFalse();
        Check.That(new CodeItem().GetDescriptor()!.IsGenericItem()).IsFalse();
        Check.That(new BinaryItem().GetDescriptor()!.IsGenericItem()).IsFalse();
        Check.That(new MultiMediaItem().GetDescriptor()!.IsGenericItem()).IsFalse();
        Check.That(new MSBuildItem().GetDescriptor()!.IsGenericItem()).IsFalse();
        Check.That(new GenericItem().GetDescriptor()!.IsGenericItem()).IsTrue();
    }

    [Test]
    public void DescriptorExtensionsIsGuidReferenceItem()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsGuidReferenceItem()).IsTrue();
        Check.That(new TextItem().GetDescriptor()!.IsGuidReferenceItem()).IsFalse();
        Check.That(new CodeItem().GetDescriptor()!.IsGuidReferenceItem()).IsFalse();
        Check.That(new BinaryItem().GetDescriptor()!.IsGuidReferenceItem()).IsFalse();
        Check.That(new MultiMediaItem().GetDescriptor()!.IsGuidReferenceItem()).IsFalse();
        Check.That(new MSBuildItem().GetDescriptor()!.IsGuidReferenceItem()).IsFalse();
        Check.That(new GenericItem().GetDescriptor()!.IsGuidReferenceItem()).IsFalse();
    }

    [Test]
    public void DescriptorExtensionsIsMSBuildItem()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsMSBuildItem()).IsFalse();
        Check.That(new TextItem().GetDescriptor()!.IsMSBuildItem()).IsFalse();
        Check.That(new CodeItem().GetDescriptor()!.IsMSBuildItem()).IsFalse();
        Check.That(new BinaryItem().GetDescriptor()!.IsMSBuildItem()).IsFalse();
        Check.That(new MultiMediaItem().GetDescriptor()!.IsMSBuildItem()).IsFalse();
        Check.That(new MSBuildItem().GetDescriptor()!.IsMSBuildItem()).IsTrue();
        Check.That(new GenericItem().GetDescriptor()!.IsMSBuildItem()).IsFalse();
    }

    [Test]
    public void DescriptorExtensionsIsMultiMediaItem()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsMultiMediaItem()).IsFalse();
        Check.That(new TextItem().GetDescriptor()!.IsMultiMediaItem()).IsFalse();
        Check.That(new CodeItem().GetDescriptor()!.IsMultiMediaItem()).IsFalse();
        Check.That(new BinaryItem().GetDescriptor()!.IsMultiMediaItem()).IsFalse();
        Check.That(new MultiMediaItem().GetDescriptor()!.IsMultiMediaItem()).IsTrue();
        Check.That(new MSBuildItem().GetDescriptor()!.IsMultiMediaItem()).IsFalse();
        Check.That(new GenericItem().GetDescriptor()!.IsMultiMediaItem()).IsFalse();
    }

    [Test]
    public void DescriptorExtensionsIsTextItem()
    {
        Check.That(new GuidReferenceItem().GetDescriptor()!.IsTextItem()).IsFalse();
        Check.That(new TextItem().GetDescriptor()!.IsTextItem()).IsTrue();
        Check.That(new CodeItem().GetDescriptor()!.IsTextItem()).IsFalse();
        Check.That(new BinaryItem().GetDescriptor()!.IsTextItem()).IsFalse();
        Check.That(new MultiMediaItem().GetDescriptor()!.IsTextItem()).IsFalse();
        Check.That(new MSBuildItem().GetDescriptor()!.IsTextItem()).IsFalse();
        Check.That(new GenericItem().GetDescriptor()!.IsTextItem()).IsFalse();
    }
}