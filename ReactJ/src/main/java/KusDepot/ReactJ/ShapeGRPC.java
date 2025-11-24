package KusDepot.ReactJ;

import java.util.UUID;
import org.slf4j.Logger;
import shapeapi.Shapeapi;
import shapeapi.ShapeAPIGrpc;
import org.slf4j.LoggerFactory;
import io.grpc.stub.StreamObserver;

public class ShapeGRPC extends ShapeAPIGrpc.ShapeAPIImplBase
{
    private static final Logger logger = LoggerFactory.getLogger(ShapeGRPC.class);

    @Override
    public void generateShape(Shapeapi.ToolShape request, StreamObserver<Shapeapi.ToolShape> responseObserver)
    {
        try
        {
            Shapeapi.ToolShape response = toProto(ShapeAPI.GenerateShape(fromProto(request)));

            responseObserver.onNext(response); responseObserver.onCompleted();
        }
        catch (Exception e)
        {
            logger.error("GenerateShapeFail", e); responseObserver.onError(e);
        }
    }

    private static ToolShape fromProto(Shapeapi.ToolShape proto)
    {
        ToolShape shape = new ToolShape();
        try
        {
            shape.setId(proto.hasId() ? UUID.fromString(proto.getId()) : null);
            shape.setX(proto.hasX() ? proto.getX() : null);
            shape.setY(proto.hasY() ? proto.getY() : null);
            shape.setCircle(proto.hasCircle() ? proto.getCircle() : null);
            shape.setOpacity(proto.hasOpacity() ? proto.getOpacity() : null);
            shape.setRgb(proto.hasRgb() ? proto.getRgb() : null);
            shape.setRotation(proto.hasRotation() ? proto.getRotation() : null);
            shape.setScale(proto.hasScale() ? proto.getScale() : null);
            shape.setSides(proto.hasSides() ? proto.getSides() : null);
            shape.setStar(proto.hasStar() ? proto.getStar() : null);
        }
        catch (Exception e)
        {
            logger.error("fromProtoFail", e);
        }
        return shape;
    }

    private static Shapeapi.ToolShape toProto(ToolShape input)
    {
        Shapeapi.ToolShape.Builder shape = Shapeapi.ToolShape.newBuilder();
        try
        {
            if (input.getId() != null) shape.setId(input.getId().toString());
            if (input.getX() != null) shape.setX(input.getX());
            if (input.getY() != null) shape.setY(input.getY());
            if (input.getCircle() != null) shape.setCircle(input.getCircle());
            if (input.getOpacity() != null) shape.setOpacity(input.getOpacity());
            if (input.getRgb() != null) shape.setRgb(input.getRgb());
            if (input.getRotation() != null) shape.setRotation(input.getRotation());
            if (input.getScale() != null) shape.setScale(input.getScale());
            if (input.getSides() != null) shape.setSides(input.getSides());
            if (input.getStar() != null) shape.setStar(input.getStar());
        }
        catch (Exception e)
        {
            logger.error("toProtoFail", e);
        }
        return shape.build();
    }
}