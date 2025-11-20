namespace KusDepot.AI;

public class AzureTextTranslator : Command
{
    public AzureTextTranslator() : base() { ExecutionMode.AllowBoth(); }

    ///<inheritdoc/>
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            try
            {
                AddActivity(a);

                Object? c = a.Details?.GetArgument("Credential"); String? r = a.Details?.GetArgument<String>("Region");

                String? i = a.Details?.GetArgument<String>("Input"); String? l = a.Details?.GetArgument<String>("Language");

                if(c is null || String.IsNullOrEmpty(r) || String.IsNullOrEmpty(i) || String.IsNullOrEmpty(l))
                { AddOutput(a.ID); return null; }

                if( Translate(a,c,r!,i!,l!) ) { return a.ID; } else { AddOutput(a.ID); return null; }
            }
            catch ( Exception _ ) { a.Logger?.Error(_,CommandExecuteFail,GetType().Name,a.Details?.Handle); AddOutput(a.ID); return null; }

            finally { CleanUp(a); }
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); CleanUp(a); return null; }
    }

    ///<inheritdoc/>
    public override async Task<Guid?> ExecuteAsync(Activity? activity = null , AccessKey? key = null)
    {
        if(activity is null) { return null; } Activity a = activity;

        if(Enabled is false || AccessCheck(key) is false) { return null; }

        try
        {
            async Task<Object?> WorkAsync()
            {
                try
                {
                    AddActivity(a);

                    Object? c = a.Details?.GetArgument("Credential"); String? r = a.Details?.GetArgument<String>("Region");

                    String? i = a.Details?.GetArgument<String>("Input"); String? l = a.Details?.GetArgument<String>("Language");

                    if(c is null || String.IsNullOrEmpty(r) || String.IsNullOrEmpty(i) || String.IsNullOrEmpty(l))
                    { AddOutput(a.ID); return false; }

                    if( Translate(a,c,r!,i!,l!) ) { await Task.CompletedTask; return true; } else { AddOutput(a.ID); return false; }
                }
                catch ( OperationCanceledException ) { AddOutput(a.ID); return false; }

                catch ( Exception _ ) { a.Logger?.Error(_,CommandExecuteFail,GetType().Name,a.Details?.Handle); AddOutput(a.ID); return false; }

                finally { CleanUp(a); }
            }

            a.Task = WorkAsync(); await a.Task.ConfigureAwait(false); return a.ID;
        }
        catch ( Exception _ ) { a?.Logger?.Error(_,CommandExecuteFail,GetType().Name,a?.Details?.Handle); AddOutput(a?.ID); CleanUp(a); return null; }
    }

    private Boolean Translate(Activity a , Object credential , String region , String input , String language)
    {
        try
        {
            TranslatedTextItem? t = null; if(credential is TokenCredential tc) { t = new TextTranslationClient(tc).Translate(language,input,null,a.Cancel!.Token).Value[0]; }

            if(credential is AzureKeyCredential kc ) { t = new TextTranslationClient(kc,region:region).Translate(language,input,null,a.Cancel!.Token).Value[0]; }

            if(t is null) { CleanUp(a); return false; } AddOutput(a.ID,t.Translations[0].Text); CleanUp(a); return true;
        }
        catch ( Exception _ ) { a.Logger?.Error(_,AzureTextTranslatorFail,a.ID); CleanUp(a); return false; }
    }
}