namespace KusDepot.Exams.Tools;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public partial class ToolDataConverterExam
{
    [SetUp]
    public void ResetConverters()
    {
        ToolValueConverter.ResetConverterRegistrations();
    }

    [Test]
    public void ToolValueConverter_GetConverterRegistrations_Defaults_AreOrdered()
    {
        ToolValueConverterRegistration[] registrations = ToolValueConverter.GetConverterRegistrations();

        Check.That(registrations).IsNotNull();
        Check.That(registrations.Length).IsEqualTo(7);
        Check.That(registrations.Select(_ => _.Order).ToArray()).IsEqualTo(new[] { 0 , 2000 , 4000 , 6000 , 8000 , 10_000 , Int32.MaxValue });
        Check.That(registrations.Select(_ => _.Name).ToArray()).IsEqualTo(new[]
        {
            nameof(ToolValueParseConverter),
            nameof(ToolValueConfigurationConverter),
            nameof(ToolValueToolDataConverter),
            nameof(ToolValueGenericDataContractConverter),
            nameof(ToolValueGenericJsonConverter),
            nameof(ToolValueBuildConverter),
            nameof(ToolValueUnhandledWriteConverter)
        });
    }

    [Test]
    public void ToolValueConverter_TryAddConverterRegistration_DuplicateOrder_Fails()
    {
        Boolean ok = ToolValueConverter.TryAddConverterRegistration(new ToolValueConverterRegistration()
        {
            Name = "DuplicateOrder",
            Order = 6000,
            Converter = new DummyToolValueConverter()
        });

        Check.That(ok).IsFalse();
    }

    [Test]
    public void ToolValueConverter_TrySetConverterRegistration_ReordersPipeline()
    {
        Boolean ok = ToolValueConverter.TrySetConverterRegistration(new ToolValueConverterRegistration()
        {
            Name = nameof(ToolValueUnhandledWriteConverter),
            Order = 10,
            Converter = new ToolValueUnhandledWriteConverter()
        });

        Check.That(ok).IsTrue();

        ToolValueConverterRegistration[] registrations = ToolValueConverter.GetConverterRegistrations();

        Check.That(registrations.Select(_ => _.Name).ToArray()).IsEqualTo(new[]
        {
            nameof(ToolValueParseConverter),
            nameof(ToolValueUnhandledWriteConverter),
            nameof(ToolValueConfigurationConverter),
            nameof(ToolValueToolDataConverter),
            nameof(ToolValueGenericDataContractConverter),
            nameof(ToolValueGenericJsonConverter),
            nameof(ToolValueBuildConverter)
        });
    }

    [Test]
    public void ToolValueConverter_TryRemoveConverterRegistration_RemovesByName()
    {
        Boolean ok = ToolValueConverter.TryRemoveConverterRegistration(nameof(ToolValueToolDataConverter));

        Check.That(ok).IsTrue();

        ToolValueConverterRegistration[] registrations = ToolValueConverter.GetConverterRegistrations();

        Check.That(registrations.Any(_ => String.Equals(_.Name,nameof(ToolValueToolDataConverter),StringComparison.Ordinal))).IsFalse();
        Check.That(registrations.Length).IsEqualTo(6);
    }

    [Test]
    public void ToolData_MyData_Setter_ParseMode_Version_Roundtrip()
    {
        ToolData data = new()
        {
            MyData = new ToolValue()
            {
                Mode = ToolValueMode.Parse,
                Type = typeof(Version).FullName,
                Data = "1.2.3.4"
            }
        };

        Check.That(data.Data).IsInstanceOf<Version>();
        Check.That(((Version)data.Data!).ToString()).IsEqualTo("1.2.3.4");

        ToolValue? back = data.MyData;
        Check.That(back).IsNotNull();
        Check.That(back!.Mode).IsEqualTo(ToolValueMode.Parse);
        Check.That(back.Type).IsEqualTo(typeof(Version).FullName);
        Check.That(back.Data).IsEqualTo("1.2.3.4");
    }

