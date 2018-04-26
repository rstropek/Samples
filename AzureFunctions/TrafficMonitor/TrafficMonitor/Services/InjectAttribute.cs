using Microsoft.Azure.WebJobs.Description;
using System;

namespace TrafficMonitor.Services
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
