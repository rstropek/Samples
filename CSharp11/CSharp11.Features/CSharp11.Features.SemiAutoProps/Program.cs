using System.ComponentModel;

// Not yet available in VS
// See https://sharplab.io/#gist:1f62e3187fa76fc38a722a731de1e43b instead
/*
var p = new Person { FirstName = "Foo", LastName = "Bar" };
p.PropertyChanged += (s, ea) => Console.WriteLine($"{ea.PropertyName} changed");
p.LastName = "Baz";

class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public string FullName => $"{LastName}, {FirstName}";
    public string FirstName
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                PropertyChanged?.Invoke(this, new(nameof(FirstName)));
                PropertyChanged?.Invoke(this, new(nameof(FullName)));
            }
        }
    }

    public string LastName
    {
        // ... (like FirstName above)
    }
}
*/