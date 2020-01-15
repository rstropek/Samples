using Moq;
using Polygon.Core;
using Polygon.Core.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PolygonDesigner.ViewLogic.Tests
{
    public class TestPolygonManagementViewModel
    {
        private Mock<IPolygonGenerator> GetMockGenerator()
        {
            var mockGenerator = new Mock<IPolygonGenerator>();
            mockGenerator.Setup(x => x.Generate(It.IsAny<double>()))
                .Returns(new ReadOnlyMemory<Point>(Array.Empty<Point>()));
            return mockGenerator;
        }

        private (TaskCompletionSource<double> tcs, Mock<IAreaCalculator> calculator) GetMockAreaCalculator()
        {
            var tcs = new TaskCompletionSource<double>();
            var mockCalculator = new Mock<IAreaCalculator>();
            mockCalculator.Setup(x => x.CalculateAreaAsync(It.IsAny<ReadOnlyMemory<Point>>(), It.IsAny<IProgress<double>>(), It.IsAny<CancellationToken>()))
                .Returns(tcs.Task);
            return (tcs, mockCalculator);
        }

        [Fact]
        public void CannotGeneratePolygonWithoutGenerator()
        {
            using var vm = new PolygonManagementViewModel(Array.Empty<IPolygonGenerator>());

            // vm.SelectedPolygonGenerator is initially null

            Assert.False(vm.GenerateAndAddPolygonCommand.CanExecute());
        }

        [Fact]
        public void SendCommandEventWhenGeneratorGetsSelected()
        {
            using var vm = new PolygonManagementViewModel(Array.Empty<IPolygonGenerator>());
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
            var mockGenerator = GetMockGenerator();
            using var vm = new PolygonManagementViewModel(Array.Empty<IPolygonGenerator>())
            {
                SelectedPolygonGenerator = mockGenerator.Object
            };
            vm.GenerateAndAddPolygonCommand.Execute();

            mockGenerator.Verify(x => x.Generate(in It.Ref<double>.IsAny), Times.Once());
            Assert.Single(vm.Polygons);

            var newPolygon = vm.Polygons[0];
            Assert.False(string.IsNullOrEmpty(newPolygon.Description));
            Assert.Equal(newPolygon.StrokeColor.R, newPolygon.FillColor.R);
            Assert.Equal(newPolygon.StrokeColor.G, newPolygon.FillColor.G);
            Assert.Equal(newPolygon.StrokeColor.B, newPolygon.FillColor.B);
        }

        [FriendlyName("Dummy")]
        private class DummyGenerator : IPolygonGenerator
        {
            public ReadOnlyMemory<Point> Generate(in double maxSideLength) =>
                throw new NotImplementedException();
        }

        [Fact]
        public void FillPolygonGeneratorsFromContainer()
        {
            using var vm = new PolygonManagementViewModel(new[] { new DummyGenerator() });
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

            using var vm = new PolygonManagementViewModel(Array.Empty<IPolygonGenerator>(), mockCalculator.Object)
            {
                SelectedPolygon = new Polygon()
            };
            var receivedEvents = new HashSet<string>();
            vm.PropertyChanged += (_, ea) => receivedEvents.Add(ea.PropertyName);
            var calculate = false;
            var cancel = false;
            vm.CalculateAreaForSelectedPolygonCommand.CanExecuteChanged += (_, __) => calculate = true;
            vm.CancelAreaCalculationCommand.CanExecuteChanged += (_, __) => cancel = true;

            // Check state before calculation starts
            Assert.True(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
            Assert.False(vm.IsCalculatingArea);
            Assert.False(vm.CancelAreaCalculationCommand.CanExecute());
            Assert.Null(vm.Area);
            Assert.False(vm.IsAreaAvailable);

            // Start calculation
            vm.CalculateAreaForSelectedPolygonCommand.Execute();

            // Check state after starting the calculation
            Assert.True(vm.IsCalculatingArea);
            Assert.Contains(nameof(PolygonManagementViewModel.IsCalculatingArea), receivedEvents);
            Assert.False(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
            Assert.True(calculate);
            Assert.True(vm.CancelAreaCalculationCommand.CanExecute());
            Assert.True(cancel);

            // Simulate finishing of calculation
            calculate = cancel = false;
            receivedEvents.Clear();
            tcs.SetResult(42d);

            // Check state after calculation
            Assert.True(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
            Assert.True(calculate);
            Assert.False(vm.CancelAreaCalculationCommand.CanExecute());
            Assert.True(cancel);
            Assert.False(vm.IsCalculatingArea);
            Assert.Contains(nameof(PolygonManagementViewModel.IsCalculatingArea), receivedEvents);
            Assert.Equal(42d, vm.Area);
            Assert.Contains(nameof(PolygonManagementViewModel.Area), receivedEvents);
            Assert.True(vm.IsAreaAvailable);
            Assert.Contains(nameof(PolygonManagementViewModel.IsAreaAvailable), receivedEvents);
        }

        [Fact]
        public void CancelAreaCalculation()
        {
            (var tcs, var mockCalculator) = GetMockAreaCalculator();

            using var vm = new PolygonManagementViewModel(Array.Empty<IPolygonGenerator>(), mockCalculator.Object)
            {
                SelectedPolygon = new Polygon()
            };
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
            using var vm = new PolygonManagementViewModel(Array.Empty<IPolygonGenerator>(), new Mock<IAreaCalculator>().Object);
            Assert.False(vm.CalculateAreaForSelectedPolygonCommand.CanExecute());
        }

        private class DummyClipper : IPolygonClipper
        {
            public ReadOnlySpan<Point> GetIntersectedPolygon(in ReadOnlySpan<Point> _, in ReadOnlySpan<Point> __) =>
                new ReadOnlySpan<Point>();
        }

        [Fact]
        public void ClipPossibleWithMultiplePolygons()
        {
            var clipper = new DummyClipper();
            var dummyGenerator = Mock.Of<IPolygonGenerator>();
            using var vm = new PolygonManagementViewModel(new[] { dummyGenerator }, null, clipper);

            Assert.False(vm.ClipPolygonsCommand.CanExecute());
            Assert.Empty(vm.Polygons);

            vm.GenerateAndAddPolygonCommand.Execute();
            vm.GenerateAndAddPolygonCommand.Execute();
            Assert.True(vm.ClipPolygonsCommand.CanExecute());

            vm.ClipPolygonsCommand.Execute();
            Assert.Equal(3, vm.Polygons.Count);
        }
    }
}