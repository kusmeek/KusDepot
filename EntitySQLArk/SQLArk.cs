namespace EntitySQLArk;

public class SQLArk
{
    public SQLArk() { Log.Logger = new LoggerConfiguration().WriteTo.File(LogFilePath,shared:true,formatProvider:InvariantCulture).CreateLogger(); }

    private readonly Object Sync = new Object();

    public Boolean AddUpdate(Descriptor? descriptor)
    {
        try
        {
            if(descriptor is null) { return false; }

            if(!TryEnter(this.Sync,SyncTime)) { throw SQLArkSyncException; }

            List<String> n = new List<String>(){typeof(GuidReferenceItem).Name,typeof(TextItem).Name,typeof(CodeItem).Name,typeof(GenericItem).Name,typeof(BinaryItem).Name,typeof(MultiMediaItem).Name,typeof(MSBuildItem).Name};

            SQLArkContext c = new SQLArkContext(); Descriptor d = descriptor; Boolean _ = true;

            _ = _ & AddUpdate(c.Tags,d.ToTagModel());                                                                                                              if(!_) { Log.Error(AddUpdateFailDescriptor,d); c.Dispose(); return false; }

            _ = _ & AddUpdate(c.Notes,d.ToNoteModel());                                                                                                            if(!_) { Log.Error(AddUpdateFailDescriptor,d); c.Dispose(); return false; }

            _ = _ & AddUpdate(c.Elements,d.ToElementModel());                                                                                                      if(!_) { Log.Error(AddUpdateFailDescriptor,d); c.Dispose(); return false; }

            if(String.Equals(d.ObjectType,typeof(MultiMediaItem).Name,StringComparison.Ordinal)) { _ = _ & AddUpdate(c.MediaLibrary,d.ToMultiMediaModel()); }      if(!_) { Log.Error(AddUpdateFailDescriptor,d); c.Dispose(); return false; }

            if(!n.Contains(d.ObjectType!))                                                       { _ = _ & AddUpdate(c.ActiveServices,d.ToActiveServiceModel()); } if(!_) { Log.Error(AddUpdateFailDescriptor,d); c.Dispose(); return false; }

            c.SaveChanges(true); Log.Information(AddUpdateSuccessDescriptor,d); c.Dispose(); return true;
        }
        catch ( Exception _ ) { Log.Error(_,AddUpdateFail); return false; }

        finally { if(IsEntered(this.Sync)) { Exit(this.Sync); } }
    }

    private static Boolean AddUpdate<T>(DbSet<T>? dbset , T? model) where T : ModelBase
    {
        try
        {
            if(dbset is null || model is null) { return false; }

            T? _ = dbset.Find(model.ID); if(_ is not null) { dbset.Entry(_).CurrentValues.SetValues(model); return true; }

            dbset.Add(model); return true;
        }
        catch ( Exception _ ) { Log.Error(_,AddUpdateFail); return false; }
    }

    public Boolean Exists(Descriptor? descriptor)
    {
        try
        {
            if(descriptor is null) { return false; }

            if(!TryEnter(this.Sync,SyncTime)) { throw SQLArkSyncException; }

            using(SQLArkContext c = new SQLArkContext())
            {
                if(c.Elements!.Find(descriptor.ID) is not null) { return true; }

                return false;
            }
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFail); return false; }

        finally { if(IsEntered(this.Sync)) { Exit(this.Sync); } }
    }

    public Boolean Remove(Descriptor? descriptor)
    {
        try
        {
            if(descriptor is null) { return false; }

            if(!TryEnter(this.Sync,SyncTime)) { throw SQLArkSyncException; }

            SQLArkContext c = new SQLArkContext(); Descriptor d = descriptor; Boolean _ = true;

            _ = _ & Remove(c.Tags,d.ToTagModel());                                                                      if(!_) { Log.Error(RemoveFailDescriptor,d); c.Dispose(); return false; }

            _ = _ & Remove(c.Notes,d.ToNoteModel());                                                                    if(!_) { Log.Error(RemoveFailDescriptor,d); c.Dispose(); return false; }

            _ = _ & Remove(c.Elements,d.ToElementModel());                                                              if(!_) { Log.Error(RemoveFailDescriptor,d); c.Dispose(); return false; }

            if(c.MediaLibrary!.Find(d.ID) is not null)   { _ = _ & Remove(c.MediaLibrary,d.ToMultiMediaModel()); }      if(!_) { Log.Error(RemoveFailDescriptor,d); c.Dispose(); return false; }

            if(c.ActiveServices!.Find(d.ID) is not null) { _ = _ & Remove(c.ActiveServices,d.ToActiveServiceModel()); } if(!_) { Log.Error(RemoveFailDescriptor,d); c.Dispose(); return false; }

            c.SaveChanges(true); Log.Information(RemoveSuccessDescriptor,d); c.Dispose(); return true;
        }
        catch ( Exception _ ) { Log.Error(_,RemoveFail); return false; }

        finally { if(IsEntered(this.Sync)) { Exit(this.Sync); } }
    }

    private static Boolean Remove<T>(DbSet<T>? dbset , T? model) where T : ModelBase
    {
        try
        {
            if(dbset is null || model is null) { return false; }

            T? _ = dbset.Find(model.ID); if(_ is not null) { dbset.Remove(_); return true; }

            return false;
        }
        catch ( Exception _ ) { Log.Error(_,RemoveFail); return false; }
    }
}