namespace KusDepot.Data;

/**<include file='ActorIds.xml' path='ActorIds/class[@name="ActorIds"]/main/*'/>*/
public static class ActorIds
{
    /**<include file='ActorIds.xml' path='ActorIds/class[@name="ActorIds"]/property[@name="CoreCache"]/*'/>*/
    public static ActorId CoreCache
    {
        get
        {
            return new ActorId("CoreCache");
        }
    }

    /**<include file='ActorIds.xml' path='ActorIds/class[@name="ActorIds"]/property[@name="DataConfiguration"]/*'/>*/
    public static ActorId DataConfiguration
    {
        get
        {
            return new ActorId("DataConfiguration");
        }
    }

    /**<include file='ActorIds.xml' path='ActorIds/class[@name="ActorIds"]/property[@name="Universe"]/*'/>*/
    public static ActorId Universe
    {
        get
        {
            return new ActorId("Universe");
        }
    }
}