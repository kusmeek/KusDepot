namespace KusDepot.Serialization;

/**<include file='AtomicFilePublisher.xml' path='AtomicFilePublisher/class[@name="AtomicFilePublisher"]/main/*'/>*/
internal static class AtomicFilePublisher
{
    /**<include file='AtomicFilePublisher.xml' path='AtomicFilePublisher/class[@name="AtomicFilePublisher"]/method[@name="Write"]/*'/>*/
    internal static Boolean Write(String path , Action<Stream> write , Boolean createDirectory = false)
    {
        String fullPath = Path.GetFullPath(path);

        if(File.Exists(fullPath)) { return false; }

        String directory = Path.GetDirectoryName(fullPath)!;

        if(createDirectory && Directory.Exists(directory) is false)
        {
            Directory.CreateDirectory(directory);
        }

        String stagingPath = Path.Combine(directory,$".kusdepot-write-{Guid.NewGuid():N}.tmp");

        Boolean published = false;

        try
        {
            using(FileStream stream = new(stagingPath,new FileStreamOptions
            {
                Access = FileAccess.Write , Mode = FileMode.CreateNew,
                Options = FileOptions.SequentialScan , Share = FileShare.None
            }))
            {
                write(stream); stream.Flush(flushToDisk:true);
            }

            try
            {
                File.Move(stagingPath,fullPath,overwrite:false);

                published = true;

                return true;
            }
            catch ( IOException ) when(File.Exists(fullPath))
            {
                return false;
            }
        }
        finally
        {
            if(published is false && File.Exists(stagingPath))
            {
                try { File.Delete(stagingPath); } catch { }
            }
        }
    }
}
