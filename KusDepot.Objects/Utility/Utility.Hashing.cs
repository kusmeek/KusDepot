namespace KusDepot;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicHashCode"]/*'/>*/
    public static Int32 GenerateDeterministicHashCode(Object? obj)
    {
        if(obj is null) { return unchecked((Int32)0xBAADF00D); }

        try
        {
            return BitConverter.ToInt32(SHA512.HashData(GetDeterministicBytes(obj,new HashSet<Object>(ReferenceEqualityComparer.Instance))),0);
        }
        catch ( StackOverflowException )
        {
            return BitConverter.ToInt32(SHA512.HashData((obj.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GenerateDeterministicHashCodeFail); if(NoExceptions) { return 0; } throw; }

        static Byte[] GetDeterministicBytes(Object? o , HashSet<Object> visited)
        {
            if(o is null) { return new Byte[] { 0xFF }; }

            if(visited.Contains(o)) { return new Byte[] { 0xFE }; } visited.Add(o);

            try
            {
                switch(o)
                {
                    case Byte b: return new[] {b};
                    case Guid g: return g.ToByteArray();
                    case BigInteger bi: return bi.ToByteArray();
                    case Char c: return BitConverter.GetBytes(c);
                    case Int16 s: return BitConverter.GetBytes(s);
                    case UInt16 us: return BitConverter.GetBytes(us);
                    case Int32 i: return BitConverter.GetBytes(i);
                    case UInt32 ui: return BitConverter.GetBytes(ui);
                    case Int64 l: return BitConverter.GetBytes(l);
                    case UInt64 ul: return BitConverter.GetBytes(ul);
                    case Single f: return BitConverter.GetBytes(f);
                    case Double d: return BitConverter.GetBytes(d);
                    case Boolean bo: return BitConverter.GetBytes(bo);
                    case SByte sb: return new[] { unchecked((Byte)sb)};
                    case String str: return str.ToByteArrayFromUTF16String();
                    case IntPtr ip: return BitConverter.GetBytes(ip.ToInt64());
                    case UIntPtr up: return BitConverter.GetBytes(up.ToUInt64());
                    case IDataItem di: return BitConverter.GetBytes(di.GetHashCode());
                    case Half h: return BitConverter.GetBytes(BitConverter.HalfToUInt16Bits(h));
                    case TimeSpan ts: return BitConverter.GetBytes(ts.Ticks);
                    case DateTime dt: return BitConverter.GetBytes(dt.ToBinary());
                    case DateTimeOffset dto: return BitConverter.GetBytes(dto.ToUnixTimeMilliseconds());
                    case Decimal dec: return Decimal.GetBits(dec).SelectMany(BitConverter.GetBytes).ToArray();
                    case Uri uri:
                    {
                        return (uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString).ToByteArrayFromUTF16String();
                    }
                    case Version v:
                    {
                        var vp = new Int32[] { v.Major,v.Minor,v.Build,v.Revision };
                        return vp.SelectMany(BitConverter.GetBytes).ToArray();
                    }
                    case System.Collections.IDictionary dct:
                    {
                        var tn = (dct.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String();
                        var e = new List<Byte[]>(dct.Count);
                        foreach(var k in dct.Keys)
                        {
                            e.Add(GetDeterministicBytes(k,visited).Concat(GetDeterministicBytes(dct[k],visited)).ToArray());
                        }
                        e.Sort(CompareByteArrays); return tn.Concat(e.SelectMany(b => b)).ToArray();
                    }
                    case Array ary:
                    {
                        var tn = (ary.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String();
                        if(ary is Byte[] ba) { return tn.Concat(ba).ToArray(); }
                        return tn.Concat(ary.Cast<Object?>().SelectMany(_ => GetDeterministicBytes(_,visited))).ToArray();
                    }
                    case System.Collections.IEnumerable en:
                    {
                        var tn = (en.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String();
                        var bl = en.Cast<Object?>().Select(_ => GetDeterministicBytes(_,visited)).ToList();
                        if(IsUnorderedCollection(en.GetType())) { bl.Sort(CompareByteArrays); }
                        return tn.Concat(bl.SelectMany(b => b)).ToArray();
                    }
                    default:
                    {
                        var t = o.GetType();
                        var ps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        .OrderBy(p => p.Name, StringComparer.Ordinal);
                        var b = new List<Byte[]>(); b.Add((t.FullName ?? String.Empty).ToByteArrayFromUTF16String());
                        var ts = String.Join(";",ps.Select(p => $"{p.Name}:{p.PropertyType.FullName}")); b.Add(Encoding.Unicode.GetBytes(ts));
                        foreach(var p in ps)
                        {
                            if(p.GetIndexParameters().Length == 0)
                            {
                                var v = p.GetValue(o);
                                b.Add(p.Name.ToByteArrayFromUTF16String());
                                b.Add(GetDeterministicBytes(v,visited));
                            }
                        }
                        return b.SelectMany(b => b).ToArray();
                    }
                }
            }
            finally { visited.Remove(o); }
        }

        static Boolean IsUnorderedCollection(Type type)
        {
            if(type.IsGenericType)
            {
                var g = type.GetGenericTypeDefinition(); if(g == typeof(ISet<>) || g == typeof(ConcurrentBag<>)) { return true; }
            }
            foreach(var i in type.GetInterfaces())
            {
                if(i.IsGenericType)
                {
                    var g = i.GetGenericTypeDefinition(); if(g == typeof(ISet<>) || g == typeof(ConcurrentBag<>)) { return true; }
                }
            }
            return false;
        }

        static Int32 CompareByteArrays(Byte[] a , Byte[] b)
        {
            for(Int32 i = 0; i < Math.Min(a.Length,b.Length); i++)
            {
                if(a[i] != b[i]) { return a[i].CompareTo(b[i]); }
            }
            return a.Length.CompareTo(b.Length);
        }
    }
}