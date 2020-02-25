@Component({
  selector: 'my-app',
  templateUrl: './app.component.html',
  styleUrls: [ './app.component.css' ]
})
export class AppComponent implements OnInit {
  greeting: string;
  fibonacciNumbers: number[] = [];

  ngOnInit() {
    const greeterService = new GreeterClient('https://localhost:5001', null, null);

    const request = new HelloRequest();
    request.setName('Angular');

    const call = greeterService.sayHello(request, {},
      (err: grpcWeb.Error, response: HelloReply) => {
        this.greeting = response.getMessage();
      });

    const mathService = new MathGuruClient('https://localhost:5001', null, null);
    const fromTo = new FromTo();
    fromTo.setFrom(10);
    fromTo.setTo(1000);
    const stream = mathService.getFibonacci(fromTo, {});
    stream.on('data', (response: NumericResult) => {
      this.fibonacciNumbers.push(response.getResult());
    });
  }
}