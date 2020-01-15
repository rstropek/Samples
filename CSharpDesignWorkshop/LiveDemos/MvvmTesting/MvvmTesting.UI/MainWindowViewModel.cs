using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MvvmTesting.UI
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            CalculateCommand = new DelegateCommand(
                () => Result = Number1 + Number2,
                () => Number1 > 0 && Number2 > 0);
        }

        public DelegateCommand CalculateCommand { get; }

        private int number1;
        public int Number1
        {
            get => number1;
            set
            {
                number1 = value;
                CalculateCommand.RaiseCanExecuteChanged();
            }
        }


        private int number2;
        public int Number2
        {
            get => number2;
            set
            {
                number2 = value;
                CalculateCommand.RaiseCanExecuteChanged();
            }
        }

        private int result;
        public int Result
        {
            get => result;
            set => SetProperty(ref result, value);
        }
    }
}
