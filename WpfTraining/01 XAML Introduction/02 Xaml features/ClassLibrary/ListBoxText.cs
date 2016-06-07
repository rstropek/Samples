using System.Text;

namespace CustomClasses.ListBox
{
    public class ListBoxText
	{
        public int Repeat { get; set; } = 1;

        public string Text { get; set; } = "";

        public override string ToString()
		{
			var text = new StringBuilder();
			for (int i = 0; i < this.Repeat; i++)
			{
				if (i > 0)
				{
					text.Append(", ");
				}

				text.Append(this.Text);
			}

			return text.ToString();
		}
	}
}
