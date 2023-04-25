import * as jspb from 'google-protobuf'

import * as google_rpc_status_pb from './google/rpc/status_pb';


export class HelloRequest extends jspb.Message {
  getName(): string;
  setName(value: string): HelloRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): HelloRequest.AsObject;
  static toObject(includeInstance: boolean, msg: HelloRequest): HelloRequest.AsObject;
  static serializeBinaryToWriter(message: HelloRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): HelloRequest;
  static deserializeBinaryFromReader(message: HelloRequest, reader: jspb.BinaryReader): HelloRequest;
}

export namespace HelloRequest {
  export type AsObject = {
    name: string,
  }
}

export class HelloReply extends jspb.Message {
  getMessage(): string;
  setMessage(value: string): HelloReply;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): HelloReply.AsObject;
  static toObject(includeInstance: boolean, msg: HelloReply): HelloReply.AsObject;
  static serializeBinaryToWriter(message: HelloReply, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): HelloReply;
  static deserializeBinaryFromReader(message: HelloReply, reader: jspb.BinaryReader): HelloReply;
}

export namespace HelloReply {
  export type AsObject = {
    message: string,
  }
}

export class FromTo extends jspb.Message {
  getFrom(): number;
  setFrom(value: number): FromTo;

  getTo(): number;
  setTo(value: number): FromTo;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): FromTo.AsObject;
  static toObject(includeInstance: boolean, msg: FromTo): FromTo.AsObject;
  static serializeBinaryToWriter(message: FromTo, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): FromTo;
  static deserializeBinaryFromReader(message: FromTo, reader: jspb.BinaryReader): FromTo;
}

export namespace FromTo {
  export type AsObject = {
    from: number,
    to: number,
  }
}

export class NumericResult extends jspb.Message {
  getResult(): number;
  setResult(value: number): NumericResult;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): NumericResult.AsObject;
  static toObject(includeInstance: boolean, msg: NumericResult): NumericResult.AsObject;
  static serializeBinaryToWriter(message: NumericResult, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): NumericResult;
  static deserializeBinaryFromReader(message: NumericResult, reader: jspb.BinaryReader): NumericResult;
}

export namespace NumericResult {
  export type AsObject = {
    result: number,
  }
}

export class StepByStepResult extends jspb.Message {
  getResult(): NumericResult | undefined;
  setResult(value?: NumericResult): StepByStepResult;
  hasResult(): boolean;
  clearResult(): StepByStepResult;

  getError(): google_rpc_status_pb.Status | undefined;
  setError(value?: google_rpc_status_pb.Status): StepByStepResult;
  hasError(): boolean;
  clearError(): StepByStepResult;

  getPayloadCase(): StepByStepResult.PayloadCase;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): StepByStepResult.AsObject;
  static toObject(includeInstance: boolean, msg: StepByStepResult): StepByStepResult.AsObject;
  static serializeBinaryToWriter(message: StepByStepResult, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): StepByStepResult;
  static deserializeBinaryFromReader(message: StepByStepResult, reader: jspb.BinaryReader): StepByStepResult;
}

export namespace StepByStepResult {
  export type AsObject = {
    result?: NumericResult.AsObject,
    error?: google_rpc_status_pb.Status.AsObject,
  }

  export enum PayloadCase { 
    PAYLOAD_NOT_SET = 0,
    RESULT = 1,
    ERROR = 2,
  }
}

