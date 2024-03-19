namespace EntitySQLArk;

public static class ModelMaker
{
    public static ActiveService? ToActiveServiceModel(this Descriptor d)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(d);

            return new ActiveService()
            {
                Application        = d.Application,
                ApplicationVersion = d.ApplicationVersion,
                BornOn             = d.BornOn,
                DistinguishedName  = d.DistinguishedName,
                ID                 = d.ID,
                Modified           = d.Modified,
                Name               = d.Name,
                Purpose            = d.Purpose,
                Registered         = DateTimeOffset.Now.ToString("O"),
                ServiceVersion     = d.ServiceVersion,
                Version            = d.Version
            };
        }
        catch ( Exception ) { return null; }
    }

    public static Element? ToElementModel(this Descriptor d)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(d);

            return new Element()
            {
                Application        = d.Application,
                ApplicationVersion = d.ApplicationVersion,
                BornOn             = d.BornOn,
                DistinguishedName  = d.DistinguishedName,
                ID                 = d.ID,
                Modified           = d.Modified,
                Name               = d.Name,
                ObjectType         = d.ObjectType,
                ServiceVersion     = d.ServiceVersion,
                Type               = d.Type,
                Version            = d.Version
            };
        }
        catch ( Exception ) { return null; }
    }

    public static MultiMedia? ToMultiMediaModel(this Descriptor d)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(d);

            return new MultiMedia()
            {
                Application       = d.Application,
                Artist            = d.Artist,
                BornOn            = d.BornOn,
                DistinguishedName = d.DistinguishedName,
                ID                = d.ID,
                Modified          = d.Modified,
                Name              = d.Name,
                Title             = d.Title,
                Type              = d.Type,
                Year              = d.Year
            };
        }
        catch ( Exception ) { return null; }
    }

    public static Note? ToNoteModel(this Descriptor d)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(d);

            Note _ = new Note() { ID = d.ID };

            if(d.Notes is not null && !Equals(d.Notes.Count,0))
            {
                List<PropertyInfo> n = typeof(Note).GetProperties().Where(p=>p.Name.StartsWith("Note",StringComparison.Ordinal)).ToList();

                if(d.Notes.Count > n.Count) { return null; }

                for(Int32 i = 0; i < d.Notes.Count; i++) { n[i].SetValue(_,d.Notes.ElementAt(i)); }
            }

            return _;
        }
        catch ( Exception ) { return null; }
    }

    public static Tag? ToTagModel(this Descriptor d)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(d);

            Tag _ = new Tag() { ID = d.ID };

            if(d.Tags is not null && !Equals(d.Tags.Count,0))
            {
                List<PropertyInfo> t = typeof(Tag).GetProperties().Where(p=>p.Name.StartsWith("Tag",StringComparison.Ordinal)).ToList();

                if(d.Tags.Count > t.Count) { return null; }

                for(Int32 i = 0; i < d.Tags.Count; i++) { t[i].SetValue(_,d.Tags.ElementAt(i)); }
            }

            return _;
        }
        catch ( Exception ) { return null; }
    }
}