package greet

import (
	grpcStatus "google.golang.org/genproto/googleapis/rpc/status"
	codes "google.golang.org/grpc/codes"
)

func StepByStepErrorResult(code codes.Code, message string) *StepByStepResult {
	return &StepByStepResult{
		Payload: &StepByStepResult_Error{
			Error: &grpcStatus.Status{
				Code:    int32(code),
				Message: message,
			},
		},
	}
}

func StepByStepSuccessResult(result int32) *StepByStepResult {
	return &StepByStepResult{
		Payload: &StepByStepResult_Result{
			Result: &NumericResult{
				Result: result,
			},
		},
	}
}
