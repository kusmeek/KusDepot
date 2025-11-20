package main

import (
	"log"
	"net"
	"net/http"
	"os"
	"google.golang.org/grpc"
	"github.com/gin-gonic/gin"
	"kusdepot/reactg/shapeapi"
	"kusdepot/reactg/shapeapi_pb"
)

func getEnv(key, fallback string) string {
	val := os.Getenv(key)
	if val == "" {
		return fallback
	}
	return val
}

func main() {
	gin.SetMode(gin.ReleaseMode)
	httpPort := getEnv("HTTP_PORT", "8083")
	grpcPort := getEnv("GRPC_PORT", "8084")

	go func() {
		lis, err := net.Listen("tcp", ":"+grpcPort)
		if err != nil {
			log.Fatalf("Failed to listen on port %s: %v", grpcPort, err)
		}
		grpcServer := grpc.NewServer()
		shapeapi_pb.RegisterShapeAPIServer(grpcServer, shapeapi.NewShapeAPIServer())
		log.Printf("[ReactG-GrpcServer] Listening on %s\n", grpcPort)
		if err := grpcServer.Serve(lis); err != nil {
			log.Fatalf("[ReactG-GrpcServer] Failed: %v", err)
		}
	}()
	
	r := gin.New()
	r.Use(gin.Recovery())
	r.SetTrustedProxies(nil)
	r.POST("/generateshape", func(c *gin.Context) {
		var input shapeapi.ToolShape
		if err := c.ShouldBindJSON(&input); err != nil {
			c.JSON(http.StatusBadRequest, gin.H{"error": "Bad Request"})
			return
		}
		result := shapeapi.GenerateShape(&input)
		if result == nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": "GenerateShape Failed"})
			return
		}
		c.JSON(http.StatusOK, result)
	})
	log.Printf("[ReactG-HttpServer] Listening on %s\n", httpPort)
	if err := r.Run(":" + httpPort); err != nil {
		log.Fatalf("[ReactG-HttpServer] Failed: %v", err)
	}
}