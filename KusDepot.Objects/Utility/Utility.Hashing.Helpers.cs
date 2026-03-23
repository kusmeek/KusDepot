namespace KusDepot.Utilities;

public static partial class Utility
{
    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/field[@name="HashingCycleMarker"]/*'/>*/
    private const Byte HashingCycleMarker = 0xFD;

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/field[@name="HashingNullMarker"]/*'/>*/
    private const Byte HashingNullMarker = 0xFF;

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/field[@name="HashingReferenceMarker"]/*'/>*/
    private const Byte HashingReferenceMarker = 0xFE;

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="CompareByteArrays"]/*'/>*/
    private static Int32 CompareByteArrays(Byte[] a , Byte[] b)
    {
        for(Int32 i = 0; i < Math.Min(a.Length,b.Length); i++)
        {
            if(a[i] != b[i]) { return a[i].CompareTo(b[i]); }
        }
        return a.Length.CompareTo(b.Length);
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="IsUnorderedCollection"]/*'/>*/
    private static Boolean IsUnorderedCollection(Type type)
    {
        if(type.IsGenericType)
        {
            var g = type.GetGenericTypeDefinition(); if(g == typeof(ISet<>) || g == typeof(IReadOnlySet<>) || g == typeof(ConcurrentBag<>)) { return true; }
        }
        foreach(var i in type.GetInterfaces())
        {
            if(i.IsGenericType)
            {
                var g = i.GetGenericTypeDefinition(); if(g == typeof(ISet<>) || g == typeof(IReadOnlySet<>) || g == typeof(ConcurrentBag<>)) { return true; }
            }
        }
        return false;
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="IsUnorderedDictionary"]/*'/>*/
    private static Boolean IsUnorderedDictionary(Type type)
    {
        if(type.IsGenericType)
        {
            var g = type.GetGenericTypeDefinition(); if(g == typeof(IDictionary<,>) || g == typeof(IReadOnlyDictionary<,>) || g == typeof(ConcurrentDictionary<,>)) { return true; }
        }
        foreach(var i in type.GetInterfaces())
        {
            if(i.IsGenericType)
            {
                var g = i.GetGenericTypeDefinition(); if(g == typeof(IDictionary<,>) || g == typeof(IReadOnlyDictionary<,>) || g == typeof(ConcurrentDictionary<,>)) { return true; }
            }
        }
        return false;
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="CreateCycleBytes"]/*'/>*/
    private static Byte[] CreateCycleBytes(Object input)
    {
        var tn = (input.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String();

        return new Byte[] { HashingCycleMarker }.Concat(tn).ToArray();
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="CreateReferenceBytes"]/*'/>*/
    private static Byte[] CreateReferenceBytes(Byte[] input)
    {
        return new Byte[] { HashingReferenceMarker }.Concat(SHA512.HashData(input)).ToArray();
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicHashCodeCore"]/*'/>*/
    private static Int32 GenerateDeterministicHashCodeCore(Object obj , CancellationToken cancel = default)
    {
        return BitConverter.ToInt32(SHA512.HashData(GetDeterministicBytesCore(obj,new HashSet<Object>(ReferenceEqualityComparer.Instance),new Dictionary<Object,Byte[]>(ReferenceEqualityComparer.Instance),cancel)),0);
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GetDeterministicBytesCore"]/*'/>*/
    private static Byte[] GetDeterministicBytesCore(Object? input , HashSet<Object> processing , Dictionary<Object,Byte[]> completed , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        if(input is null) { return new Byte[] { HashingNullMarker }; }

        switch(input)
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
            case Byte[] ba:
            {
                var tn = (ba.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String();
                return tn.Concat(ba).ToArray();
            }
        }

        if(completed.TryGetValue(input,out Byte[]? cached)) { return CreateReferenceBytes(cached); }

        if(processing.Contains(input)) { return CreateCycleBytes(input); }

        processing.Add(input);

        try
        {
            Byte[] output = input switch
            {
                System.Collections.IDictionary dct => GetDictionaryBytes(dct,processing,completed,cancel),
                Array ary => GetArrayBytes(ary,processing,completed,cancel),
                System.Collections.IEnumerable en => GetEnumerableBytes(en,processing,completed,cancel),
                _ => GetObjectBytes(input,processing,completed,cancel)
            };

            completed[input] = output;

            return output;
        }
        finally { processing.Remove(input); }
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GetDictionaryBytes"]/*'/>*/
    private static Byte[] GetDictionaryBytes(System.Collections.IDictionary input , HashSet<Object> processing , Dictionary<Object,Byte[]> completed , CancellationToken cancel = default)
    {
        var tn = (input.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String();
        var e = new List<(Byte[] SortKey,Object? Key,Object? Value)>(input.Count);

        foreach(var k in input.Keys)
        {
            var v = input[k];
            e.Add((GetSortKeyBytes(k,v,cancel),k,v));
        }

        e.Sort((a,b) => CompareByteArrays(a.SortKey,b.SortKey));

        return tn.Concat(e.SelectMany(_ => GetDeterministicBytesCore(_.Key,processing,completed,cancel).Concat(GetDeterministicBytesCore(_.Value,processing,completed,cancel)))).ToArray();
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GetArrayBytes"]/*'/>*/
    private static Byte[] GetArrayBytes(Array input , HashSet<Object> processing , Dictionary<Object,Byte[]> completed , CancellationToken cancel = default)
    {
        var tn = (input.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String();

        return tn.Concat(input.Cast<Object?>().SelectMany(_ => GetDeterministicBytesCore(_,processing,completed,cancel))).ToArray();
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GetEnumerableBytes"]/*'/>*/
    private static Byte[] GetEnumerableBytes(System.Collections.IEnumerable input , HashSet<Object> processing , Dictionary<Object,Byte[]> completed , CancellationToken cancel = default)
    {
        var tn = (input.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String();

        var items = input.Cast<Object?>().ToList();

        if(IsUnorderedCollection(input.GetType()) || IsUnorderedDictionary(input.GetType()))
        {
            items = items.Select(_ => (SortKey:GetSortKeyBytes(_,cancel),Value:_)).OrderBy(_ => _.SortKey,Comparer<Byte[]>.Create(CompareByteArrays)).Select(_ => _.Value).ToList();
        }

        return tn.Concat(items.SelectMany(_ => GetDeterministicBytesCore(_,processing,completed,cancel))).ToArray();
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GetObjectBytes"]/*'/>*/
    private static Byte[] GetObjectBytes(Object input , HashSet<Object> processing , Dictionary<Object,Byte[]> completed , CancellationToken cancel = default)
    {
        var t = input.GetType();
        var ps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name,StringComparer.Ordinal);
        var b = new List<Byte[]>(); b.Add((t.FullName ?? String.Empty).ToByteArrayFromUTF16String());
        var ts = String.Join(";",ps.Select(p => $"{p.Name}:{p.PropertyType.FullName}")); b.Add(Encoding.Unicode.GetBytes(ts));
        foreach(var p in ps)
        {
            cancel.ThrowIfCancellationRequested();

            if(p.GetIndexParameters().Length == 0)
            {
                var v = p.GetValue(input);
                b.Add(p.Name.ToByteArrayFromUTF16String());
                b.Add(GetDeterministicBytesCore(v,processing,completed,cancel));
            }
        }
        return b.SelectMany(_ => _).ToArray();
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GetSortKeyBytes"]/*'/>*/
    private static Byte[] GetSortKeyBytes(Object? input , CancellationToken cancel = default)
    {
        return GetDeterministicBytesCore(input,new HashSet<Object>(ReferenceEqualityComparer.Instance),new Dictionary<Object,Byte[]>(ReferenceEqualityComparer.Instance),cancel);
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GetSortKeyBytesPair"]/*'/>*/
    private static Byte[] GetSortKeyBytes(Object? key , Object? value , CancellationToken cancel = default)
    {
        return GetSortKeyBytes(key,cancel).Concat(GetSortKeyBytes(value,cancel)).ToArray();
    }
}
