using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpInfotag
{
    public class Theme : INotifyPropertyChanged
    {
        private string NameValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return NameValue; }
            set
            {
                if (NameValue != value)
                {
                    NameValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
                }
            }
        }

        public string FullName
        {
            get
            {
                return string.Format("Theme: {0}, Background: {1}, Foreground: {2}",
                    this.Name, this.BackgroundColor, this.ForegroundColor);
            }
        }

        // Note that the exact syntax of string interpolations will change until RTM of VS2015
        public string FullNameNew => 
            "Theme: \{Name}, Background: \{BackgroundColor}, Foreground: \{ForegroundColor}";

        public ConsoleColor BackgroundColor { get; } = ConsoleColor.Yellow;

        public ConsoleColor ForegroundColor { get; } = ConsoleColor.Red;

        public Theme(string name, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            if (backgroundColor == foregroundColor)
            {
                // Simulate an exception with an inner exception (needed for another demo)
                throw new Exception("Critical exception", new ArgumentException("Invalid colors"));
            }

            this.Name = name;
            this.BackgroundColor = backgroundColor;
            this.ForegroundColor = foregroundColor;
        }
    }
}
