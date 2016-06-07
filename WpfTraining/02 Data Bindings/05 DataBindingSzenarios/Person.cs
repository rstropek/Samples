using System;

namespace Samples
{
	public class Person
	{
		public virtual string FirstName { get; set; }

		public virtual string LastName { get; set; }

        public virtual DateTime Birthday { get; set; }

        public virtual bool IsMarried { get; set; }

        public virtual int NumberOfChildren { get; set; }

        public override string ToString() => FirstName + ' ' + LastName;
	}
}
