import {HelloRequest, HelloReply} from './greet_pb';
import {GreeterClient} from './GreetServiceClientPb';
import XMLHttpRequest = require('xhr2');
global.XMLHttpRequest = XMLHttpRequest;

var client = new GreeterClient('http://localhost:5000');

var request = new HelloRequest();
request.setName('World');

client.sayHello(request, {}, (err, response) => {
  console.log(response.getMessage());
});
