using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Collections.ObjectModel;
using IronPython.Hosting;
using System.Reflection;

namespace IronPython.UI.Scripts
{
	public class ExtendedObject<T> : DynamicObject
	{
		private Dictionary<string, Func<T, object>> calculatedProperties = new Dictionary<string, Func<T, object>>();

		public ExtendedObject(T underlyingObject)
		{
			this.UnderlyingObject = underlyingObject;
		}

		public T UnderlyingObject { get; private set; }

		public void AddCalculatedProperty(string propertyName, string formula)
		{
			var engine = Python.CreateEngine();
			var script = engine.CreateScriptSourceFromString(formula);
			var function = script.Execute<Func<T, object>>();
			this.calculatedProperties.Add(propertyName, function);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (this.calculatedProperties.ContainsKey(binder.Name))
			{
				result = this.calculatedProperties[binder.Name](this.UnderlyingObject);
				return true;
			}
			else
			{
				if (this.UnderlyingObject.GetType().GetProperty(binder.Name) != null)
				{
					result = this.UnderlyingObject.GetType().InvokeMember(binder.Name, BindingFlags.GetProperty, null, this.UnderlyingObject, null);
					return true;
				}
			}

			return base.TryGetMember(binder, out result);
		}
	}
}
