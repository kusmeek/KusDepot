# KusDepot for Copilot

## A. Core Abstractions
- Tool: Base for lifecycle, access control, service hosting, command registration, outputs, data, status, synchronization.
- Command: Encapsulates executable unit (sync and/or async) attached to a Tool via a key.
- Activity: Execution context holding CommandDetails, arguments, logger, cancellation.
- Access: AccessManager issues AccessKey, ManagerKey, CommandKey; verify through AccessCheck before protected operations.
- Configuration: Inject IConfiguration; map strongly typed records; validate before use.

## B. Lifecycle Pattern
Implement overrides (StartingAsync, StartedAsync, StoppingAsync, StoppedAsync) with:
1. DC(); disposal guard.
2. Acquire a private SemaphoreSlim to serialize state changes.
3. Guard with LifeState.*OK() and AccessCheck.
4. Perform custom init then call base method (start) or call base first (stop).
5. Log success/failure; swallow exceptions if exception suppression flags are active.

## C. Synchronization Rules
- Use existing Sync nodes (e.g. Sync.Commands, Sync.Outputs) before mutating protected collections.
- Acquire locks or semaphores; release in finally.
- Never expose internal mutable collections directly; return copies or clones.

## D. Access Control
- Every protected operation requires AccessCheck(key).
- Initialize access in constructor via InitializeAccessManagement producing SelfAccessKey and SelfManagerKey.
- Lock/Unlock uses secrets created from management keys; follow existing Lock semantics.

## E. Command Design
- Derive from Command; set ExecutionMode (AllowSynchronous / AllowAsynchronous / AllowBoth).
- Guard: activity != null, AccessCheck(key) == true.
- AddActivity(activity) then log.
- Extract arguments from activity.Details.
- On success AddOutput(activity.ID,result) (if any) and return activity.ID.
- On failure AddOutput(activity.ID) (placeholder) and return null.
- Always CleanUp(activity) in finally.
- Async version wraps logic in local WorkAsync() assigned to activity.Task.
- CommandWorkflow - Determine action and log work steps.

## F. Activity & Output Retrieval
- Caller receives Guid? from Execute / ExecuteAsync.
- Retrieve output using GetOutput(id,key) or GetOutputAsync(id,cancel,timeout,interval,key).
- Use GetRemoveOutput(id,key) for consume-once semantics.

## G. Manager Pattern
- Specialized Tool maintaining scoped configuration and operational state (e.g. process id, health metrics).
- Constructor loads config section, validates.
- Provide Manage() method executing health check command and remediation (restart/terminate etc.) based on output presence or code.

## H. Supervisor Pattern
- Aggregates multiple Manager instances; stores them in a dictionary keyed by type name.
- In StartingAsync: load and validate aggregate configuration; resolve hosted services; register managers.

## I. Configuration Records
- Use immutable records with Parse(), ToString(), Validate().
- Serialization via JsonSerializer; case-sensitive handling.

## J. Factory Pattern
- Use builder abstraction to compose Tool instances.
- Chain: CreateBuilder() -> RegisterCommand<T>("Handle") -> ConfigureToolConfiguration(...) -> ConfigureServices(... logger, services) -> BuildAsync<ToolType>().
- For aggregate factories: build subordinate Tools first, register them as implemented interfaces in parent builder.

## K. Logging Conventions
- Use structured logging (Logger?.Information / Logger?.Error) with constant message templates.
- Log at start/end of lifecycle transitions, command execution.

## L. Data Items & DataFactory
- Security: Encrypt/Decrypt-Data, Sign/Verify-Data.
- DataFactory provides creation helpers returning concrete derivatives.
- DataSetItem aggregates other DataItems; GenericItem can hold arbitrary object collection. GuidReferenceItem stores a Guid reference graph element.

## M. Access Management
- Protected operations are enumerated (Activate .. UpdateInputs) and resolved by name -> index using ProtectedOperationResolver for bitmap lookup.
- Operation sets define permissions scope: Client, Command, Executive, Host, MyHost, Service (each is Int32 array of operation indices).
- AccessKeySecret.TryCreate builds encrypted key material: embeds tool id, access manager id, issued time, optional expiry, subject string, permission blocks (UInt128 bitmap per block) + salt + token hash; returns secret + AccessKeyToken.
- AccessKeySecret.TryParse decrypts and validates secret, recomputes hash, extracts permissions bitmap, subject, issued/expiry, token; verifies tool and manager ids.
- AccessKeyToken ensures membership: stored per subject; cleared on DestroySecrets.
 
-- AccessRequests:
 - StandardRequest: generic client access.
 - ServiceRequest: carries requesting Tool reference.
 - ManagementRequest: provides serialized management/owner key enabling executive elevation.
- Keys: ClientKey, ServiceKey, CommandKey, HostKey, MyHostKey, ExecutiveKey, OwnerKey, ManagerKey.

-- Custom AccessManager:
 - Derive from AccessManager; override RequestAccess to impose domain-specific gating (e.g. subject patterns, rate limits).
 - Override Generate*Key methods to alter lifetime or permission sets (pass custom operations instead of predefined sets).
 - Maintain locking semantics: if Tool.GetLocked() is true and AccessCheck fails -> deny privileged actions.
 - Preserve token issuance pattern: TryIssueToken(operations,subject,out ciphertext,out token) then AddMembership.
 - For auditing: wrap AddMembership / RemoveMembership with logging or metrics before calling base.
 - Never bypass AccessCheck inside overrides for protected operations.

-- ProtectedOperationSets:
 - Use custom or predefined sets (ProtectedOperationSets.Client / Command / Executive) when issuing keys to map role.
 - Pass the set directly to key generation (e.g. GenerateCommandKey(ProtectedOperationSets.Command)).