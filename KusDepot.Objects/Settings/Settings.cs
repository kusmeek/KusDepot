﻿namespace KusDepot;

/**<include file='Settings.xml' path='Settings/class[@name="Settings"]/main/*'/>*/
public static class Settings
{
    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="BinaryItemValidDataTypes"]/*'/>*/
    public static readonly String?[] BinaryItemValidDataTypes = new String?[]
    {
        DataType.APPLICATION,
        DataType.APPREF,
        DataType.ACCDB,
        DataType.APPX,
        DataType.ARK,
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
        DataType.DKR,
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
        DataType.GZ ,
        DataType.HXS,
        DataType.ICO,
        DataType.IMG,
        DataType.ISO,
        DataType.JAR,
        DataType.JAVA,
        DataType.KCMD,
        DataType.KEY,
        DataType.KDATA,
        DataType.KML,
        DataType.KTOOL,
        DataType.KTRT,
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
        DataType.RAR,
        DataType.RLIB,
        DataType.SCR,
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
    };

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="CodeItemValidDataTypes"]/*'/>*/
    public static readonly String?[] CodeItemValidDataTypes = new String?[]
    {
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
        DataType.KQL,
        DataType.LUA,
        DataType.PS1,
        DataType.PY,
        DataType.RAZOR,
        DataType.RS,
        DataType.SH,
        DataType.SQL,
        DataType.TS,
        DataType.VB,
        DataType.WAT,
        DataType.XAML,
        DataType.XHTML,
        DataType.XML,
        DataType.YAML,
        DataType.YML
    };

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="GuidReferenceItemValidDataTypes"]/*'/>*/
    public static readonly String?[] GuidReferenceItemValidDataTypes = new String?[] { DataType.GUID };

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="MultiMediaItemValidDataTypes"]/*'/>*/
    public static readonly String?[] MultiMediaItemValidDataTypes = new String?[]
    {
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
    };

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/property[@name="NoExceptions"]/*'/>*/
    public static Boolean NoExceptions { get; set; } = true;

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="SyncTime"]/*'/>*/
    public static readonly TimeSpan SyncTime = TimeSpan.FromSeconds(10);

    /**<include file='Settings.xml' path='Settings/class[@name="Settings"]/field[@name="TextItemValidDataTypes"]/*'/>*/
    public static readonly String?[] TextItemValidDataTypes = new String?[]
    {
        DataType.ADML,
        DataType.ADMX,
        DataType.AHK,
        DataType.API,
        DataType.ARK,
        DataType.ARM,
        DataType.ASM,
        DataType.AU3,
        DataType.BAT,
        DataType.BICEP,
        DataType.C,
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
        DataType.KQL,
        DataType.LOG,
        DataType.LUA,
        DataType.MJS,
        DataType.MOF,
        DataType.NFO,
        DataType.PDF,
        DataType.PPTX,
        DataType.PROPS,
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
        DataType.SQL,
        DataType.SQLPROJ,
        DataType.TARGETS,
        DataType.TS,
        DataType.TXT,
        DataType.VB,
        DataType.VSD,
        DataType.WAT,
        DataType.WSDL,
        DataType.XAML,
        DataType.XHTML,
        DataType.XLSX,
        DataType.XML,
        DataType.YAML,
        DataType.YML
    };
}