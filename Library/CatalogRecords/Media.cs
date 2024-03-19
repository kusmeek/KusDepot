namespace KusDepot.Data;

/**<include file='Media.xml' path='Media/record[@name="Media"]/main/*'/>*/
public record Media
{
    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Album"]/*'/>*/
    public String? Album             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Application"]/*'/>*/
    public String? Application       {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Artist"]/*'/>*/
    public String? Artist            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="BornOn"]/*'/>*/
    public String? BornOn            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="DistinguishedName"]/*'/>*/
    public String? DistinguishedName {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="ID"]/*'/>*/
    public Guid? ID                  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Modified"]/*'/>*/
    public String? Modified          {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Name"]/*'/>*/
    public String? Name              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Notes"]/*'/>*/
    public String[]? Notes           {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Tags"]/*'/>*/
    public String[]? Tags            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Title"]/*'/>*/
    public String? Title             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Type"]/*'/>*/
    public String? Type              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="Media"]/property[@name="Year"]/*'/>*/
    public String? Year              {get;init;}
}

/**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/main/*'/>*/
public record MediaRequest
{
    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Album"]/*'/>*/
    public String? Album             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Application"]/*'/>*/
    public String? Application       {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Artist"]/*'/>*/
    public String? Artist            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="BornOn"]/*'/>*/
    public String? BornOn            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="DistinguishedName"]/*'/>*/
    public String? DistinguishedName {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="ID"]/*'/>*/
    public Guid? ID                  {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Modified"]/*'/>*/
    public String? Modified          {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Name"]/*'/>*/
    public String? Name              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Notes"]/*'/>*/
    public String[]? Notes           {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Title"]/*'/>*/
    public String? Title             {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Tags"]/*'/>*/
    public String[]? Tags            {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Type"]/*'/>*/
    public String? Type              {get;init;}

    /**<include file='Media.xml' path='Media/record[@name="MediaRequest"]/property[@name="Year"]/*'/>*/
    public String? Year              {get;init;}
}

/**<include file='Media.xml' path='Media/record[@name="MediaResponse"]/main/*'/>*/
public record MediaResponse
{
    /**<include file='Media.xml' path='Media/record[@name="MediaResponse"]/property[@name="Media"]/*'/>*/
    public Media[] Media {get;init;} = Array.Empty<Media>();
}