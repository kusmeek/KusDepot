using Shapeapi;
namespace KusDepot.Reacts;

public sealed class ShapeApiGRPClient : IShapeApi
{
    public Boolean Online { get; private set; } = false;

    private readonly GrpcChannel Channel; private readonly ShapeAPI.ShapeAPIClient Client;

    public String? Endpoint {get; private set; } public String? Service { get; private set; }

    public ShapeApiGRPClient(String endpoint , String? service)
    {
        Channel = GrpcChannel.ForAddress(endpoint);
        Endpoint = endpoint; this.Service = service;
        Client = new ShapeAPI.ShapeAPIClient(Channel);
    }

    public async Task<ToolShape?> GenerateShape(ToolShape? input = null)
    {
        try
        {
            return FromGrpcToolShape(await Client.GenerateShapeAsync(ToGrpcToolShape(input),deadline:DateTime.UtcNow.AddMilliseconds(100)));
        }
        catch ( RpcException _ ) when (Equals(_.StatusCode,StatusCode.DeadlineExceeded))
        {
            Log.Error(GenerateShapeTimeout,Endpoint,Service); return null;
        }
        catch ( Exception _ )
        {
            Log.Error(_,GenerateShapeFail,Endpoint,Service); return null;
        }
    }

    private static ToolShape? FromGrpcToolShape(Shapeapi.ToolShape? grpc)
    {
        try
        {
            if(grpc is null) { return null; }

            return new ToolShape
            {
                id = grpc.HasId ? Guid.TryParse(grpc.Id, out var g) ? g : null : null,
                x = grpc.HasX ? grpc.X : null,
                y = grpc.HasY ? grpc.Y : null,
                circle = grpc.HasCircle ? grpc.Circle : null,
                opacity = grpc.HasOpacity ? grpc.Opacity : null,
                rgb = grpc.HasRgb ? grpc.Rgb : null,
                rotation = grpc.HasRotation ? grpc.Rotation : null,
                scale = grpc.HasScale ? grpc.Scale : null,
                sides = grpc.HasSides ? grpc.Sides : null,
                star = grpc.HasStar ? grpc.Star : null
            };
        }
        catch ( Exception _ ) { Log.Error(_,FromGrpcToolShapeFail); return null; }
    }

    private static Shapeapi.ToolShape? ToGrpcToolShape(ToolShape? shape)
    {
        try
        {
            if(shape is null) { return null; } var g = new Shapeapi.ToolShape();

            if(shape.id is not null) { g.Id = shape.id.ToString(); }
            if(shape.x is not null) { g.X = shape.x.Value; }
            if(shape.y is not null) { g.Y = shape.y.Value; }
            if(shape.circle is not null) { g.Circle = shape.circle.Value; }
            if(shape.opacity is not null) { g.Opacity = shape.opacity.Value; }
            if(shape.rgb is not null) { g.Rgb = shape.rgb; }
            if(shape.rotation is not null) { g.Rotation = shape.rotation.Value; }
            if(shape.scale is not null) { g.Scale = shape.scale.Value; }
            if(shape.sides is not null) { g.Sides = shape.sides.Value; }
            if(shape.star is not null) { g.Star = shape.star.Value; }

            return g;
        }
        catch ( Exception _ ) { Log.Error(_,ToGrpcToolShapeFail); return null; }
    }

    public async Task<Boolean> IsOnline()
    {
        try
        {
            var r = await Client.GenerateShapeAsync(
                ToGrpcToolShape(new ToolShape()),
                deadline: DateTime.UtcNow.AddMilliseconds(100)
            );

            if(r is null) { Log.Warning(IsOnlineFail,Endpoint,Service); Online = false; return false; }

            Log.Verbose(IsOnlineSuccess,Endpoint,Service); Online = true; return true;
        }
        catch ( Exception _ ) { Log.Warning(_,IsOnlineFail,Endpoint,Service); Online = false; return false; }
    }

    private const String ToGrpcToolShapeFail = "ShapeApiGRPClient ToGrpcToolShape Failed";
    private const String FromGrpcToolShapeFail = "ShapeApiGRPClient FromGrpcToolShape Failed";
    private const String IsOnlineFail = "ShapeApiGRPClient IsOnline Failed {@Endpoint} {@Service}";
    private const String IsOnlineSuccess = "ShapeApiGRPClient IsOnline Success {@Endpoint} {@Service}";
    private const String GenerateShapeFail = "ShapeApiGRPClient GenerateShape Failed {@Endpoint} {@Service}";
    private const String GenerateShapeTimeout = "ShapeApiGRPClient GenerateShape Timeout {@Endpoint} {@Service}";
}