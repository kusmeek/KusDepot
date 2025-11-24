namespace KusDepot.Data;

/**<include file='TraceUtility.xml' path='TraceUtility/class[@name="TraceUtility"]/main/*'/>*/
public static class TraceUtility
{
    /**<include file='TraceUtility.xml' path='TraceUtility/class[@name="TraceUtility"]/method[@name="GetUPN"]/*'/>*/
    public static String? GetUPN(String? token)
    {
        try
        {
            if(String.IsNullOrEmpty(token)) { return null; }

            JsonWebTokenHandler _ = new(); if(!_.CanReadToken(token)) { return null; }

            return ((JsonWebToken)_.ReadToken(token)).Claims.FirstOrDefault((_)=>{ return String.Equals(_.Type,"upn",StringComparison.Ordinal); })?.Value;
        }
        catch { return null; }
    }
}