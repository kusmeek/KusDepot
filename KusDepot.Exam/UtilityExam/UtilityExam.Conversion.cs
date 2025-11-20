namespace KusDepot.Exams;

[TestFixture]
public partial class UtilityExam
{
    [Test]
    public void TryGetStringFromByteArray()
    {
        String original = "Hello, this is a test string.";

        Byte[] utf8Bytes = Encoding.UTF8.GetBytes(original);
        Check.That(utf8Bytes.TryGetStringFromByteArray()).IsEqualTo(original);

        Byte[] utf16Bytes = Encoding.Unicode.GetBytes(original);
        Check.That(utf16Bytes.TryGetStringFromByteArray()).IsEqualTo(original);

        Byte[] utf32Bytes = Encoding.UTF32.GetBytes(original);
        Check.That(utf32Bytes.TryGetStringFromByteArray()).IsEqualTo(original);

        Check.That(((Byte[]?)null).TryGetStringFromByteArray()).IsEqualTo(String.Empty);
        Check.That(Array.Empty<Byte>().TryGetStringFromByteArray()).IsEqualTo(String.Empty);

        Byte[] invalidBytes = new Byte[] { 0xFF, 0xFE, 0xFD };
        Check.ThatCode(() => invalidBytes.TryGetStringFromByteArray()).Throws<FormatException>();
    }

    [Test]
    public void String_And_ByteArray_Conversion_RoundTrips()
    {
        String originalString = "Hello, this is a UTF-16 string!";
        Byte[] utf16Bytes = originalString.ToByteArrayFromUTF16String();
        String roundTripString = utf16Bytes.ToUTF16StringFromByteArray();
        Check.That(roundTripString).IsEqualTo(originalString);

        Byte[] originalBytes = RandomBytes(256);
        String base64String = originalBytes.ToBase64FromByteArray();
        Byte[] roundTripBytes = base64String.ToByteArrayFromBase64();
        Check.That(roundTripBytes).IsEqualTo(originalBytes);

        Int32 number = 12345;
        String invariantString = number.ToStringInvariant()!;
        Check.That(invariantString).IsEqualTo("12345");
    }
}