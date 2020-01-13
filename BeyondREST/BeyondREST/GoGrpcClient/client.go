package main

import (
	"context"
	"log"

	"github.com/Samples/BeyondREST/GoGrpcClient/greet"
	"google.golang.org/grpc"
)

func main() {
	conn, err := grpc.Dial("localhost:8080", grpc.WithInsecure())
	if err != nil {
		panic(err)
	}
	defer conn.Close()

	client := greet.NewGreeterClient(conn)
	response, err := client.SayHello(context.Background(), &greet.HelloRequest{Name: "FooBar"})
	if err != nil {
		panic(err)
	}

	log.Println(response.Message)
}
