namespace KusDepot;

/**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/main/*'/>*/
public static class DataValidation
{
    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/field[@name="TextItemValidDataTypes"]/*'/>*/
    public static readonly ImmutableArray<String?> TextItemValidDataTypes;

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/field[@name="CodeItemValidDataTypes"]/*'/>*/
    public static readonly ImmutableArray<String?> CodeItemValidDataTypes;

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/field[@name="BinaryItemValidDataTypes"]/*'/>*/
    public static readonly ImmutableArray<String?> BinaryItemValidDataTypes;

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/field[@name="MultiMediaItemValidDataTypes"]/*'/>*/
    public static readonly ImmutableArray<String?> MultiMediaItemValidDataTypes;

    static DataValidation()
    {
        BinaryItemValidDataTypes = ImmutableArray.Create<String?>(
            DataType.APPLICATION,
            DataType.APPREF,
            DataType.ACCDB,
            DataType.APPX,
            DataType.BACPAC,
            DataType.BAML,
            DataType.BIN,
            DataType.CAB,
            DataType.CAD,
            DataType.CER,
            DataType.CHM,
            DataType.CIL,
            DataType.COM,
            DataType.DACPAC,
            DataType.DAT,
            DataType.DAW,
            DataType.DB,
            DataType.DIAGSESSION,
            DataType.DIT,
            DataType.DLL,
            DataType.DMP,
            DataType.EDB,
            DataType.EOT,
            DataType.EPUB,
            DataType.ETL,
            DataType.EVTX,
            DataType.EXE,
            DataType.EXT,
            DataType.GCDUMP,
            DataType.GZ,
            DataType.HXS,
            DataType.ICO,
            DataType.IMG,
            DataType.ISO,
            DataType.JAR,
            DataType.JAVA,
            DataType.KCMD,
            DataType.KCMDWF,
            DataType.KEY,
            DataType.KDATA,
            DataType.KML,
            DataType.KTOOL,
            DataType.LDF,
            DataType.MDB,
            DataType.MDF,
            DataType.MEM,
            DataType.MSHC,
            DataType.MSIL,
            DataType.MSIX,
            DataType.MUI,
            DataType.NDF,
            DataType.NETTRACE,
            DataType.NONE,
            null,
            DataType.NUPKG,
            DataType.OCIL,
            DataType.OCILZ,
            DataType.OCILN,
            DataType.OCILNZ,
            DataType.OTF,
            DataType.P7B,
            DataType.PAGE,
            DataType.PCAP,
            DataType.PDB,
            DataType.PSD1,
            DataType.PSM1,
            DataType.PSSC,
            DataType.PEM,
            DataType.PFX,
            DataType.PHP,
            DataType.PPKG,
            DataType.RAR,
            DataType.RLIB,
            DataType.SCR,
            DataType.SFPKG,
            DataType.SO,
            DataType.SUO,
            DataType.SYS,
            DataType.TAR,
            DataType.TTF,
            DataType.VHDX,
            DataType.VSIX,
            DataType.WASM,
            DataType.WDBG,
            DataType.WIM,
            DataType.WINMD,
            DataType.WOFF,
            DataType.XAP,
            DataType.ZIP
        );

        CodeItemValidDataTypes = ImmutableArray.Create<String?>(
            DataType.AHK,
            DataType.ARM,
            DataType.ASM,
            DataType.AU3,
            DataType.BAT,
            DataType.BICEP,
            DataType.C,
            DataType.CAKE,
            DataType.CMD,
            DataType.CPP,
            DataType.CS,
            DataType.CSHTML,
            DataType.CSS,
            DataType.CSX,
            DataType.FS,
            DataType.GO,
            DataType.H,
            DataType.HPP,
            DataType.HTM,
            DataType.HTML,
            DataType.JS,
            DataType.JSON,
            DataType.KCMDWF,
            DataType.KQL,
            DataType.LUA,
            DataType.PROMPT,
            DataType.PROTO,
            DataType.PS1,
            DataType.PY,
            DataType.RAZOR,
            DataType.RS,
            DataType.SH,
            DataType.SQL,
            DataType.TS,
            DataType.TSX,
            DataType.VB,
            DataType.WAT,
            DataType.WINGET,
            DataType.WIT,
            DataType.XAML,
            DataType.XHTML,
            DataType.XML,
            DataType.YAML,
            DataType.YML
        );

        TextItemValidDataTypes = ImmutableArray.Create<String?>(
            DataType.ADML,
            DataType.ADMX,
            DataType.AHK,
            DataType.API,
            DataType.ARM,
            DataType.ASM,
            DataType.AU3,
            DataType.BAT,
            DataType.BICEP,
            DataType.C,
            DataType.CR,
            DataType.CAKE,
            DataType.CFG,
            DataType.CMD,
            DataType.CONFIG,
            DataType.CPP,
            DataType.CS,
            DataType.CSHTML,
            DataType.CSPROJ,
            DataType.CSS,
            DataType.CSV,
            DataType.CUE,
            DataType.DAT,
            DataType.DOCX,
            DataType.FS,
            DataType.GEO,
            DataType.GO,
            DataType.GPX,
            DataType.HAR,
            DataType.HTM,
            DataType.HTML,
            DataType.ICS,
            DataType.INF,
            DataType.INI,
            DataType.JS,
            DataType.JSON,
            DataType.KCMDWF,
            DataType.KQL,
            DataType.LOG,
            DataType.LUA,
            DataType.MJS,
            DataType.MOF,
            DataType.MSBUILDPROJ,
            DataType.NFO,
            DataType.PDF,
            DataType.PPTX,
            DataType.PROMPT,
            DataType.PROPS,
            DataType.PROTO,
            DataType.PS1,
            DataType.PY,
            DataType.RAZOR,
            DataType.REG,
            DataType.RESX,
            DataType.RFC,
            DataType.RS,
            DataType.RTF,
            DataType.SFPROJ,
            DataType.SGML,
            DataType.SH,
            DataType.SHFBPROJ,
            DataType.SLN,
            DataType.SLNX,
            DataType.SQL,
            DataType.SQLPROJ,
            DataType.TARGETS,
            DataType.TS,
            DataType.TSX,
            DataType.TXT,
            DataType.VB,
            DataType.VSD,
            DataType.WAPPROJ,
            DataType.WAT,
            DataType.WINGET,
            DataType.WIT,
            DataType.WPRP,
            DataType.WSDL,
            DataType.XAML,
            DataType.XHTML,
            DataType.XLSX,
            DataType.XML,
            DataType.YAML,
            DataType.YML
        );

        MultiMediaItemValidDataTypes = ImmutableArray.Create<String?>(
            DataType.AVI,
            DataType.BAR,
            DataType.BMP,
            DataType.FLAC,
            DataType.GIF,
            DataType.IMAX,
            DataType.JPEG,
            DataType.JPG,
            DataType.MIDI,
            DataType.MOV,
            DataType.M4A,
            DataType.MP3,
            DataType.MP4,
            DataType.MPEG,
            DataType.MPG,
            DataType.OGA,
            DataType.OGV,
            DataType.OGX,
            DataType.PNG,
            DataType.QRC,
            DataType.SVG,
            DataType.TIFF,
            DataType.WAV,
            DataType.WMA,
            DataType.WEBA,
            DataType.WEBM,
            DataType.WEBP,
            DataType.WMV
        );
    }

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/method[@name="IsValidTextDataType"]/*'/>*/
    public static Boolean IsValidTextDataType(String? type) => TextItemValidDataTypes.AsSpan().Contains(type);

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/method[@name="IsValidCodeDataType"]/*'/>*/
    public static Boolean IsValidCodeDataType(String? type) => CodeItemValidDataTypes.AsSpan().Contains(type);

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/method[@name="IsValidBinaryDataType"]/*'/>*/
    public static Boolean IsValidBinaryDataType(String? type) => BinaryItemValidDataTypes.AsSpan().Contains(type);

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/method[@name="IsValidMultiMediaDataType"]/*'/>*/
    public static Boolean IsValidMultiMediaDataType(String? type) => MultiMediaItemValidDataTypes.AsSpan().Contains(type);

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/method[@name="IsDataTypeChangeAllowed"]/*'/>*/
    public static Boolean IsDataTypeChangeAllowed(DataItem? item)
    {
        if(item is null) { return false; }

        switch(item)
        {
            case KeySet:            { return false; }
            case MSBuildItem:       { return false; }
            case GuidReferenceItem: { return false; }
            default:                { return true; }
        }
    }

    /**<include file='DataValidation.xml' path='DataValidation/class[@name="DataValidation"]/method[@name="IsValid"]/*'/>*/
    public static Boolean IsValid(DataItem? item , String? type)
    {
        if(item is null) { return false; }

        return item switch
        {
            DataSetItem       => true,
            GenericItem       => true,
            TextItem          => IsValidTextDataType(type),
            CodeItem          => IsValidCodeDataType(type),
            BinaryItem        => IsValidBinaryDataType(type),
            MultiMediaItem    => IsValidMultiMediaDataType(type),
            MSBuildItem       => String.Equals(type,DataType.MSB,Ordinal),
            GuidReferenceItem => String.Equals(type,DataType.GUID,Ordinal),
            KeySet            => String.Equals(type,DataType.KEYSET,Ordinal),
            _ => false
        };
    }
}