namespace KusDepot.ToolGrains;

[Alias("KusDepot.IToolGrain")]
public interface IToolGrain : IGrainWithStringKey , IToolGrainBase
{
    [Alias("ExecuteCommand")]
    [Transaction(TransactionOption.CreateOrJoin)]
    Task<Guid?> ExecuteCommand(CommandDetails? details , AccessKey? key);

    [Alias("ExecuteCommandCab")]
    [Transaction(TransactionOption.CreateOrJoin)]
    Task<Guid?> ExecuteCommandCab(KusDepotCab? cab , AccessKey? key);

    [Alias("GetOutput")]
    [Transaction(TransactionOption.CreateOrJoin)]
    Task<Object?> GetOutput(Guid? id , AccessKey? key);

    [Alias("RequestAccess")]
    [Transaction(TransactionOption.CreateOrJoin)]
    Task<AccessKey?> RequestAccess(AccessRequest? request);

    [Alias("RevokeAccess")]
    [Transaction(TransactionOption.CreateOrJoin)]
    Task<Boolean> RevokeAccess(AccessKey? key);
}