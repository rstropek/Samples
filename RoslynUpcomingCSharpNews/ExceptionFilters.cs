using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RoslynUpcomingCSharpNews
{
    // BTW - Note use of primary constructor here
    public class DataAccessException(string message, public bool IsCritical) 
        : Exception(message)
    {
    }

    public static class DataAccess
    {
        private static void UpdateInternal(bool fastMode)
        {
            if (fastMode)
            {
                // Let's assume something bad happened in "fast mode" ...
                throw new DataAccessException("Something bad happened!", true);
            }
            else
            {
                // Let's assume everything is OK in "normal" mode ...
            }
        }

        public static void Update_OldRethrowing()
        {
            try
            {
                UpdateInternal(fastMode: false);
                UpdateInternal(fastMode: true);
            }
            catch (DataAccessException dae) 
            {
                if (!dae.IsCritical)
                {
                    // Log and handle (e.g. retry) non-critical errors
                }
                else
                {
                    // We re-throw critical errors
                    throw;
                }
            }
        }

        public static void Update_RethrowingWithExceptionFilter()
        {
            try
            {
                UpdateInternal(fastMode: false);
                UpdateInternal(fastMode: true);
            }
            catch (DataAccessException dae) if (!dae.IsCritical)
            {
                // Log and handle (e.g. retry) non-critical errors
            }
        }
    }


    [TestClass]
    public class ExceptionFilters
    {
        [TestMethod]
        public void TestExceptionFilter()
        {
            string oldStackTrace;
            string newStackTrace;

            try
            {
                DataAccess.Update_OldRethrowing();
            }
            catch (DataAccessException dae)
            {
                oldStackTrace = dae.StackTrace;
            }

            try
            {
                DataAccess.Update_RethrowingWithExceptionFilter();
            }
            catch (DataAccessException dae)
            {
                newStackTrace = dae.StackTrace;
            }
        }
    }
}
