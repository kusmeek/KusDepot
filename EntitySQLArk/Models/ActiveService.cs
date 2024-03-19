namespace EntitySQLArk;

[PrimaryKey("ID")]
public partial class ActiveService : ModelBase
{
    public Guid?   ActorID            {get;set;}

    public String? ActorNameID        {get;set;}

    public String? Application        {get;set;}

    public String? ApplicationVersion {get;set;}

    public String? BornOn             {get;set;}

    public String? DistinguishedName  {get;set;}

    public override Guid? ID          {get;set;}

    public String? Interfaces         {get;set;}

    public String? Modified           {get;set;}

    public String? Name               {get;set;}

    public String? Purpose            {get;set;}

    public String? Registered         {get;set;}

    public String? ServiceVersion     {get;set;}

    public String? Url                {get;set;}

    public String? Version            {get;set;}
}