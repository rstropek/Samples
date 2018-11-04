using Moq;
using Polygon.Core;
using Polygon.Core.Generators;
using System;
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
    }
}