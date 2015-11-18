using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModuleToTest;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests
{
    // Note that you can get a list of all testing-related classes at
    // https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.aspx

    // Note that unit test class has to be marked with TestClass attribute.
    // https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testclassattribute.aspx
    [TestClass]
    public class UnitTestBasics
    {
        #region Initialization and cleanup
        // Note that you can specify methods to initalize/cleanup the test environment.
        // See https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testinitializeattribute.aspx
        //   for details.
        [TestInitialize]
        public void RunsBeforeEachTest()
        {
            // Todo: Initialize environment before each test
            Trace.WriteLine("Test Initialize is running.");
        }
        #endregion

        // BTW: Do you know the "testc" and "testm" snippets?

        #region Asserts
        [TestMethod]
        public void Asserts()
        {
            // For details about asserts see
            // https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.assert.aspx

            // Some trivial asserts
            Assert.IsTrue(1 == 1);
            Assert.IsFalse(1 == 0);
            Assert.IsNotNull("X");

            // Note the difference between AreEqual and AreSame
            Assert.AreEqual("ab", "ab");
            // Assert.AreSame("ab", $"{"a"}b"); --> this would fail

            // Manual asserts with fail and inconclusive
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Fail("This test will fail on Sunday.");
            } else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
            {
                // Note that inconclusive does not lead to a failing test.
                // The test will be marked as "skipped".
                Assert.Inconclusive("Result of test cannot be determined on Saturday.");
            }

            // Check type of an object
            var someObjects = new object[] { 1, "Test", 5.0 };
            Assert.IsInstanceOfType(someObjects[0], typeof(int));
        }

        private class Person { }
        private class VipPerson : Person { }

        [TestMethod]
        public void CollectionAsserts()
        {
            var somePersons = new Person[] { new Person(), new VipPerson() };

            // Some trivial collection asserts
            CollectionAssert.AllItemsAreNotNull(somePersons);
            CollectionAssert.Contains(somePersons, somePersons[0]);

            // Some more interesting checks
            CollectionAssert.AllItemsAreInstancesOfType(somePersons, typeof(Person));

            CollectionAssert.AllItemsAreUnique(somePersons);
            // CollectionAssert.AllItemsAreUnique(new [] { "A", "A" }); --> this would fail.

            var someStrings = new[] { "AB", "CD" };
            var someOtherStrings = new[] { $"{"A"}B", "CD" };
            var evenMoreStrings = new[] { "CD", $"{"A"}B" };
            CollectionAssert.AreEqual(someStrings, someOtherStrings);
            // CollectionAssert.AreEqual(someStrings, evenMoreStrings); -> this would fail

            // Note that AreEquivalent ignores order, so this works:
            CollectionAssert.AreEquivalent(someStrings, evenMoreStrings);

            CollectionAssert.IsSubsetOf(new[] { "AB" }, someStrings);
        }

        [TestMethod]
        public void StringAsserts()
        {
            // Trivial string asserts
            StringAssert.Contains("ABC", "B");
            StringAssert.EndsWith("ABC", "C");
            StringAssert.StartsWith("ABC", "A");

            // Pattern matching with Regex
            StringAssert.Matches("-10.0", new Regex(@"[+-]?\d+\.\d*$"));
        }
        #endregion

        #region Testing private members
        [TestMethod]
        public void TestPrivates()
        {
            // Note how we test private instance method with PrivateObject
            // https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.privateobject.aspx
            var po = new PrivateObject(new SomeBusinessLogicClass());
            Assert.AreEqual(3, po.Invoke("SomeInternalLogic", 1, 2));

            // Note how we test private static methods with PrivateType
            // https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.privatetype.aspx
            var pt = new PrivateType(typeof(SomeBusinessLogicClass));
            Assert.AreEqual(42, pt.InvokeStatic("SomeInternalStaticLogic"));

            // For PrivateObject and PrivateType, be VERY CAREFUL with parameters.
            // If you use the wrong type, you will get strange erros as methods
            //   cannot be found.
        }
        #endregion

        #region Data-driven test
        // Note that the framework will automatically fill this property
        public TestContext TestContext { get; set; }

        [DataSource("System.Data.SqlServerCe.4.0", "Data Source=TestData.sdf;", "TestValues", DataAccessMethod.Sequential)]
        [DeploymentItem("TestData.sdf")]
        [TestMethod]
        public void DataDrivenTest()
        {
            // Don't forget to set a breakpoint in this method and
            //   inspect this.TestContext and watch how the test method
            //   is called multiple times depending on the content 
            //   of the data source.

            var objectToTest = new SomeBusinessLogicClass();

            var number = Convert.ToInt32(this.TestContext.DataRow["Value"]);
            Assert.AreEqual(number * number, objectToTest.Square(number));
        }
        #endregion

        #region Test method attributes
        // Note how you can add descriptive information using attributes
        [Description("This is a test that demonstrates various attributes.")]
        [Owner("Rainer")]
        [Priority(1)]
        [TestCategory("Demotests")]
        [TestMethod]
        [TestProperty("DB", "SQL Server")]
        public void TestWithManyAttributes()
        {
        }

        // Note that this test expects a specific exception
        [ExpectedException(typeof(DivideByZeroException))]
        [TestMethod]
        public void TestWithException()
        {
            var x = 0;
            var y = 5;
            Trace.WriteLine(y / x);
        }

        // Note that you can mark a test as ignorable.
        [Ignore]
        [TestMethod]
        public void FailingTest()
        {
            Assert.Fail();
        }

        // Note that you can specify timeouts to check perf limits.
        [Timeout(150)]
        [TestMethod]
        public void LongRunningTest()
        {
            Thread.Sleep(100);
        }

        // You can also create your own test attributes.
        // This is out of scope for this example.
        // See e.g. http://blogs.msdn.com/b/vstsqualitytools/archive/2009/09/04/extending-the-visual-studio-unit-test-type-part-1.aspx
        #endregion

        #region Async test
        // Note that the following test is WRONG!
        [Ignore]
        [TestMethod]
        public void TestAsyncDBAccess()
        {
            // Note that this test will FAIL as ReadDataAsync
            // is an async method.
            Assert.AreEqual(42, SomeDatabaseAccess.ReadDataAsync());
        }

        // Async tests MUST return Task.
        // Note that you can use async await in async tests.
        [TestMethod]
        public async Task SimpleAsyncTest()
        {
            Assert.AreEqual(42, await SomeDatabaseAccess.ReadDataAsync());
        }

        // Note that you can still use ExpectedException 
        // when writing async tests.
        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public async Task AsyncExceptionTest()
        {
            await Task.Delay(100);
            await SomeDatabaseAccess.FailingDataAccessAsync();
        }
        #endregion

    }
}
