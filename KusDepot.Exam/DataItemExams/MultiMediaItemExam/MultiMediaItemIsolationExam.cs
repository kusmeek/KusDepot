namespace KusDepot.Exams.DataItems;

public partial class MultiMediaItemExam
{
    [Test]
    public void GetContent_DataIsolationEnabled_ClonesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            Byte[] content = new Byte[]{1,2,3};
            MultiMediaItem item = new(content);

            Byte[]? got = item.GetContent();

            Check.That(got).ContainsExactly(content);
            Check.That(ReferenceEquals(got,content)).IsFalse();
        }
    }

    [Test]
    public void GetContent_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            Byte[] content = new Byte[]{1,2,3};
            MultiMediaItem item = new(content);

            Byte[]? got = item.GetContent();

            Check.That(got).ContainsExactly(content);
            Check.That(ReferenceEquals(got,content)).IsTrue();
        }
    }

    [Test]
    public void SetContent_DataIsolationEnabled_IsolatesIncomingArray()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            Byte[] content = new Byte[]{10,20,30};
            MultiMediaItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent(),content)).IsFalse();
        }
    }

    [Test]
    public void SetContent_DataIsolationDisabled_SharesIncomingArray()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            Byte[] content = new Byte[]{10,20,30};
            MultiMediaItem item = new();

            Check.That(item.SetContent(content)).IsTrue();

            Check.That(ReferenceEquals(item.GetContent(),content)).IsTrue();
        }
    }

    [Test]
    public void GetArtists_DataIsolationEnabled_ClonesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String artist = new("ArtistName");
            MultiMediaItem item = new();

            Check.That(item.SetArtists(new[]{artist})).IsTrue();

            HashSet<String>? got1 = item.GetArtists();
            HashSet<String>? got2 = item.GetArtists();

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsFalse();
        }
    }

    [Test]
    public void GetArtists_DataIsolationDisabled_SharesCollection()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String artist = new("ArtistName");
            MultiMediaItem item = new();

            Check.That(item.SetArtists(new[]{artist})).IsTrue();

            HashSet<String>? got1 = item.GetArtists();
            HashSet<String>? got2 = item.GetArtists();

            Check.That(got1).IsNotNull();
            Check.That(ReferenceEquals(got1,got2)).IsTrue();
        }
    }

    [Test]
    public void GetLanguage_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String language = new("en-US");
            MultiMediaItem item = new();

            Check.That(item.SetLanguage(language)).IsTrue();

            String? got = item.GetLanguage();

            Check.That(got).IsEqualTo(language);
            Check.That(ReferenceEquals(got,language)).IsFalse();
        }
    }

    [Test]
    public void GetLanguage_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String language = new("en-US");
            MultiMediaItem item = new();

            Check.That(item.SetLanguage(language)).IsTrue();

            String? got = item.GetLanguage();

            Check.That(got).IsEqualTo(language);
            Check.That(ReferenceEquals(got,language)).IsTrue();
        }
    }

    [Test]
    public void GetTitle_DataIsolationEnabled_IsolatesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String title = new("MyTitle");
            MultiMediaItem item = new();

            Check.That(item.SetTitle(title)).IsTrue();

            String? got = item.GetTitle();

            Check.That(got).IsEqualTo(title);
            Check.That(ReferenceEquals(got,title)).IsFalse();
        }
    }

    [Test]
    public void GetTitle_DataIsolationDisabled_SharesValue()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String title = new("MyTitle");
            MultiMediaItem item = new();

            Check.That(item.SetTitle(title)).IsTrue();

            String? got = item.GetTitle();

            Check.That(got).IsEqualTo(title);
            Check.That(ReferenceEquals(got,title)).IsTrue();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationEnabled_ClonesContent()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            Byte[] content = new Byte[]{5,6,7};
            MultiMediaItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.MultiMedia).ContainsKey("Content");
            Check.That(ReferenceEquals(dc.MultiMedia!["Content"],content)).IsFalse();
        }
    }

    [Test]
    public async Task GetDataContent_DataIsolationDisabled_SharesContent()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            Byte[] content = new Byte[]{5,6,7};
            MultiMediaItem item = new(content);

            DataContent? dc = await item.GetDataContent(security:null);

            Check.That(dc).IsNotNull();
            Check.That(dc!.MultiMedia).ContainsKey("Content");
            Check.That(ReferenceEquals(dc.MultiMedia!["Content"],content)).IsTrue();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationEnabled_IsolatesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Enabled))
        {
            String name = new("IsolatedName");
            MultiMediaItem item = new(name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsFalse();
        }
    }

    [Test]
    public void GetDescriptor_DataIsolationDisabled_SharesFields()
    {
        using(DataIsolation.BeginScope(DataIsolationMode.Disabled))
        {
            String name = new("SharedName");
            MultiMediaItem item = new(name:name);

            Descriptor descriptor = item.GetDescriptor()!;

            Check.That(descriptor.Name).IsEqualTo(name);
            Check.That(ReferenceEquals(descriptor.Name,name)).IsTrue();
        }
    }
}
