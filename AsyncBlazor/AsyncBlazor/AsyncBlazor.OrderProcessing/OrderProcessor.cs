using AsyncBlazor.Model;
using System.Threading.Tasks;

namespace AsyncBlazor.OrderProcessing
{
    /// <summary>
    /// Logic to verify and process orders
    /// </summary>
    /// <remarks>
    /// This class does not contain actual business logic. The functions are
    /// just placeholder to demonstrate the concept of asynchronous processing
    /// of business transactions.
    /// </remarks>
    public class OrderProcessor
    {
        /// <summary>
        /// Verifies a given order
        /// </summary>
        /// <remarks>
        /// Light-weight order verification. A user can expect this function to
        /// finish within a few dozen milliseconds.
        /// </remarks>
        /// <returns>True if order is valid, otherwise false.</returns>
        public async Task<bool> VerifyOrderAsync(Order _)
        {
            // Simulate some processing time. Assumption: Initial checking of
            // incoming order is rather fast (e.g. check of master data like 
            // referenced customer exists).
            await Task.Delay(50);

            return true;
        }

        /// <summary>
        /// Processes a given order
        /// </summary>
        /// <remarks>
        /// This function contains the actual business logic for processing an
        /// order. In practice, it would interact with backend ERP systems, do
        /// a lot of DB interactions, etc. Therefore, processing an order 
        /// typically takes 10-20 seconds.
        /// </remarks>
        /// <returns>True if order has been successfully processed, otherwise false.</returns>
        public async Task<bool> ProcessOrderAsync(Order _)
        {
            // Simulate processing time. Assumption: This is the heavy lifting
            // for order processing. Might include complex calculations, talking
            // to multiple slow backend services, executing complex queries, etc.
            await Task.Delay(5000);

            return true;
        }
    }
}
