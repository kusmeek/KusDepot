FROM python:3-alpine
WORKDIR /app
COPY reactp ./reactp
COPY requirements.txt ./requirements.txt
RUN pip install --no-cache-dir -r requirements.txt
EXPOSE 8089 8090
ENTRYPOINT ["python", "-m", "reactp.main"]