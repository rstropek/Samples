using System;
using System.ComponentModel;

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

					// Note the use of the null conditional and nameof Operators here
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme.Name)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
                }
            }
        }

		// Note the use of Lambda bodied property here.
        // The exact syntax of string interpolations will change until RTM of VS2015.
        public string FullName => 
            "Theme: \{Name}, Background: \{BackgroundColor}, Foreground: \{ForegroundColor}";

		// Note the use of Lambda bodied function here.
		public Theme Clone() => new Theme(this.Name, this.BackgroundColor, this.ForegroundColor);

		// Note the use of a getter-only property with initializer here.
		public ConsoleColor BackgroundColor { get; } = ConsoleColor.White;

        public ConsoleColor ForegroundColor { get; } = ConsoleColor.Black;

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

		public Theme()
		{
			// No need to set default values any more.
		}
    }
}
