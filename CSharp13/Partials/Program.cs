// UserCode.cs
using System.ComponentModel;

var vm = new ViewModel();
vm.UserName = "John Doe";
Console.WriteLine(vm.UserName);

vm["UserName"] = "Jane Doe";
Console.WriteLine(vm.UserName);

public partial class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // Partial properties are new in C# 13. They are particularly useful for code generation.
    public partial string UserName { get; set; }

    // Partial indexers are also new in C# 13.
    public partial object? this[string propertyName] { get; set; }
}

// Generated.cs
public partial class ViewModel
{
    private string __generated_userName = "";

    public partial string UserName
    {
        get => __generated_userName;
        set
        {
            if (value != __generated_userName) {
                __generated_userName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
            }
        }
    }

    public partial object? this[string propertyName]
    {
        get
        {
            // Use reflection to get the property value
            var propertyInfo = this.GetType().GetProperty(propertyName);
            return propertyInfo?.GetValue(this);
        }
        set
        {
            // Use reflection to set the property value
            var propertyInfo = this.GetType().GetProperty(propertyName);
            propertyInfo?.SetValue(this, value);
        }
    }
}