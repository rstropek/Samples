using System.Text;

namespace CustomClasses.Button
{
    public class ButtonText
	{
        public int Repeat { get; set; } = 1;

        public string Text { get; set; } = "";

		public override string ToString()
		{
			var text = new StringBuilder("BUTTON\n");
			for (int i = 0; i < this.Repeat; i++)
			{
				if (i > 0)
				{
					text.Append(", ");
				}

				text.Append(this.Text);
			}

			return  text.ToString();
		}
	}
}
