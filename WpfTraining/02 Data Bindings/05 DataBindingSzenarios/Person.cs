using System;

namespace Samples
{
	public class Person
	{
		private string firstName;
		private string lastName;
		private DateTime birthday;
		private bool isMarried;
		private int numberOfChildren;

		public Person()
		{
		}

		public virtual string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

		public virtual string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}

		public virtual DateTime Birthday
		{
			get { return birthday; }
			set { birthday = value; }
		}

		public virtual bool IsMarried
		{
			get { return isMarried; }
			set { isMarried = value; }
		}

		public virtual int NumberOfChildren
		{
			get { return numberOfChildren; }
			set { numberOfChildren = value; }
		}

		public override string ToString()
		{
			return FirstName + ' ' + LastName;
		}
	}
}