    [Test]
    public void ToolData_MyData_Setter_ParseMode_String_Roundtrip()
    {
        ToolData data = new()
        {
            MyData = new ToolValue()
            {
                Mode = ToolValueMode.Parse,
                Type = typeof(String).FullName,
                Data = "payload"
            }
        };

        Check.That(data.Data).IsInstanceOf<String>();
        Check.That((String)data.Data!).IsEqualTo("payload");

        ToolValue? back = data.MyData;
        Check.That(back).IsNotNull();
        Check.That(back!.Mode).IsEqualTo(ToolValueMode.Parse);
        Check.That(back.Type).IsEqualTo(typeof(String).FullName);
        Check.That(back.Data).IsEqualTo("payload");
    }

    [Test]
    public void ToolData_MyData_Setter_ExplicitBuildPayload_Uri_ReadsAndWritesAsCustomJson()
    {
        ToolData data = new()
        {
            MyData = new ToolValue()
            {
                Mode = ToolValueMode.Build,
                Type = typeof(Uri).FullName,
                Arguments =
                [
                    new ToolValueArgument()
                    {
                        Index = 0,
                        Mode = ToolValueMode.Parse,
                        Type = typeof(String).FullName,
                        Data = "https://example.org/"
                    }
                ]
            }
        };

        Check.That(data.Data).IsInstanceOf<Uri>();
        Check.That(data.Data!.ToString()).IsEqualTo("https://example.org/");

        ToolValue? back = data.MyData;
        Check.That(back).IsNotNull();
        Check.That(back!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(back.Type).IsEqualTo(typeof(Uri).FullName);
        Check.That(back.Data).IsEqualTo("\"https://example.org/\"");
        Check.That(back.Arguments).IsNull();
    }

    [Test]
    public void ToolData_MyData_Setter_ExplicitNestedBuildPayload_UriBuilder_ReadsAndWritesAsCustomJson()
    {
        ToolData data = new()
        {
            MyData = new ToolValue()
            {
                Mode = ToolValueMode.Build,
                Type = typeof(UriBuilder).FullName,
                Arguments =
                [
                    new ToolValueArgument()
                    {
                        Index = 0,
                        Mode = ToolValueMode.Build,
                        Type = typeof(Uri).FullName,
                        Parameters =
                        [
                            new ToolValueArgumentParameter()
                            {
                                Index = 0,
                                Type = typeof(String).FullName,
                                Data = "https://example.org/path"
                            }
                        ]
                    }
                ]
            }
        };

        Check.That(data.Data).IsInstanceOf<UriBuilder>();
        Check.That(data.Data!.ToString()).IsEqualTo("https://example.org:443/path");

        ToolValue? back = data.MyData;
        Check.That(back).IsNotNull();
        Check.That(back!.Mode).IsEqualTo(ToolValueMode.Custom);
        Check.That(back.Type).IsEqualTo(typeof(UriBuilder).FullName);
        Check.That(String.IsNullOrWhiteSpace(back.Data)).IsFalse();
        Check.That(back.Arguments).IsNull();
    }

    [Test]
    public void ToolData_MyData_Getter_UnhandledMode_SystemObject()
    {
        ToolData data = new() { Data = new Object() };

        ToolValue? back = data.MyData;

        Check.That(back).IsNotNull();
        Check.That(back!.Mode).IsEqualTo(ToolValueMode.Unhandled);
        Check.That(back.Type).IsEqualTo(typeof(Object).FullName);
        Check.That(back.Data).IsEqualTo("System.Object");
    }

    private sealed class DummyToolValueConverter : IToolValueConverter
    {
        public Boolean TryRead(ToolValue? value , out Object? result)
        {
            result = null; return false;
        }

        public Boolean TryWrite(Object? value , out ToolValue? result)
        {
            result = null; return false;
        }
    }
}
