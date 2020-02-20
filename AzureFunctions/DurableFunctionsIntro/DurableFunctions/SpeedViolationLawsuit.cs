using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace DurableFunctions
{
    /// <summary>
    /// Represents a speed violation lawsuite
    /// </summary>
    public class SpeedViolationLawsuit
    {
        public SpeedViolation SpeedViolation { get; set; }

        public string Driver { get; set; }

        public decimal? Fine { get; set; }

        public bool Paid { get; set; }

        public void SetSpeedViolation(SpeedViolation sv) => SpeedViolation = sv;

        public void StoreDriver(string driver) => Driver = driver;

        public async Task SetFine(decimal fine)
        {
            if (string.IsNullOrEmpty(Driver))
            {
                throw new InvalidOperationException();
            }

            // Simulate an async operation (e.g. for I/O)
            await Task.Delay(1);

            Fine = fine;
        }

        public void MarkAsPaid()
        {
            if (!Fine.HasValue)
            {
                throw new InvalidOperationException();
            }

            Paid = true;
        }

        public void Delete()
        {
            // Note how we access the current entity
            Entity.Current.DeleteState();
        }

        [FunctionName(nameof(SpeedViolationLawsuit))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
        {
            // When creating a new entity, make sure it is marked as not paid
            if (!ctx.HasState)
            {
                ctx.SetState(new SpeedViolationLawsuit
                {
                    Paid = false
                });
            }

            return ctx.DispatchAsync<SpeedViolationLawsuit>();
        }
    }
}
