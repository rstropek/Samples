import { HelloRequest, HelloReply, FromTo, NumericResult } from "./greet_pb";
import { GreeterClient, MathGuruClient } from "./GreetServiceClientPb";
import XMLHttpRequest = require("xhr2");
global.XMLHttpRequest = XMLHttpRequest;

(async () => {
  const client = new GreeterClient("http://localhost:5002");

  const request = new HelloRequest();
  request.setName("World");

  try {
    const message = await client.sayHello(request, {});
    console.log("Greeting:", message.getMessage());

    const mathService = new MathGuruClient("http://localhost:5002");
    const fromTo = new FromTo();
    fromTo.setFrom(10);
    fromTo.setTo(1000);
    const stream = mathService.getFibonacci(fromTo, {});
    stream.on("data", (response: NumericResult) => {
      console.log(response.getResult());
    });
    stream.on("status", (status) => {
      console.log("status");
    });
    stream.on("end", () => {
      console.log("end");
    });
  } catch (error) {
    console.error(error);
  }
}) ();
