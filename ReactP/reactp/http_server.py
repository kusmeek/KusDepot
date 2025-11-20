from . import logger
from .toolshape import ToolShape
from fastapi import FastAPI, Request
from .shapeapi import generate_shape

app = FastAPI()

@app.post("/generateshape")
async def generate_shape_endpoint(request: Request):
    try:
        data = await request.json()
        input_shape = ToolShape.from_dict(data) if data else None
        return generate_shape(input_shape).to_dict()
    except Exception as e:
        logger.error("HTTP /generateshape failed", exc_info=True)
        return {"error": "Failed to generate shape"}