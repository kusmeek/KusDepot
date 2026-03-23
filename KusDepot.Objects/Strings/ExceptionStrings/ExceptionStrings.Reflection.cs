namespace KusDepot.Strings;

public static partial class ExceptionStrings
{
    #pragma warning disable CS1591

    public const String WithDataFail                                  = @"WithData Failed";
    public const String SetDataFail                                   = @"SetData Failed";
    public const String WithFormatterFail                             = @"WithFormatter Failed";
    public const String SetFormatterFail                              = @"SetFormatter Failed";
    public const String WithContextFail                               = @"WithContext Failed";
    public const String SetContextFail                                = @"SetContext Failed";
    public const String FindTryParseMethodFail                        = @"FindTryParseMethod Failed";
    public const String FindParseMethodFail                           = @"FindParseMethod Failed";
    public const String TryInvokeTryParseFail                         = @"TryInvokeTryParseMethod Failed";
    public const String TryInvokeParseFail                            = @"TryInvokeParseMethod Failed";
    public const String IsTryParseMethodFail                          = @"IsTryParseMethod Failed";
    public const String IsParseMethodFail                             = @"IsParseMethod Failed";
    public const String CanParseTypeFail                              = @"CanParse(Type) Failed";
    public const String CanParseNameFail                              = @"CanParse(String) Failed";

    public const String BuildFail                                     = @"Build Failed";
    public const String WithArgumentFail                              = @"WithArgument Failed";
    public const String SetArgumentFail                               = @"SetArgument Failed";
    public const String TryResolveTypeFail                            = @"TryResolveType Failed";
    public const String ResolveBuildTypeFail                          = @"ResolveBuildType Failed";
    public const String SelectConstructorFail                         = @"SelectConstructor Failed";
    public const String GetRequiredWidthFail                          = @"GetRequiredWidth Failed";
    public const String TryCreateInvokeArgumentsFail                  = @"TryCreateInvokeArguments Failed";

    public const String WithAllowStaticMembersFail                    = @"WithAllowStaticMembers Failed";
    public const String SetAllowStaticMembersFail                     = @"SetAllowStaticMembers Failed";
    public const String WithInstanceFail                              = @"WithInstance Failed";
    public const String SetInstanceFail                               = @"SetInstance Failed";
    public const String GetFieldFail                                  = @"GetField Failed";
    public const String SetFieldFail                                  = @"SetField Failed";
    public const String GetPropertyFail                               = @"GetProperty Failed";
    public const String SetPropertyFail                               = @"SetProperty Failed";
    public const String InvokeFail                                    = @"Invoke Failed";
    public const String InvokeAsyncFail                               = @"InvokeAsync Failed";
    public const String NormalizeAsyncResultFail                      = @"NormalizeAsyncResult Failed";
    public const String GetTargetTypeFail                             = @"GetTargetType Failed";
    public const String TryFindFieldFail                              = @"TryFindField Failed";
    public const String TryFindPropertyFail                           = @"TryFindProperty Failed";
    public const String TrySelectMethodFail                           = @"TrySelectMethod Failed";
    public const String TryCreateInvokeArgsFail                       = @"TryCreateInvokeArgs Failed";
    public const String TryBindLeadingParametersFail                  = @"TryBindLeadingParameters Failed";
    public const String TryBindParamsTailFail                         = @"TryBindParamsTail Failed";
    public const String GetByRefIndicesFail                           = @"GetByRefIndices Failed";
    public const String InvokeDetailedFail                            = @"InvokeDetailed Failed";
    public const String InvokeDetailedAsyncFail                       = @"InvokeDetailedAsync Failed";
    public const String InvokeStaticFail                              = @"InvokeStatic Failed";
    public const String InvokeStaticAsyncFail                         = @"InvokeStaticAsync Failed";
    public const String TrySelectStaticMethodFail                     = @"TrySelectStaticMethod Failed";
    public const String GetTypeFail                                   = @"GetType Failed [{@ObjectType}] [{@ObjectID}]";
    public const String GetValueFail                                  = @"GetValue Failed [{@ObjectType}] [{@ObjectID}]";

