namespace KusDepot.Utilities;

internal sealed partial class CanonicalHashGraph
{
    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphNode"]/main/*'/>*/
    private sealed class HashGraphNode
    {
        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphNode"]/field[@name="Label"]/*'/>*/
        public Byte[] Label;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphNode"]/field[@name="Edges"]/*'/>*/
        public List<HashGraphEdge> Edges = new();

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphNode"]/constructor[@name="HashGraphNode"]/*'/>*/
        public HashGraphNode(Byte[] label) { this.Label = label; }
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphEdge"]/main/*'/>*/
    private sealed class HashGraphEdge
    {
        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphEdge"]/field[@name="Label"]/*'/>*/
        public Byte[] Label;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphEdge"]/field[@name="Value"]/*'/>*/
        public HashGraphValue Value;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphEdge"]/constructor[@name="HashGraphEdge"]/*'/>*/
        public HashGraphEdge(Byte[] label , HashGraphValue value) { this.Label = label; this.Value = value; }
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphValue"]/main/*'/>*/
    private sealed class HashGraphValue
    {
        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphValue"]/field[@name="IsNode"]/*'/>*/
        public Boolean IsNode;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphValue"]/field[@name="NodeIndex"]/*'/>*/
        public Int32 NodeIndex;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphValue"]/field[@name="Atom"]/*'/>*/
        public Byte[]? Atom;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphValue"]/constructor[@name="HashGraphValue"]/*'/>*/
        private HashGraphValue(Boolean isNode , Int32 nodeIndex , Byte[]? atom) { this.IsNode = isNode; this.NodeIndex = nodeIndex; this.Atom = atom; }

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphValue"]/method[@name="FromNode"]/*'/>*/
        public static HashGraphValue FromNode(Int32 nodeIndex) => new(true,nodeIndex,null);

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphValue"]/method[@name="FromAtom"]/*'/>*/
        public static HashGraphValue FromAtom(Byte[] atom) => new(false,-1,atom);
    }

    /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/main/*'/>*/
    private static class HashGraphKind
    {
        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="PropertyEdge"]/*'/>*/
        public const Byte PropertyEdge = 0x01;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="DictionaryFieldEdge"]/*'/>*/
        public const Byte DictionaryFieldEdge = 0x02;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="DictionaryEntryEdge"]/*'/>*/
        public const Byte DictionaryEntryEdge = 0x03;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="OrderedElementEdge"]/*'/>*/
        public const Byte OrderedElementEdge = 0x04;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="UnorderedElementEdge"]/*'/>*/
        public const Byte UnorderedElementEdge = 0x05;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="ByteArrayPayloadEdge"]/*'/>*/
        public const Byte ByteArrayPayloadEdge = 0x06;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="NullAtom"]/*'/>*/
        public const Byte NullAtom = 0x20;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="EnumAtom"]/*'/>*/
        public const Byte EnumAtom = 0x21;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="ByteAtom"]/*'/>*/
        public const Byte ByteAtom = 0x22;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="SByteAtom"]/*'/>*/
        public const Byte SByteAtom = 0x23;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="BooleanAtom"]/*'/>*/
        public const Byte BooleanAtom = 0x24;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="CharAtom"]/*'/>*/
        public const Byte CharAtom = 0x25;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="Int16Atom"]/*'/>*/
        public const Byte Int16Atom = 0x26;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="UInt16Atom"]/*'/>*/
        public const Byte UInt16Atom = 0x27;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="Int32Atom"]/*'/>*/
        public const Byte Int32Atom = 0x28;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="UInt32Atom"]/*'/>*/
        public const Byte UInt32Atom = 0x29;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="Int64Atom"]/*'/>*/
        public const Byte Int64Atom = 0x2A;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="UInt64Atom"]/*'/>*/
        public const Byte UInt64Atom = 0x2B;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="IntPtrAtom"]/*'/>*/
        public const Byte IntPtrAtom = 0x2C;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="UIntPtrAtom"]/*'/>*/
        public const Byte UIntPtrAtom = 0x2D;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="HalfAtom"]/*'/>*/
        public const Byte HalfAtom = 0x2E;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="SingleAtom"]/*'/>*/
        public const Byte SingleAtom = 0x2F;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="DoubleAtom"]/*'/>*/
        public const Byte DoubleAtom = 0x30;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="DecimalAtom"]/*'/>*/
        public const Byte DecimalAtom = 0x31;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="BigIntegerAtom"]/*'/>*/
        public const Byte BigIntegerAtom = 0x32;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="GuidAtom"]/*'/>*/
        public const Byte GuidAtom = 0x33;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="StringAtom"]/*'/>*/
        public const Byte StringAtom = 0x34;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="DateTimeAtom"]/*'/>*/
        public const Byte DateTimeAtom = 0x35;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="DateTimeOffsetAtom"]/*'/>*/
        public const Byte DateTimeOffsetAtom = 0x36;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="TimeSpanAtom"]/*'/>*/
        public const Byte TimeSpanAtom = 0x37;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="VersionAtom"]/*'/>*/
        public const Byte VersionAtom = 0x38;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="UriAtom"]/*'/>*/
        public const Byte UriAtom = 0x39;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="DataItemIdentityAtom"]/*'/>*/
        public const Byte DataItemIdentityAtom = 0x3A;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="ByteArrayAtom"]/*'/>*/
        public const Byte ByteArrayAtom = 0x3B;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="TypeAtom"]/*'/>*/
        public const Byte TypeAtom = 0x3C;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="ObjectNode"]/*'/>*/
        public const Byte ObjectNode = 0x40;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="DictionaryEntryNode"]/*'/>*/
        public const Byte DictionaryEntryNode = 0x41;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="ArrayNode"]/*'/>*/
        public const Byte ArrayNode = 0x42;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="ByteArrayNode"]/*'/>*/
        public const Byte ByteArrayNode = 0x43;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="ValueAtom"]/*'/>*/
        public const Byte ValueAtom = 0x60;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="ValueNodeReference"]/*'/>*/
        public const Byte ValueNodeReference = 0x61;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="IncidenceNodeReference"]/*'/>*/
        public const Byte IncidenceNodeReference = 0x70;

        /**<include file='CanonicalHashGraph.xml' path='CanonicalHashGraph/class[@name="CanonicalHashGraph"]/class[@name="HashGraphKind"]/field[@name="IncidenceAtom"]/*'/>*/
        public const Byte IncidenceAtom = 0x71;
    }
}
