namespace KusDepot.Utilities;

internal sealed partial class CanonicalHashGraph
{
    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="TryEncodeScalar"]/*'/>*/
    private static Boolean TryEncodeScalar(Object input , out Byte[]? encoded)
    {
        Type type = input.GetType();

        if(type.IsEnum)
        {
            Byte[] typeName = StrictUtf8.GetBytes(GetCanonicalTypeName(type));

            Byte[] value = EncodeEnumValue(input,Enum.GetUnderlyingType(type));

            ArrayBufferWriter<Byte> payload = new(); WriteLengthPrefixed(payload,typeName); WriteLengthPrefixed(payload,value);

            encoded = EncodeAtom(HashGraphKind.EnumAtom,payload.WrittenSpan); return true;
        }

        switch(input)
        {
            case Byte value: encoded = EncodeAtom(HashGraphKind.ByteAtom,[value]); return true;
            case SByte value: encoded = EncodeAtom(HashGraphKind.SByteAtom,[unchecked((Byte)value)]); return true;
            case Boolean value: encoded = EncodeAtom(HashGraphKind.BooleanAtom,[value ? (Byte)1 : (Byte)0]); return true;
            case Char value: encoded = EncodeAtom(HashGraphKind.CharAtom,EncodeUInt16(value)); return true;
            case Int16 value: encoded = EncodeAtom(HashGraphKind.Int16Atom,EncodeInt16(value)); return true;
            case UInt16 value: encoded = EncodeAtom(HashGraphKind.UInt16Atom,EncodeUInt16(value)); return true;
            case Int32 value: encoded = EncodeAtom(HashGraphKind.Int32Atom,EncodeInt32(value)); return true;
            case UInt32 value: encoded = EncodeAtom(HashGraphKind.UInt32Atom,EncodeUInt32(value)); return true;
            case Int64 value: encoded = EncodeAtom(HashGraphKind.Int64Atom,EncodeInt64(value)); return true;
            case UInt64 value: encoded = EncodeAtom(HashGraphKind.UInt64Atom,EncodeUInt64(value)); return true;
            case IntPtr value: encoded = EncodeAtom(HashGraphKind.IntPtrAtom,EncodeInt64(value.ToInt64())); return true;
            case UIntPtr value: encoded = EncodeAtom(HashGraphKind.UIntPtrAtom,EncodeUInt64(value.ToUInt64())); return true;
            case Half value: encoded = EncodeAtom(HashGraphKind.HalfAtom,EncodeUInt16(BitConverter.HalfToUInt16Bits(value))); return true;
            case Single value: encoded = EncodeAtom(HashGraphKind.SingleAtom,EncodeUInt32(unchecked((UInt32)BitConverter.SingleToInt32Bits(value)))); return true;
            case Double value: encoded = EncodeAtom(HashGraphKind.DoubleAtom,EncodeUInt64(unchecked((UInt64)BitConverter.DoubleToInt64Bits(value)))); return true;
            case Decimal value: encoded = EncodeAtom(HashGraphKind.DecimalAtom,EncodeInt32Array(Decimal.GetBits(value))); return true;
            case BigInteger value: encoded = EncodeAtom(HashGraphKind.BigIntegerAtom,value.ToByteArray(isUnsigned:false,isBigEndian:true)); return true;
            case Guid value: encoded = EncodeAtom(HashGraphKind.GuidAtom,GuidToCanonicalBytes(value)); return true;
            case String value: encoded = EncodeAtom(HashGraphKind.StringAtom,StrictUtf8.GetBytes(value)); return true;

            case DateTime value:
            {
                ArrayBufferWriter<Byte> payload = new(); WriteInt64(payload,value.Ticks); WriteByte(payload,(Byte)value.Kind);

                encoded = EncodeAtom(HashGraphKind.DateTimeAtom,payload.WrittenSpan); return true;
            }
            case DateTimeOffset value:
            {
                ArrayBufferWriter<Byte> payload = new(); WriteInt64(payload,value.Ticks); WriteInt64(payload,value.Offset.Ticks);

                encoded = EncodeAtom(HashGraphKind.DateTimeOffsetAtom,payload.WrittenSpan); return true;
            }

            case TimeSpan value: encoded = EncodeAtom(HashGraphKind.TimeSpanAtom,EncodeInt64(value.Ticks)); return true;

            case Version value: encoded = EncodeAtom(HashGraphKind.VersionAtom,EncodeInt32Array([value.Major,value.Minor,value.Build,value.Revision])); return true;

            case Uri value:
            {
                ArrayBufferWriter<Byte> payload = new(); WriteByte(payload,value.IsAbsoluteUri ? (Byte)1 : (Byte)0); WriteLengthPrefixed(payload,StrictUtf8.GetBytes(value.OriginalString));

                encoded = EncodeAtom(HashGraphKind.UriAtom,payload.WrittenSpan); return true;
            }
            case Type value: encoded = EncodeAtom(HashGraphKind.TypeAtom,StrictUtf8.GetBytes(GetCanonicalTypeName(value))); return true;

            default: encoded = null; return false;
        }
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeEnumValue"]/*'/>*/
    private static Byte[] EncodeEnumValue(Object value , Type underlyingType)
    {
        TypeCode code = Type.GetTypeCode(underlyingType);

        return code switch
        {
            TypeCode.SByte => [unchecked((Byte)Convert.ToSByte(value,CultureInfo.InvariantCulture))],

            TypeCode.Byte => [Convert.ToByte(value,CultureInfo.InvariantCulture)],

            TypeCode.Int16 => EncodeInt16(Convert.ToInt16(value,CultureInfo.InvariantCulture)),

            TypeCode.UInt16 => EncodeUInt16(Convert.ToUInt16(value,CultureInfo.InvariantCulture)),

            TypeCode.Int32 => EncodeInt32(Convert.ToInt32(value,CultureInfo.InvariantCulture)),

            TypeCode.UInt32 => EncodeUInt32(Convert.ToUInt32(value,CultureInfo.InvariantCulture)),

            TypeCode.Int64 => EncodeInt64(Convert.ToInt64(value,CultureInfo.InvariantCulture)),

            TypeCode.UInt64 => EncodeUInt64(Convert.ToUInt64(value,CultureInfo.InvariantCulture)),

            _ => throw new InvalidOperationException("Unsupported enum underlying type.")
        };
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="GetCanonicalTypeName"]/*'/>*/
    private static String GetCanonicalTypeName(Type type) => GetCanonicalTypeNameCore(type,true);

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="GetCanonicalTypeNameCore"]/*'/>*/
    private static String GetCanonicalTypeNameCore(Type type , Boolean includeGenericParameterOwner)
    {
        if(type.IsFunctionPointer) { throw new InvalidOperationException("Function-pointer types are not supported in deterministic hashing."); }

        if(type.IsArray) { return $"A{type.GetArrayRank()}:{(type.IsSZArray ? 1 : 0)}{FrameTypeName(GetCanonicalTypeNameCore(type.GetElementType()!,includeGenericParameterOwner))}"; }

        if(type.IsPointer) { return $"P{FrameTypeName(GetCanonicalTypeNameCore(type.GetElementType()!,includeGenericParameterOwner))}"; }

        if(type.IsByRef) { return $"R{FrameTypeName(GetCanonicalTypeNameCore(type.GetElementType()!,includeGenericParameterOwner))}"; }

        if(type.IsGenericParameter)
        {
            String kind = type.DeclaringMethod is null ? "!" : "!!";

            String owner = includeGenericParameterOwner ? GetGenericParameterOwnerName(type) : String.Empty;

            return $"{kind}{type.GenericParameterPosition}:{FrameTypeName(type.Name)}{FrameTypeName(owner)}";
        }

        if(type.IsGenericType)
        {
            Type definition = type.GetGenericTypeDefinition();

            return GetCanonicalNamedTypeName(definition,type.GetGenericArguments(),includeGenericParameterOwner);
        }

        return GetCanonicalNamedTypeName(type,Type.EmptyTypes,includeGenericParameterOwner);
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="GetCanonicalNamedTypeName"]/*'/>*/
    private static String GetCanonicalNamedTypeName(Type definition , Type[] arguments , Boolean includeGenericParameterOwner)
    {
        Type? declaringType = definition.DeclaringType;

        Int32 parentArgumentCount = declaringType?.GetGenericArguments().Length ?? 0;

        String parentName = declaringType is null ? String.Empty : GetCanonicalNamedTypeName(declaringType,arguments.Take(parentArgumentCount).ToArray(),includeGenericParameterOwner);

        Int32 totalArgumentCount = definition.IsGenericType ? definition.GetGenericArguments().Length : 0;

        Int32 localArgumentCount = totalArgumentCount-parentArgumentCount;

        String[] localArguments = arguments.Skip(parentArgumentCount).Take(localArgumentCount).Select(argument => FrameTypeName(GetCanonicalTypeNameCore(argument,includeGenericParameterOwner))).ToArray();

        String namespaceName = declaringType is null ? definition.Namespace ?? String.Empty : String.Empty;

        return $"N{FrameTypeName(namespaceName)}{FrameTypeName(RemoveGenericArity(definition.Name))}{FrameTypeName(parentName)}{localArgumentCount}:{String.Concat(localArguments)}";
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="GetGenericParameterOwnerName"]/*'/>*/
    private static String GetGenericParameterOwnerName(Type type)
    {
        if(type.DeclaringMethod is MethodBase method) { return GetCanonicalMethodOwnerName(method); }

        return GetCanonicalTypeNameCore(type.DeclaringType!,false);
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="GetCanonicalMethodOwnerName"]/*'/>*/
    private static String GetCanonicalMethodOwnerName(MethodBase method)
    {
        Type declaringType = method.DeclaringType!;

        String parameterTypes = String.Concat(method.GetParameters().Select(parameter => FrameTypeName(GetCanonicalTypeNameCore(parameter.ParameterType,false))));

        String returnType = method is MethodInfo methodInfo ? GetCanonicalTypeNameCore(methodInfo.ReturnType,false) : String.Empty;

        return $"M{FrameTypeName(GetCanonicalTypeNameCore(declaringType,false))}{FrameTypeName(method.Name)}{method.GetGenericArguments().Length}:{method.GetParameters().Length}:{parameterTypes}{FrameTypeName(returnType)}";
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="RemoveGenericArity"]/*'/>*/
    private static String RemoveGenericArity(String name)
    {
        Int32 arityMarker = name.LastIndexOf('`');

        return arityMarker >= 0 && Int32.TryParse(name.AsSpan(arityMarker+1),NumberStyles.None,CultureInfo.InvariantCulture,out _) ? name[..arityMarker] : name;
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="FrameTypeName"]/*'/>*/
    private static String FrameTypeName(String value) => $"{value.Length}:{value}";

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="IsGraphUnorderedCollection"]/*'/>*/
    private static Boolean IsGraphUnorderedCollection(Type type)
    {
        if(type.IsGenericType)
        {
            Type definition = type.GetGenericTypeDefinition();

            if(definition == typeof(ISet<>) || definition == typeof(IReadOnlySet<>) || definition == typeof(ConcurrentBag<>)) { return true; }
        }
        if(type.GetInterfaces().Any(@interface => @interface.IsGenericType && (@interface.GetGenericTypeDefinition() == typeof(ISet<>) || @interface.GetGenericTypeDefinition() == typeof(IReadOnlySet<>)))) { return true; }

        for(Type? current = type; current is not null; current = current.BaseType)
        {
            if(current.IsGenericType && current.GetGenericTypeDefinition() == typeof(ConcurrentBag<>)) { return true; }
        }
        return false;
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="IsGraphUnorderedDictionary"]/*'/>*/
    private static Boolean IsGraphUnorderedDictionary(Type type)
    {
        if(type.IsGenericType)
        {
            Type definition = type.GetGenericTypeDefinition();

            if(definition == typeof(IDictionary<,>) || definition == typeof(IReadOnlyDictionary<,>) || definition == typeof(ConcurrentDictionary<,>)) { return true; }
        }
        return type.GetInterfaces().Any(@interface => @interface.IsGenericType && (@interface.GetGenericTypeDefinition() == typeof(IDictionary<,>) || @interface.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>) || @interface.GetGenericTypeDefinition() == typeof(ConcurrentDictionary<,>)));
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="CreateNodeLabel"]/*'/>*/
    private static Byte[] CreateNodeLabel(Byte kind , String typeName , ReadOnlySpan<Byte> detail)
    {
        ArrayBufferWriter<Byte> label = new(); WriteByte(label,kind); WriteLengthPrefixed(label,StrictUtf8.GetBytes(typeName)); WriteLengthPrefixed(label,detail); return label.WrittenSpan.ToArray();
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="CreateEdgeLabel"]/*'/>*/
    private static Byte[] CreateEdgeLabel(Byte kind , ReadOnlySpan<Byte> name)
    {
        ArrayBufferWriter<Byte> label = new(); WriteByte(label,kind); WriteLengthPrefixed(label,name); return label.WrittenSpan.ToArray();
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeAtom"]/*'/>*/
    private static Byte[] EncodeAtom(Byte kind , ReadOnlySpan<Byte> value) => CreateFramedValue(kind,value);

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeDataItemIdentity"]/*'/>*/
    private static Byte[] EncodeDataItemIdentity(Type type , Guid? id)
    {
        ArrayBufferWriter<Byte> payload = new();

        WriteLengthPrefixed(payload,StrictUtf8.GetBytes(GetCanonicalTypeName(type)));

        WriteLengthPrefixed(payload,id.HasValue ? GuidToCanonicalBytes(id.Value) : Array.Empty<Byte>());

        return EncodeAtom(HashGraphKind.DataItemIdentityAtom,payload.WrittenSpan);
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="CreateFramedValue"]/*'/>*/
    private static Byte[] CreateFramedValue(Byte kind , ReadOnlySpan<Byte> value)
    {
        ArrayBufferWriter<Byte> output = new(); WriteByte(output,kind); WriteLengthPrefixed(output,value); return output.WrittenSpan.ToArray();
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="GuidToCanonicalBytes"]/*'/>*/
    private static Byte[] GuidToCanonicalBytes(Guid value)
    {
        Byte[] output = new Byte[16];

        value.TryWriteBytes(output,bigEndian:true,out _);

        return output;
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeInt16"]/*'/>*/
    private static Byte[] EncodeInt16(Int16 value) { Byte[] output = new Byte[sizeof(Int16)]; WriteInt16LittleEndian(output,value); return output; }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeUInt16"]/*'/>*/
    private static Byte[] EncodeUInt16(UInt16 value) { Byte[] output = new Byte[sizeof(UInt16)]; WriteUInt16LittleEndian(output,value); return output; }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeInt32"]/*'/>*/
    private static Byte[] EncodeInt32(Int32 value) { Byte[] output = new Byte[sizeof(Int32)]; WriteInt32LittleEndian(output,value); return output; }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeUInt32"]/*'/>*/
    private static Byte[] EncodeUInt32(UInt32 value) { Byte[] output = new Byte[sizeof(UInt32)]; WriteUInt32LittleEndian(output,value); return output; }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeInt64"]/*'/>*/
    private static Byte[] EncodeInt64(Int64 value) { Byte[] output = new Byte[sizeof(Int64)]; WriteInt64LittleEndian(output,value); return output; }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeUInt64"]/*'/>*/
    private static Byte[] EncodeUInt64(UInt64 value) { Byte[] output = new Byte[sizeof(UInt64)]; WriteUInt64LittleEndian(output,value); return output; }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="EncodeInt32Array"]/*'/>*/
    private static Byte[] EncodeInt32Array(ReadOnlySpan<Int32> values)
    {
        Byte[] output = new Byte[values.Length*sizeof(Int32)];

        for(Int32 index = 0; index < values.Length; index++) { WriteInt32LittleEndian(output.AsSpan(index*sizeof(Int32)),values[index]); }

        return output;
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="WriteByte"]/*'/>*/
    private static void WriteByte(ArrayBufferWriter<Byte> output , Byte value) { output.GetSpan(1)[0] = value; output.Advance(1); }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="WriteInt32"]/*'/>*/
    private static void WriteInt32(ArrayBufferWriter<Byte> output , Int32 value) { Span<Byte> span = output.GetSpan(sizeof(Int32)); WriteInt32LittleEndian(span,value); output.Advance(sizeof(Int32)); }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="WriteInt64"]/*'/>*/
    private static void WriteInt64(ArrayBufferWriter<Byte> output , Int64 value) { Span<Byte> span = output.GetSpan(sizeof(Int64)); WriteInt64LittleEndian(span,value); output.Advance(sizeof(Int64)); }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="WriteLengthPrefixed"]/*'/>*/
    private static void WriteLengthPrefixed(ArrayBufferWriter<Byte> output , ReadOnlySpan<Byte> value) { WriteInt32(output,value.Length); WriteRaw(output,value); }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="WriteRaw"]/*'/>*/
    private static void WriteRaw(ArrayBufferWriter<Byte> output , ReadOnlySpan<Byte> value) { if(value.IsEmpty) { return; } value.CopyTo(output.GetSpan(value.Length)); output.Advance(value.Length); }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/method[@name="CompareBytes"]/*'/>*/
    private static Int32 CompareBytes(Byte[] left , Byte[] right)
    {
        Int32 length = Math.Min(left.Length,right.Length);

        for(Int32 index = 0; index < length; index++) { if(left[index] != right[index]) { return left[index].CompareTo(right[index]); } }

        return left.Length.CompareTo(right.Length);
    }
}
