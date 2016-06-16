using System;

namespace RefReturns
{
    class Program
    {
        static void Main(string[] args)
        {
            var calculatedDeliveryDate = new DateTime(2016, 5, 18);
            var desiredDeliveryDate = new DateTime(2016, 5, 1);

            // This is "old-style". Passing DateTime values around means
            // copying memory (Ldloc IL statement).
            Console.WriteLine(GetDeliveryDateByValue(
                calculatedDeliveryDate, 
                desiredDeliveryDate));

            // This is "new-style". Passing and returning DateTime values 
            // by reference does not need to copy memory (Ldloca IL statement).
            Console.WriteLine(GetDeliveryDateByRef(
                ref calculatedDeliveryDate, 
                ref desiredDeliveryDate));
        }

        static DateTime GetDeliveryDateByValue(DateTime calc, DateTime desired)
        {
            if (calc < desired)
            {
                return desired;
            }
            else
            {
                return calc;
            }
        }

        static ref DateTime GetDeliveryDateByRef(ref DateTime calc, ref DateTime desired)
        {
            if (calc < desired)
            {
                return ref desired;
            }
            else
            {
                return ref calc;
            }
        }
    }
}
