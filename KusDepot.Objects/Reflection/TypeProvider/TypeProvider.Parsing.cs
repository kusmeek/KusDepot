namespace KusDepot.Reflection;

public sealed partial class TypeProvider
{
    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryParseRequestNoSync"]/*'/>*/
    private Boolean TryParseRequest_NoSync(out TypeRequest? request)
    {
        request = null;

        try
        {
            if(String.IsNullOrWhiteSpace(Name)) { return false; }

            String n = Name.Trim(); Int32 i = FindAssemblySeparator(n);

            if(i < 0)
            {
                request = new(n); return true;
            }

            String tn = n[..i].Trim(); String a = n[(i + 1)..].Trim();

            if(String.IsNullOrWhiteSpace(tn) || String.IsNullOrWhiteSpace(a)) { return false; }

            if(TryParseAssemblyIdentity(a,out String? an,out Version? v,out Boolean hv,out String? c,out Boolean hc,out String? pkt,out Boolean hpkt) is false)
            {
                return false;
            }

            request = new(tn,an,v,hv,c,hc,pkt,hpkt); return true;
        }
        catch ( Exception _ ) { request = null; KusDepotLog.Error(_,TryParseRequestFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="FindAssemblySeparator"]/*'/>*/
    private static Int32 FindAssemblySeparator(String name)
    {
        try
        {
            Int32 d = 0;

            for(Int32 i = 0; i < name.Length; i++)
            {
                Char c = name[i];

                if(c == '[') { d++; continue; }

                if(c == ']') { if(d > 0) { d--; } continue; }

                if(c == ',' && Equals(d,0)) { return i; }
            }

            return -1;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,FindAssemblySeparatorFail); if(NoExceptions) { return -1; } throw; }
    }

    /**<include file='TypeProvider.xml' path='TypeProvider/class[@name="TypeProvider"]/method[@name="TryParseAssemblyIdentity"]/*'/>*/
    private static Boolean TryParseAssemblyIdentity(String input , out String? name , out Version? version , out Boolean hasversion , out String? culture , out Boolean hasculture , out String? publickeytoken , out Boolean haspublickeytoken)
    {
        name = null; version = null; hasversion = false; culture = null; hasculture = false; publickeytoken = null; haspublickeytoken = false;

        try
        {
            String[] t = input.Split(',',StringSplitOptions.TrimEntries);

            if(Equals(t.Length,0) || String.IsNullOrWhiteSpace(t[0])) { return false; }

            name = t[0].Trim();

            for(Int32 i = 1; i < t.Length; i++)
            {
                if(String.IsNullOrWhiteSpace(t[i])) { return false; }

                Int32 s = t[i].IndexOf('='); if(s <= 0 || s >= t[i].Length - 1) { return false; }

                String k = t[i][..s].Trim(); String v = t[i][(s + 1)..].Trim();

                if(String.IsNullOrWhiteSpace(k) || String.IsNullOrWhiteSpace(v)) { return false; }

                if(String.Equals(k,"Version",OrdinalIgnoreCase))
                {
                    if(hasversion || Version.TryParse(v,out version) is false) { return false; }

                    hasversion = true; continue;
                }

                if(String.Equals(k,"Culture",OrdinalIgnoreCase))
                {
                    if(hasculture) { return false; }

                    culture = v; hasculture = true; continue;
                }

                if(String.Equals(k,"PublicKeyToken",OrdinalIgnoreCase))
                {
                    if(haspublickeytoken) { return false; }

                    publickeytoken = v; haspublickeytoken = true; continue;
                }

                return false;
            }

            return true;
        }
        catch ( Exception _ )
        {
            name = null; version = null; hasversion = false; culture = null; hasculture = false; publickeytoken = null; haspublickeytoken = false;

            KusDepotLog.Error(_,TryParseAssemblyIdentityFail); if(NoExceptions) { return false; } throw;
        }
    }
}