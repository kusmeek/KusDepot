namespace KusDepot;

/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/main/*'/>*/
internal sealed class DataItemStateSnapshot
{
	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/field[@name="dataEncrypted"]/*'/>*/
	private readonly Boolean dataEncrypted;

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/field[@name="dataProtectionInfo"]/*'/>*/
	private readonly DataProtectionInfo? dataProtectionInfo;

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/field[@name="encryptedData"]/*'/>*/
	private readonly Byte[]? encryptedData;

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/field[@name="fieldAssertions"]/*'/>*/
	private readonly Dictionary<String,Byte[]>? fieldAssertions;

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/field[@name="fieldIntegrityHashes"]/*'/>*/
	private readonly Dictionary<String,Byte[]>? fieldIntegrityHashes;

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/field[@name="fileSizeCached"]/*'/>*/
	private readonly Int64? fileSizeCached;

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/field[@name="contentStreamed"]/*'/>*/
	private readonly Boolean contentStreamed;

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/field[@name="contentMemento"]/*'/>*/
	private readonly DataItemContentMutationMemento? contentMemento;

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/constructor[@name="Constructor"]/*'/>*/
	internal DataItemStateSnapshot(DataItem item) : this(item,null) { }

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/constructor[@name="ConstructorWithContentMemento"]/*'/>*/
	private DataItemStateSnapshot(DataItem item , DataItemContentMutationMemento? contentMemento)
	{
		ArgumentNullException.ThrowIfNull(item);

		this.dataEncrypted = item.DataEncrypted;
		this.dataProtectionInfo = item.DataProtectionInfo;
		this.encryptedData = item.EncryptedData;
		this.fieldAssertions = Clone(item.FieldAssertions);
		this.fieldIntegrityHashes = Clone(item.FieldIntegrityHashes);
		this.fileSizeCached = item.FileSizeCached;
		this.contentStreamed = item.ContentStreamed;
		this.contentMemento = contentMemento;
	}

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/method[@name="CaptureForContentMutation_NoSync"]/*'/>*/
	internal static DataItemStateSnapshot CaptureForContentMutation_NoSync(DataItem item)
	{
		ArgumentNullException.ThrowIfNull(item);

		return new DataItemStateSnapshot(item,item.CaptureContentMutationMemento_NoSync());
	}

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/method[@name="Restore"]/*'/>*/
	internal void Restore(DataItem item)
	{
		this.contentMemento?.Restore(item);

		item.DataEncrypted = this.dataEncrypted;
		item.DataProtectionInfo = this.dataProtectionInfo;
		item.EncryptedData = this.encryptedData;
		item.FieldAssertions = Clone(this.fieldAssertions);
		item.FieldIntegrityHashes = Clone(this.fieldIntegrityHashes);
		item.FileSizeCached = this.fileSizeCached;
		item.ContentStreamed = this.contentStreamed;
	}

	/**<include file='DataItemStateSnapshot.xml' path='DataItemStateSnapshot/class[@name="DataItemStateSnapshot"]/method[@name="Clone"]/*'/>*/
	private static Dictionary<String,Byte[]>? Clone(Dictionary<String,Byte[]>? source)
	{
		if(source is null) { return null; }

		Dictionary<String,Byte[]> copy = new(source.Count,source.Comparer);

		foreach(var pair in source) { copy.Add(pair.Key,pair.Value is null ? null! : (Byte[])pair.Value.Clone()); }

		return copy;
	}
}
