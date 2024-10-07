using ConsoleTetris;
using Moq;

namespace Tetris.Tests
{
    public class AsciiDrawingTests
    {
        private readonly Mock<ITetrisConsole> _consoleMock;
        private readonly AsciiDrawing _asciiDrawing;

        public AsciiDrawingTests()
        {
            _consoleMock = new Mock<ITetrisConsole>();
            _asciiDrawing = new AsciiDrawing(_consoleMock.Object, new TetrisBlock());
        }

        [Theory]
        [InlineData(5, 2, 5, 4)]  // Vertical line
        [InlineData(2, 3, 5, 3)]  // Horizontal line
        [InlineData(5, 4, 5, 2)]  // Reverse vertical line
        [InlineData(5, 3, 2, 3)]  // Reverse horizontal line
        public void DrawLine_ValidLines_ShouldDrawCorrectly(int x1, int y1, int x2, int y2)
        {
            // Act
            _asciiDrawing.DrawLine(x1, y1, x2, y2);

            // Assert
            VerifyLineDrawn(x1, y1, x2, y2);
        }

        [Fact]
        public void DrawLine_DiagonalLine_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _asciiDrawing.DrawLine(1, 1, 3, 3));
        }

        private void VerifyLineDrawn(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2)  // Vertical line
            {
                int start = Math.Min(y1, y2);
                int end = Math.Max(y1, y2);
                _consoleMock.Verify(
                    c => c.SetCursorPosition(x1, It.IsInRange(start, end, Moq.Range.Inclusive)),
                    Times.Exactly(end - start + 1));
                _consoleMock.Verify(c => c.Write(It.IsAny<char>()), Times.Exactly(end - start + 1));
            }
            else  // Horizontal line
            {
                int start = Math.Min(x1, x2);
                int end = Math.Max(x1, x2);
                _consoleMock.Verify(c => c.SetCursorPosition(start, y1), Times.Once);
                _consoleMock.Verify(c => c.Write(It.Is<string>(s => s.Length == end - start + 1)), Times.Once);
            }
        }
    }
}