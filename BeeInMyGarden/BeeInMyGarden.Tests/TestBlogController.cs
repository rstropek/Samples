using BeeInMyGarden.Data;
using BeeInMyGarden.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BeeInMyGarden.Tests
{
	[TestClass]
	public class TestBlogController
	{
		[TestMethod]
		public void TestDemoDataGeneration()
		{
			// Run index method on controller
			var controller = new BlogController();
			var result = controller.Index(null) as ViewResult;

			// Check that result's model is of correct type
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Model);
			var resultList = result.Model as IEnumerable<BlogItem>;
			Assert.IsNotNull(resultList);

			// Check if proper demo data has been created
			Assert.AreEqual(3, resultList.Count());
		}

		[TestMethod]
		public void TestNotExistingBlog()
		{
			// Run index method on controller for non-existing blog entry
			var controller = new BlogController();

			// Verify that http error was returned
			Assert.IsInstanceOfType(controller.Index(99), typeof(HttpNotFoundResult));

		}
	}
}
