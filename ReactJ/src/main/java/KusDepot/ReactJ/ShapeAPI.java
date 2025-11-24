package KusDepot.ReactJ;

import java.util.UUID;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import java.security.SecureRandom;

public final class ShapeAPI
{
    private static final SecureRandom rng = new SecureRandom();
    private static final Logger logger = LoggerFactory.getLogger(ShapeAPI.class);
    private ShapeAPI() {}

    public static ToolShape GenerateShape(ToolShape input)
    {
        try
        {
            ToolShape shape = new ToolShape();
            shape.setId(input != null && input.getId() != null ? input.getId() : UUID.randomUUID());
            shape.setX(input != null && input.getX() != null ? input.getX() : RandomCoordinate(7648));
            shape.setY(input != null && input.getY() != null ? input.getY() : RandomCoordinate(4304));
            shape.setCircle(input != null && input.getCircle() != null ? input.getCircle() : RandomCircle());
            shape.setOpacity(input != null && input.getOpacity() != null ? input.getOpacity() : RandomOpacity(0.0,1.0));
            shape.setRgb(input != null && input.getRgb() != null ? input.getRgb() : RandomColor());
            shape.setRotation(input != null && input.getRotation() != null ? input.getRotation() : RandomRotation());
            shape.setScale(input != null && input.getScale() != null ? input.getScale() : RandomScale(0.1,2.0));
            shape.setSides(input != null && input.getSides() != null ? input.getSides() : RandomSides(3,8));
            shape.setStar(input != null && input.getStar() != null ? input.getStar() : RandomStar());
            return shape;
        }
        catch ( Exception e )
        {
            logger.error("GenerateShapeFail",e); return null;
        }
    }

    private static boolean RandomCircle()
    {
        try
        {
            return rng.nextInt(256) < 25;
        }
        catch ( Exception e )
        {
            logger.error("RandomCircleFail",e); return false;
        }
    }

    private static String RandomColor()
    {
        try
        {
            byte[] bytes = new byte[3]; rng.nextBytes(bytes);

            return String.format("#%02X%02X%02X",bytes[0],bytes[1],bytes[2]);
        }
        catch ( Exception e )
        {
            logger.error("RandomColorFail",e); return "";
        }
    }

    private static double RandomCoordinate(int max)
    {
        try
        {
            if (max <= 0) { return 0; }

            return rng.nextDouble() * max;
        }
        catch ( Exception e )
        {
            logger.error("RandomCoordinateFail",e); return 0;
        }
    }

    private static double RandomOpacity(double min , double max)
    {
        try
        {
            return min + (max - min) * rng.nextDouble();
        }
        catch ( Exception e )
        {
            logger.error("RandomOpacityFail",e); return 1.0;
        }
    }

    private static double RandomRotation()
    {
        try
        {
            return rng.nextDouble() * 360.0;
        }
        catch ( Exception e )
        {
            logger.error("RandomRotationFail",e); return 0.0;
        }
    }

    private static double RandomScale(double min , double max)
    {
        try
        {
            return min + (max - min) * rng.nextDouble();
        }
        catch ( Exception e )
        {
            logger.error("RandomScaleFail",e); return 1.0;
        }
    }

    private static int RandomSides(int min , int max)
    {
        try
        {
            if (min > max) { return 12; }

            return rng.nextInt(max - min + 1) + min;
        }
        catch ( Exception e )
        {
            logger.error("RandomSidesFail",e); return 12;
        }
    }

    private static boolean RandomStar()
    {
        try
        {
            return rng.nextInt(256) < 50;
        }
        catch ( Exception e )
        {
            logger.error("RandomStarFail",e); return false;
        }
    }
}