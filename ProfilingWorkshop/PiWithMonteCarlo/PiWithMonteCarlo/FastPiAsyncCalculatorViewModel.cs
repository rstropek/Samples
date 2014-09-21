using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PiWithMonteCarlo
{
	/// <summary>
	/// Viewmodel that can easily be consumed in XAML-based applications
	/// </summary>
	public class FastPiAsyncCalculatorViewModel : BindableBase
	{
		public FastPiAsyncCalculatorViewModel()
		{
			this.startCommand = new DelegateCommand(
				async () => await this.OnStartCalculation(),
				() => !this.IsCalculating);

			this.stopCommand = new DelegateCommand(
				this.OnStopCalculation,
				() => this.IsCalculating);
		}

		private CancellationTokenSource cts = null;
		private Stopwatch watch = null;
		private static Task finishedTask = Task.FromResult(0);

		private async Task OnStartCalculation()
		{
			if (!this.IsCalculating)
			{
				// Start calculation
				this.IsCalculating = true;
				this.watch = new Stopwatch();
				this.watch.Start();
				this.cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
				await FastPiAsyncCalculator.CalculateAsync(
					this.cts.Token,
					result =>
					{
						// Navigate back to UI thread to update notification properties
						SynchronizationContext.Current.Post(new SendOrPostCallback(_ =>
						{
							this.CalcualtedPi = result.ResultPi;
							this.Iterations += result.Iterations;
							this.IterationsPerSecond = (double)this.Iterations / this.watch.Elapsed.TotalSeconds;
						}), null);
						return FastPiAsyncCalculatorViewModel.finishedTask;
					},
					() =>
					{
						// Switch state to not calculating
						this.IsCalculating = false;
						return FastPiAsyncCalculatorViewModel.finishedTask;
					});
			}
		}

		private void OnStopCalculation()
		{
			if (this.IsCalculating)
			{
				// Cancel calculation
				this.cts.Cancel();
			}
		}

		#region Bindable properties
		private double calculatedPi;
		public double CalcualtedPi
		{
			get { return this.calculatedPi; }
			private set { SetProperty(ref this.calculatedPi, value); }
		}

		private long iterations;
		public long Iterations
		{
			get { return this.iterations; }
			private set { SetProperty(ref this.iterations, value); }
		}

		private double iterationsPerSecond;
		public double IterationsPerSecond
		{
			get { return this.iterationsPerSecond; }
			private set { SetProperty(ref this.iterationsPerSecond, value); }
		}

		private bool isCalculating;
		public bool IsCalculating
		{
			get { return this.isCalculating; }
			private set 
			{ 
				SetProperty(ref this.isCalculating, value);
				this.startCommand.RaiseCanExecuteChanged();
				this.stopCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion

		#region Bindable commands
		private DelegateCommand startCommand;
		public ICommand StartCommand
		{
			get { return this.startCommand; }
		}

		private DelegateCommand stopCommand;
		public ICommand StopCommand
		{
			get { return this.stopCommand; }
		}
		#endregion
	}
}
