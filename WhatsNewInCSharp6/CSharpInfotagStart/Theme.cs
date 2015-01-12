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
					if (this.PropertyChanged != null)
					{
						this.PropertyChanged(this, new PropertyChangedEventArgs("Name"));
						this.PropertyChanged(this, new PropertyChangedEventArgs("FullName"));
					}
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

		public Theme Clone()
		{
			return new Theme(this.Name, this.BackgroundColor, this.ForegroundColor);
		}

		// Note that we cannot make BackgroundColor readonly if we use 
		// auto-implemented properties. Also note that prior to C# 6 we
		// could not initialize value of BackgroundColor here.
		public ConsoleColor BackgroundColor { get; private set; }

		// Note that we cannot use auto-implemented properties if we want
		// to use readonly.
		private readonly ConsoleColor ForegroundColorValue = ConsoleColor.Black;
		public ConsoleColor ForegroundColor { get { return this.ForegroundColorValue; } }

        public Theme(string name, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            if (backgroundColor == foregroundColor)
            {
                // Simulate an exception with an inner exception (needed for another demo)
                throw new Exception("Critical exception", new ArgumentException("Invalid colors"));
            }

            this.Name = name;
            this.BackgroundColor = backgroundColor;
            this.ForegroundColorValue = foregroundColor;
        }

		public Theme()
		{
			// Set default colors
			this.ForegroundColorValue = ConsoleColor.Black;
		}
    }
}
