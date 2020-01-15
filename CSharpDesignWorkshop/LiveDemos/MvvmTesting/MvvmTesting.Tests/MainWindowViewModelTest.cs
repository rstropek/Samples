using MvvmTesting.UI;
using System;
using Xunit;

namespace MvvmTesting.Tests
{
    public class MainWindowViewModelTest
    {
        [Fact]
        public void Add_Is_Disabled_If_Number_Is_Zero()
        {
            var vm = new MainWindowViewModel();

            vm.Number1 = vm.Number2 = 0;
            Assert.False(vm.CalculateCommand.CanExecute());

            var canExecuteChangedFired = false;
            vm.CalculateCommand.CanExecuteChanged += (_, __) => canExecuteChangedFired = true;
            vm.Number1 = vm.Number2 = 1;
            Assert.True(vm.CalculateCommand.CanExecute());
            Assert.True(canExecuteChangedFired);
        }

        [Fact]
        public void Result_Is_Set_When_Adding()
        {
            var vm = new MainWindowViewModel();

            var propertyChangedFired = false;
            vm.PropertyChanged += (_, ea) => propertyChangedFired |= ea.PropertyName == nameof(MainWindowViewModel.Result);

            vm.Number1 = vm.Number2 = 21;
            vm.CalculateCommand.Execute();

            Assert.Equal(42, vm.Result);
            Assert.True(propertyChangedFired);
        }
    }
}
