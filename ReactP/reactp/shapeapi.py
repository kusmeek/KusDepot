import uuid
import random
from . import logger
from .toolshape import ToolShape

def RandomCircle():
    try:
        return random.randint(0, 255) < 25
    except Exception as e:
        logger.error("RandomCircleFail", exc_info=True)
        return False

def RandomColor():
    try:
        bytes_ = [random.randint(0, 255) for _ in range(3)]
        return "#{:02X}{:02X}{:02X}".format(*bytes_)
    except Exception as e:
        logger.error("RandomColorFail", exc_info=True)
        return ""

def RandomCoordinate(maximum):
    try:
        if maximum <= 0:
            return 0.0
        return random.random() * maximum
    except Exception as e:
        logger.error("RandomCoordinateFail", exc_info=True)
        return 0.0

def RandomOpacity(minimum, maximum):
    try:
        return minimum + (maximum - minimum) * random.random()
    except Exception as e:
        logger.error("RandomOpacityFail", exc_info=True)
        return 1.0

def RandomRotation():
    try:
        return random.random() * 360.0
    except Exception as e:
        logger.error("RandomRotationFail", exc_info=True)
        return 0.0

def RandomScale(minimum, maximum):
    try:
        return minimum + (maximum - minimum) * random.random()
    except Exception as e:
        logger.error("RandomScaleFail", exc_info=True)
        return 1.0

def RandomSides(minimum, maximum):
    try:
        if minimum > maximum:
            return 12
        return random.randint(minimum, maximum)
    except Exception as e:
        logger.error("RandomSidesFail", exc_info=True)
        return 12

def RandomStar():
    try:
        return random.randint(0, 255) < 50
    except Exception as e:
        logger.error("RandomStarFail", exc_info=True)
        return False

def generate_shape(input_shape: ToolShape = None) -> ToolShape:
    try:
        input_shape = input_shape or ToolShape()
        shape = ToolShape()
        shape.id = input_shape.id or uuid.uuid4()
        shape.x = input_shape.x if input_shape.x is not None else RandomCoordinate(7648)
        shape.y = input_shape.y if input_shape.y is not None else RandomCoordinate(4304)
        shape.circle = input_shape.circle if input_shape.circle is not None else RandomCircle()
        shape.opacity = input_shape.opacity if input_shape.opacity is not None else RandomOpacity(0.0, 1.0)
        shape.rgb = input_shape.rgb if input_shape.rgb is not None else RandomColor()
        shape.rotation = input_shape.rotation if input_shape.rotation is not None else RandomRotation()
        shape.scale = input_shape.scale if input_shape.scale is not None else RandomScale(0.1, 2.0)
        shape.sides = input_shape.sides if input_shape.sides is not None else RandomSides(3, 8)
        shape.star = input_shape.star if input_shape.star is not None else RandomStar()
        return shape
    except Exception as e:
        logger.error("GenerateShapeFail", exc_info=True)
        return None