    public const String WithTypeFail                                  = @"WithType Failed";
    public const String SetTypeFail                                   = @"SetType Failed";
    public const String WithAssembliesFail                            = @"WithAssemblies Failed";
    public const String SetAssembliesFail                             = @"SetAssemblies Failed";
    public const String WithDirectoriesFail                           = @"WithDirectories Failed";
    public const String SetDirectoriesFail                            = @"SetDirectories Failed";
    public const String WithAssemblyNamesFail                         = @"WithAssemblyNames Failed";
    public const String SetAssemblyNamesFail                          = @"SetAssemblyNames Failed";
    public const String TryResolveFail                                = @"TryResolve Failed";
    public const String ResolveFail                                   = @"Resolve Failed";
    public const String UnloadFail                                    = @"Unload Failed";
    public const String AssemblyNamesEqualFail                        = @"AssemblyNamesEqual Failed";
    public const String GetCultureTextFail                            = @"GetCultureText Failed";
    public const String GetPublicKeyTokenTextFail                     = @"GetPublicKeyTokenText Failed";
    public const String MatchesRequestAssemblyFail                    = @"MatchesRequestAssembly Failed";
    public const String MatchesRequestedAssemblyFail                  = @"MatchesRequestedAssembly Failed";
    public const String TryFindLoadedByPathInContextFail              = @"TryFindLoadedByPathInContext Failed";
    public const String TryFindLoadedByRequestInContextFail           = @"TryFindLoadedByRequestInContext Failed";
    public const String TryFindLoadedByRequestedAssemblyInContextFail = @"TryFindLoadedByRequestedAssemblyInContext Failed";
    public const String TryGetAssemblyNameFail                        = @"TryGetAssemblyName Failed";
    public const String TryNormalizeAssemblyNameFail                  = @"TryNormalizeAssemblyName Failed";
    public const String TryReadAssemblyNameFail                       = @"TryReadAssemblyName Failed";
    public const String TrySnapshotAssemblyImageFail                  = @"TrySnapshotAssemblyImage Failed";
    public const String TryParseRequestFail                           = @"TryParseRequest Failed";
    public const String FindAssemblySeparatorFail                     = @"FindAssemblySeparator Failed";
    public const String TryParseAssemblyIdentityFail                  = @"TryParseAssemblyIdentity Failed";
    public const String TryResolveNoSyncFail                          = @"TryResolve_NoSync Failed";
    public const String TryGetEffectiveRequestsFail                   = @"TryGetEffectiveRequests Failed";
    public const String TryResolveInDefaultContextFail                = @"TryResolveInDefaultContext Failed";
    public const String EnsureActiveContextFail                       = @"EnsureActiveContext Failed";
    public const String UnloadOwnedContextFail                        = @"UnloadOwnedContext Failed";
    public const String EnumerateCandidateAssemblyPathsFail           = @"EnumerateCandidateAssemblyPaths Failed";
    public const String GetAssemblyDirectoriesFail                    = @"GetAssemblyDirectories Failed";
    public const String TryResolveFromPathsInActiveContextFail        = @"TryResolveFromPathsInActiveContext Failed";
    public const String TryResolveFromImagesInActiveContextFail       = @"TryResolveFromImagesInActiveContext Failed";
    public const String ResolveContextDependencyFail                  = @"ResolveContextDependency Failed";
    public const String TryLoadImageByRequestedAssemblyInContextFail  = @"TryLoadImageByRequestedAssemblyInContext Failed";
    public const String TryResolveInAssemblyFail                      = @"TryResolveInAssembly Failed";
    public const String MatchesResolvedTypeFail                       = @"MatchesResolvedType Failed";
    public const String TypeRequestsEqualFail                         = @"TypeRequestsEqual Failed";

    public const String IsCompatibleFail                              = @"IsCompatible Failed";
    public const String CanAssignNullFail                             = @"CanAssignNull Failed";
    public const String CompareFail                                   = @"Compare Failed";
    public const String GetBindableTypeFail                           = @"GetBindableType Failed";
    public const String IsOutParameterFail                            = @"IsOutParameter Failed";
    public const String IsRefParameterFail                            = @"IsRefParameter Failed";
    public const String AutoFillOutParameterFail                      = @"AutoFillOutParameter Failed";
}