namespace KusDepot.Reflection;

/**<include file='ReflectionBindingScore.xml' path='ReflectionBindingScore/class[@name="ReflectionBindingScore"]/main/*'/>*/
internal static class ReflectionBindingScore
{
    /**<include file='ReflectionBindingScore.xml' path='ReflectionBindingScore/class[@name="ReflectionBindingScore"]/method[@name="Compare"]/*'/>*/
    internal static Int32 Compare(Int32 length , Int32 exact , Int32 assignable , Boolean isstatic , Int32 bestLength , Int32 bestExact , Int32 bestAssignable , Boolean? bestStatic , Boolean preferInstanceOnTie)
    {
        try
        {
            if(length < bestLength) { return 1; }

            if(length > bestLength) { return -1; }

            if(exact > bestExact) { return 1; }

            if(exact < bestExact) { return -1; }

            if(assignable > bestAssignable) { return 1; }

            if(assignable < bestAssignable) { return -1; }

            if(preferInstanceOnTie && bestStatic is not null)
            {
                if(bestStatic.Value && !isstatic) { return 1; }

                if(!bestStatic.Value && isstatic) { return -1; }
            }

            return 0;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CompareFail); if(NoExceptions) { return 0; } throw; }
    }
}