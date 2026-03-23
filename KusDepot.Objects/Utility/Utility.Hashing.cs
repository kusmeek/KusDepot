namespace KusDepot.Utilities;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicHashCode"]/*'/>*/
    public static Int32 GenerateDeterministicHashCode(Object? obj)
    {
        if(obj is null) { return unchecked((Int32)0xBAADF00D); }

        try
        {
            return GenerateDeterministicHashCodeCore(obj);
        }
        catch ( StackOverflowException )
        {
            return BitConverter.ToInt32(SHA512.HashData((obj.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String()),0);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GenerateDeterministicHashCodeFail); if(NoExceptions) { return 0; } throw; }
    }

    /**<include file='Utility.Hashing.xml' path='Utility/class[@name="Utility"]/method[@name="GenerateDeterministicHashCodeAsync"]/*'/>*/
    public static Task<Int32> GenerateDeterministicHashCodeAsync(Object? obj , CancellationToken cancel = default)
    {
        if(obj is null) { return Task.FromResult(unchecked((Int32)0xBAADF00D)); }

        cancel.ThrowIfCancellationRequested();

        try
        {
            return Task.FromResult(GenerateDeterministicHashCodeCore(obj,cancel));
        }
        catch ( StackOverflowException )
        {
            return Task.FromResult(BitConverter.ToInt32(SHA512.HashData((obj.GetType().FullName ?? String.Empty).ToByteArrayFromUTF16String()),0));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GenerateDeterministicHashCodeFail); if(NoExceptions) { return Task.FromResult(0); } throw; }
    }
}