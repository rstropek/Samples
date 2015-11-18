using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModuleToTest
{
    public class SomeBusinessLogicClass
    {
        private int SomeInternalLogic(int x, int y) => x + y;

        private static int SomeInternalStaticLogic() => 42;

        public int Square(int x) => x * x;
    }

    public class SomeDatabaseAccess
    {
        // Note how we use the unit test as a code example
        // for our API. See also Sudoku sample for larger example
        // including Sandcastle Help File Builder.
        // https://github.com/rstropek/Samples/blob/master/SudokuBoard/Samples.Sudoku/Board.cs

        /// <summary>
        /// Reads some data from the database.
        /// </summary>
        /// <returns>Result from the database.</returns>
		/// <example>
		/// <code source="../UnitTests/UnitTestBasics.cs" region="Async test" language="C#" />
		/// </example>
        public static async Task<int> ReadDataAsync()
        {
            // Simulate DB access (would be 
            // e.g. await myCmd.ExecuteReaderAsync() in practice).
            await Task.Delay(100);

            // Return result of "DB access"
            return 42;
        }

        public static async Task FailingDataAccessAsync()
        {
            // Simulate DB access (would be 
            // e.g. await myCmd.ExecuteReaderAsync() in practice).
            await Task.Delay(100);

            throw new Exception("Something bad happed with the DB.");
        }
    }
}
