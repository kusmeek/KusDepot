namespace EntitySQLArk;

[PrimaryKey("ID")]
public partial class Element : ModelBase
{
    public String? Application        {get;set;}

    public String? ApplicationVersion {get;set;}

    public String? BornOn             {get;set;}

    public String? DistinguishedName  {get;set;}

    public override Guid? ID          {get;set;}

    public String? Modified           {get;set;}

    public String? Name               {get;set;}

    public String? ObjectType         {get;set;}

    public String? ServiceVersion     {get;set;}

    public String? Type               {get;set;}

    public String? Version            {get;set;}
}