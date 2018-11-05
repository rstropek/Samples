using Moq;
using Polygon.Core;
using Polygon.Core.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;
using Xunit;

namespace PolygonDesigner.ViewLogic.Tests
{
    public class TestPolygonManagementViewModel
    {
        private Mock<PolygonGenerator> GetMockGenerator()
        {
            var mockGenerator = new Mock<PolygonGenerator>();
            mockGenerator.Setup(x => x.Generate(It.IsAny<double>()))
                .Returns(new ReadOnlyMemory<Point>(Array.Empty<Point>()));
            return mockGenerator;
        }

        private (TaskCompletionSource<double> tcs, Mock<AreaCalculator> calculator) GetMockAreaCalculator()
        {
            var tcs = new TaskCompletionSource<double>();
            var mockCalculator = new Mock<AreaCalculator>();
            mockCalculator.Setup(x => x.CalculateAreaAsync(It.IsAny<ReadOnlyMemory<Point>>(), It.IsAny<CancellationToken>(), It.IsAny<IProgress<double>>()))
                .Returns(tcs.Task);
            return (tcs, mockCalculator);
        }

        [Fact]
        public void CannotGeneratePolygonWithoutGenerator()
        {
            var vm = new PolygonManagementViewModel();

            // vm.SelectedPolygonGenerator is initially null

            Assert.False(vm.GenerateAndAddPolygonCommand.CanExecute());
        }

        [Fact]
        public void SendCommandEventWhenGeneratorGetsSelected()
        {
            var vm = new PolygonManagementViewModel();
            var receivedGenerateCommandChanged = false;
            vm.GenerateAndAddPolygonCommand.CanExecuteChanged +=
                (_, __) => receivedGenerateCommandChanged = true;

            vm.SelectedPolygonGenerator = GetMockGenerator().Object;

            Assert.True(receivedGenerateCommandChanged);
            Assert.True(vm.GenerateAndAddPolygonCommand.CanExecute());
        }

        [Fact]
        public void GeneratePolygon()
        {
            var vm = new PolygonManagementViewModel();
            vm.SelectedPolygonGenerator = GetMockGenerator().Object;
            vm.GenerateAndAddPolygonCommand.Execute();

            Assert.Single(vm.Polygons);

            var newPolygon = vm.Polygons[0];
            Assert.True(!string.IsNullOrEmpty(newPolygon.Description));
            Assert.Equal(newPolygon.StrokeColor.R, newPolygon.FillColor.R);
            Assert.Equal(newPolygon.StrokeColor.G, newPolygon.FillColor.G);
            Assert.Equal(newPolygon.StrokeColor.B, newPolygon.FillColor.B);
        }

        [FriendlyName("Dummy")]
        private class DummyGenerator : PolygonGenerator
        {
            public ReadOnlyMemory<Point> Generate(in double maxSideLength) =>
                throw new NotImplementedException();
        }

        [Fact]
        public void FillPolygonGeneratorsFromContainer()
        {
            var mockGenerator = new Mock<IUnityContainer>();
            mockGenerator.Setup(x => x.Resolve(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ResolverOverride[]>()))
                .Returns(new[] { new DummyGenerator() });

            var vm = new PolygonManagementViewModel(mockGenerator.Object);
            Assert.Single(vm.Generators);
            var generator = vm.Generators.First();
            Assert.Equal("Dummy", generator.FriendlyName);
            Assert.IsType<DummyGenerator>(generator.Generator);
            Assert.Equal(vm.SelectedPolygonGenerator, generator.Generator);
        }

        [Fact]
        public void CalculateAreaForSelectedPolygon()
        {
            (var tcs, var mockCalculator) = GetMockAreaCalculator();

            var vm = new PolygonManagementViewModel(null, mockCalculator.Object);
            vm.SelectedPolygon = new Polygon();
            var receivedEvents = new HashSet<string>();
            vm.PropertyChanged += (_, ea) => receivedEvents.Add(ea.PropertyName);
            var commandExecStates = (Calculate: false, Cancel: false);
            vm.CalculateAreaForSelectedPolygonCommand.CanExecuteChanged += (_, __) => commandExecStates.Calculate = true;
            vm.CancelAreaCalculationCommand.CanExecuteChanged += (_, __) => commandExecStates.Cancel = true;

            // Check state before calculation starts
            Assert.True(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
            Assert.False(vm.IsCalculatingArea);
            Assert.False(vm.CancelAreaCalculationCommand.CanExecute());
            Assert.Null(vm.Area);
            Assert.False(vm.AreaAvailable);

            // Start calculation
            vm.CalculateAreaForSelectedPolygonCommand.Execute();

            // Check state after starting the calculation
            Assert.True(vm.IsCalculatingArea);
            Assert.Contains(nameof(PolygonManagementViewModel.IsCalculatingArea), receivedEvents);
            Assert.False(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
            Assert.True(commandExecStates.Calculate);
            Assert.True(vm.CancelAreaCalculationCommand.CanExecute());
            Assert.True(commandExecStates.Cancel);

            // Simulate finishing of calculation
            commandExecStates.Calculate = commandExecStates.Cancel = false;
            receivedEvents.Clear();
            tcs.SetResult(42d);

            // Check state after calculation
            Assert.True(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
            Assert.True(commandExecStates.Calculate);
            Assert.False(vm.CancelAreaCalculationCommand.CanExecute());
            Assert.True(commandExecStates.Cancel);
            Assert.False(vm.IsCalculatingArea);
            Assert.Contains(nameof(PolygonManagementViewModel.IsCalculatingArea), receivedEvents);
            Assert.Equal(42d, vm.Area);
            Assert.Contains(nameof(PolygonManagementViewModel.Area), receivedEvents);
            Assert.True(vm.AreaAvailable);
            Assert.Contains(nameof(PolygonManagementViewModel.AreaAvailable), receivedEvents);
        }

        [Fact]
        public void CancelAreaCalculation()
        {
            (var tcs, var mockCalculator) = GetMockAreaCalculator();

            var vm = new PolygonManagementViewModel(null, mockCalculator.Object);
            vm.SelectedPolygon = new Polygon();
            vm.CalculateAreaForSelectedPolygonCommand.Execute();
            tcs.SetCanceled();

            // Check state after cancellation
            Assert.Null(vm.Area);
            Assert.True(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
            Assert.False(vm.CancelAreaCalculationCommand.CanExecute());
        }

        [Fact]
        public void CalculationNotPossibleIfNoPolygonSelected()
        {
            var vm = new PolygonManagementViewModel(null, new Mock<AreaCalculator>().Object);
            Assert.False(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
        }
    }
}