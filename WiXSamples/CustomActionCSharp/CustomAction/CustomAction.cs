using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace CustomAction
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult GetRandomNumber(Session session)
		{
			session["RANDOM_NUMBER"] = new Random().Next(100000).ToString();
			return ActionResult.Success;
		}
	}
}
