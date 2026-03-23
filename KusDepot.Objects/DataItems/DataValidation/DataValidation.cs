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
            DataTypes.APPLICATION,
            DataTypes.APPREF,
            DataTypes.ACCDB,
            DataTypes.APPX,
            DataTypes.BACPAC,
            DataTypes.BAML,
            DataTypes.BIN,
            DataTypes.CAB,
            DataTypes.CAD,
            DataTypes.CER,
            DataTypes.CHM,
            DataTypes.CIL,
            DataTypes.COM,
            DataTypes.DACPAC,
            DataTypes.DAT,
            DataTypes.DAW,
            DataTypes.DB,
            DataTypes.DIAGSESSION,
            DataTypes.DIT,
            DataTypes.DLL,
            DataTypes.DMP,
            DataTypes.EDB,
            DataTypes.EOT,
            DataTypes.EPUB,
            DataTypes.ETL,
            DataTypes.EVTX,
            DataTypes.EXE,
            DataTypes.EXT,
            DataTypes.GCDUMP,
            DataTypes.GZ,
            DataTypes.HXS,
            DataTypes.ICO,
            DataTypes.IMG,
            DataTypes.ISO,
            DataTypes.JAR,
            DataTypes.JAVA,
            DataTypes.KCMD,
            DataTypes.KCMDWF,
            DataTypes.KEY,
            DataTypes.KDATA,
            DataTypes.KML,
            DataTypes.KTOOL,
            DataTypes.LDF,
            DataTypes.MDB,
            DataTypes.MDF,
            DataTypes.MEM,
            DataTypes.MSHC,
            DataTypes.MSIL,
            DataTypes.MSIX,
            DataTypes.MUI,
            DataTypes.NDF,
            DataTypes.NETTRACE,
            DataTypes.NONE,
            null,
            DataTypes.NUPKG,
            DataTypes.OCIL,
            DataTypes.OCILZ,
            DataTypes.OCILN,
            DataTypes.OCILNZ,
            DataTypes.OTF,
            DataTypes.P7B,
            DataTypes.PAGE,
            DataTypes.PCAP,
            DataTypes.PDB,
            DataTypes.PSD1,
            DataTypes.PSM1,
            DataTypes.PSSC,
            DataTypes.PEM,
            DataTypes.PFX,
            DataTypes.PHP,
            DataTypes.PPKG,
            DataTypes.RAR,
            DataTypes.RLIB,
            DataTypes.SCR,
            DataTypes.SFPKG,
            DataTypes.SO,
            DataTypes.SUO,
            DataTypes.SYS,
            DataTypes.TAR,
            DataTypes.TTF,
            DataTypes.VHDX,
            DataTypes.VSIX,
            DataTypes.WASM,
            DataTypes.WDBG,
            DataTypes.WIM,
            DataTypes.WINMD,
            DataTypes.WOFF,
            DataTypes.XAP,
            DataTypes.ZIP
        );

        CodeItemValidDataTypes = ImmutableArray.Create<String?>(
            DataTypes.AHK,
            DataTypes.ARM,
            DataTypes.ASM,
            DataTypes.AU3,
            DataTypes.BAT,
            DataTypes.BICEP,
            DataTypes.C,
            DataTypes.CAKE,
            DataTypes.CMD,
            DataTypes.CPP,
            DataTypes.CS,
            DataTypes.CSHTML,
            DataTypes.CSS,
            DataTypes.CSX,
            DataTypes.FS,
            DataTypes.GO,
            DataTypes.H,
            DataTypes.HPP,
            DataTypes.HTM,
            DataTypes.HTML,
            DataTypes.JS,
            DataTypes.JSON,
            DataTypes.KCMDWF,
            DataTypes.KQL,
            DataTypes.LUA,
            DataTypes.PROMPT,
            DataTypes.PROTO,
            DataTypes.PS1,
            DataTypes.PY,
            DataTypes.RAZOR,
            DataTypes.RS,
            DataTypes.SH,
            DataTypes.SQL,
            DataTypes.TS,
            DataTypes.TSX,
            DataTypes.VB,
            DataTypes.WAT,
            DataTypes.WINGET,
            DataTypes.WIT,
            DataTypes.XAML,
            DataTypes.XHTML,
            DataTypes.XML,
            DataTypes.YAML,
            DataTypes.YML
        );

        TextItemValidDataTypes = ImmutableArray.Create<String?>(
            DataTypes.ADML,
            DataTypes.ADMX,
            DataTypes.AHK,
            DataTypes.API,
            DataTypes.ARM,
            DataTypes.ASM,
            DataTypes.AU3,
            DataTypes.BAT,
            DataTypes.BICEP,
            DataTypes.C,
            DataTypes.CR,
            DataTypes.CAKE,
            DataTypes.CFG,
            DataTypes.CMD,
            DataTypes.CONFIG,
            DataTypes.CPP,
            DataTypes.CS,
            DataTypes.CSHTML,
            DataTypes.CSPROJ,
            DataTypes.CSS,
            DataTypes.CSV,
            DataTypes.CUE,
            DataTypes.DAT,
            DataTypes.DOCX,
            DataTypes.FS,
            DataTypes.GEO,
            DataTypes.GO,
            DataTypes.GPX,
            DataTypes.HAR,
            DataTypes.HTM,
            DataTypes.HTML,
            DataTypes.ICS,
            DataTypes.INF,
            DataTypes.INI,
            DataTypes.JS,
            DataTypes.JSON,
            DataTypes.KCMDWF,
            DataTypes.KQL,
            DataTypes.LOG,
            DataTypes.LUA,
            DataTypes.MJS,
            DataTypes.MOF,
            DataTypes.MSBUILDPROJ,
            DataTypes.NFO,
            DataTypes.PDF,
            DataTypes.PPTX,
            DataTypes.PROMPT,
            DataTypes.PROPS,
            DataTypes.PROTO,
            DataTypes.PS1,
            DataTypes.PY,
            DataTypes.RAZOR,
            DataTypes.REG,
            DataTypes.RESX,
            DataTypes.RFC,
            DataTypes.RS,
            DataTypes.RTF,
            DataTypes.SFPROJ,
            DataTypes.SGML,
            DataTypes.SH,
            DataTypes.SHFBPROJ,
            DataTypes.SLN,
            DataTypes.SLNX,
            DataTypes.SQL,
            DataTypes.SQLPROJ,
            DataTypes.TARGETS,
            DataTypes.TS,
            DataTypes.TSX,
            DataTypes.TXT,
            DataTypes.VB,
            DataTypes.VSD,
            DataTypes.WAPPROJ,
            DataTypes.WAT,
            DataTypes.WINGET,
            DataTypes.WIT,
            DataTypes.WPRP,
            DataTypes.WSDL,
            DataTypes.XAML,
            DataTypes.XHTML,
            DataTypes.XLSX,
            DataTypes.XML,
            DataTypes.YAML,
            DataTypes.YML
        );

        MultiMediaItemValidDataTypes = ImmutableArray.Create<String?>(
            DataTypes.AVI,
            DataTypes.BAR,
            DataTypes.BMP,
            DataTypes.FLAC,
            DataTypes.GIF,
            DataTypes.IMAX,
            DataTypes.JPEG,
            DataTypes.JPG,
            DataTypes.MIDI,
            DataTypes.MOV,
            DataTypes.M4A,
            DataTypes.MP3,
            DataTypes.MP4,
            DataTypes.MPEG,
            DataTypes.MPG,
            DataTypes.OGA,
            DataTypes.OGV,
            DataTypes.OGX,
            DataTypes.PNG,
            DataTypes.QRC,
            DataTypes.SVG,
            DataTypes.TIFF,
            DataTypes.WAV,
            DataTypes.WMA,
            DataTypes.WEBA,
            DataTypes.WEBM,
            DataTypes.WEBP,
            DataTypes.WMV
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
            MSBuildItem       => String.Equals(type,DataTypes.MSB,Ordinal),
            GuidReferenceItem => String.Equals(type,DataTypes.GUID,Ordinal),
            KeySet            => String.Equals(type,DataTypes.KEYSET,Ordinal),
            _ => false
        };
    }
}