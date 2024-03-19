namespace KusDepot.Data;

/**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/main/*'/>*/
public record ActiveService
{
    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="ActorID"]/*'/>*/
    public Guid? ActorID              {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="ActorNameID"]/*'/>*/
    public String? ActorNameID        {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Application"]/*'/>*/
    public String? Application        {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="ApplicationVersion"]/*'/>*/
    public String? ApplicationVersion {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="BornOn"]/*'/>*/
    public String? BornOn             {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="DistinguishedName"]/*'/>*/
    public String? DistinguishedName  {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="ID"]/*'/>*/
    public Guid? ID                   {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Interfaces"]/*'/>*/
    public String? Interfaces         {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Modified"]/*'/>*/
    public String? Modified           {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Name"]/*'/>*/
    public String? Name               {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Notes"]/*'/>*/
    public String[]? Notes            {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Purpose"]/*'/>*/
    public String? Purpose            {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Registered"]/*'/>*/
    public String? Registered         {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="ServiceVersion"]/*'/>*/
    public String? ServiceVersion     {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Tags"]/*'/>*/
    public String[]? Tags             {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Url"]/*'/>*/
    public String? Url                {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveService"]/property[@name="Version"]/*'/>*/
    public String? Version            {get;init;}
}

/**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/main/*'/>*/
public record ActiveServiceRequest
{
    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="ActorID"]/*'/>*/
    public Guid? ActorID              {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="ActorNameID"]/*'/>*/
    public String? ActorNameID        {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Application"]/*'/>*/
    public String? Application        {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="ApplicationVersion"]/*'/>*/
    public String? ApplicationVersion {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="BornOn"]/*'/>*/
    public String? BornOn             {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="DistinguishedName"]/*'/>*/
    public String? DistinguishedName  {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="ID"]/*'/>*/
    public Guid? ID                   {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Interfaces"]/*'/>*/
    public String? Interfaces         {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Modified"]/*'/>*/
    public String? Modified           {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Name"]/*'/>*/
    public String? Name               {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Notes"]/*'/>*/
    public String[]? Notes            {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Purpose"]/*'/>*/
    public String? Purpose            {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Registered"]/*'/>*/
    public String? Registered         {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="ServiceVersion"]/*'/>*/
    public String? ServiceVersion     {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Tags"]/*'/>*/
    public String[]? Tags             {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Url"]/*'/>*/
    public String? Url                {get;init;}

    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceRequest"]/property[@name="Version"]/*'/>*/
    public String? Version            {get;init;}
}

/**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceResponse"]/main/*'/>*/
public record ActiveServiceResponse
{
    /**<include file='ActiveService.xml' path='ActiveService/record[@name="ActiveServiceResponse"]/property[@name="ActiveServices"]/*'/>*/
    public ActiveService[] ActiveServices {get;init;} = Array.Empty<ActiveService>();
}