@View({ template: "<h1>Hello {{ bar }}!</h1>"})
class C {
    @log
    foo(n: number) {
        return n * 2;
    }
    
    public bar: string = "World";
}

// The following decorator extends the functionality of the
// decorated function. It adds logging.
function log(target: Function, key: string, value: any) {
    // 'target' will be C.prototype
    // 'key' will be 'foo'
    // 'value' will be the property descriptor of 'foo'
    return {
        value: function (...args: any[]) {

            // convert list of foo arguments to string
            var arguments = args.map(a => JSON.stringify(a)).join();

            // invoke foo() and get its return value
            var result = value.value.apply(this, args);

            // convert result to string
            var jsonResult = JSON.stringify(result);

            // display in console the function call details
            console.log(`Call: ${key}(${arguments}) => ${jsonResult}`);

            // return the result of invoking foo
            return result;
        }
    };
}

var c = new C();
// Invoke foo. Will print 'Call: foo(5) => 10'
console.log(c.foo(5));

// The following decorator adds metadata to a class.
interface IView {
    template: string;
}

function View(view: IView) {
    return (target: any) => {
        target.view = view; 
        return target;
    }
}

// Get template for decorated class
var template : string = (<any>C).view.template;
// Use a poor mens' template engine to print '<h1>Hello World!</h1>' 
console.log(template.replace("{{ bar }}", c.bar));


