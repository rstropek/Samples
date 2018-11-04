using System;
using System.Collections.Generic;
using System.Text;

namespace Polygon.Core.Generators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class FriendlyNameAttribute : Attribute
    {
        public string FriendlyName { get; }

        public FriendlyNameAttribute(string friendlyName)
        {
            FriendlyName = friendlyName;
        }
    }
}
