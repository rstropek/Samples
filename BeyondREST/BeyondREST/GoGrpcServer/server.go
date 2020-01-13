package main

import (
	"context"
	"fmt"
	"io"
	"log"
	"net"
	"time"

	"github.com/Samples/BeyondREST/GoGrpcServer/greet"
	"github.com/Samples/BeyondREST/GoGrpcServer/mathalgorithms"
	"google.golang.org/grpc"
	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"
)

type service struct {
}

type mathGuruService struct {
}

func (s service) SayHello(ctx context.Context, req *greet.HelloRequest) (*greet.HelloReply, error) {
	return &greet.HelloReply{
		Message: fmt.Sprintf("Hello %s", req.Name),
	}, nil
}

func (mgs mathGuruService) GetFibonacci(fromTo *greet.FromTo, stream greet.MathGuru_GetFibonacciServer) error {
	if fromTo.From > fromTo.To {
		return status.Error(codes.InvalidArgument, "FromTo.From must be <= FromTo.To")
	}

	fib := mathalgorithms.NewFibonacciIterator()
	for fib.Next() {
		if fib.Value() < fromTo.From {
			continue
		}

		if fib.Value() <= fromTo.To {
			stream.Send(&greet.NumericResult{
				Result: int32(fib.Value()),
			})
			time.Sleep(250 * time.Millisecond)
			continue
		}

		break
	}

	return nil
}

func (mgs mathGuruService) GetFibonacciStepByStep(stream greet.MathGuru_GetFibonacciStepByStepServer) error {
	fib := mathalgorithms.NewFibonacciIterator()
	var previousFromTo *greet.FromTo
	for {
		in, err := stream.Recv()
		if err == io.EOF {
			return nil
		}
		if err != nil {
			return err
		}

		if in.From > in.To {
			stream.Send(greet.StepByStepErrorResult(codes.InvalidArgument, "FromTo.From must be <= FromTo.To"))
			continue
		}

		if previousFromTo != nil && in.From < previousFromTo.To {
			stream.Send(greet.StepByStepErrorResult(codes.InvalidArgument, "From must be >= previously sent To"))
			continue
		}

		previousFromTo = in

		for fib.Next(); fib.Value() < in.From; fib.Next() {
		}

		if fib.Value() > in.To {
			continue
		}

		for ; fib.Value() <= in.To; fib.Next() {
			stream.Send(greet.StepByStepSuccessResult(fib.Value()))
			time.Sleep(250 * time.Millisecond)
		}
	}
}

func main() {
	listen, err := net.Listen("tcp", ":8080")
	if err != nil {
		panic(err)
	}

	// register service
	server := grpc.NewServer()
	greet.RegisterGreeterServer(server, service{})
	greet.RegisterMathGuruServer(server, mathGuruService{})

	// start gRPC server
	log.Println("starting gRPC server...")
	server.Serve(listen)
}
