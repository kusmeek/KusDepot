namespace KusDepot.Exams;

public static class DataItemSecurityTestHelpers
{
    public static void TamperEncryptedData(DataItem item)
    {
        var field = ((Object)item).GetType().GetField("EncryptedData", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy) ?? throw new InvalidOperationException("EncryptedData field not found");
        var data = (Byte[]?)field.GetValue(item);
        if (data != null && data.Length > 0)
        {
            data[0] ^= 0xFF;
            field.SetValue(item, data);
        }
    }

    public static void TamperFile(String filePath)
    {
        var bytes = File.ReadAllBytes(filePath);
        if (bytes.Length > 0) bytes[0] ^= 0xFF;
        File.WriteAllBytes(filePath, bytes);
    }

    public static String SetupStreamedContent(DataItem item, Byte[] data)
    {
        String tempFile = Path.GetTempFileName() + ".kdi";
        File.WriteAllBytes(tempFile, data);
        item.SetFILE(tempFile);
        item.SetContentStreamed(true);
        return tempFile;
    }
}
