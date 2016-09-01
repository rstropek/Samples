using Prism.Commands;

namespace PrismUnityDemo.Contracts
{
    public class GlobalCommands
	{
		public GlobalCommands()
		{
			this.Print = new CompositeCommand(true);
			this.PrintAll = new CompositeCommand();
			this.Close = new CompositeCommand(true);
		}

		public CompositeCommand Print { get; private set; }
		public CompositeCommand PrintAll { get; private set; }
		public CompositeCommand Close { get; private set; }
	}
}
