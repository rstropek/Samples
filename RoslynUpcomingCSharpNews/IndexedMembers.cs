using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RoslynUpcomingCSharpNews
{
    [TestClass]
    public class IndexedMembers
    {
        [TestMethod]
        public void TestIndexedMembers()
        {
            var jsonContent = "{ 'FirstName': 'Max', 'LastName': 'Muster' }";
            var objectContent = JsonConvert.DeserializeObject(jsonContent) as JObject;

            // Use "old" indexed member syntax.
            Assert.AreEqual("Max", objectContent["FirstName"]);

            // Use "new" indexed member syntax.
            Assert.AreEqual("Max", objectContent.$FirstName);
            objectContent.$Age = 20;
            Assert.AreEqual(20, objectContent.$Age);
        }

        private class Person(public string FirstName, public string LastName)
        { }

        [TestMethod]
        public void TestElementInitializers()
        {
            // C# supports collection initialization for quite a while now.
            var people = new List<Person>()
            {
                new Person("Max", "Muster"),
                new Person("Tim", "Smith")
            };

            // It has not been possible for dictionaries.
            var peopleDictionary = new Dictionary<string, Person>();
            peopleDictionary.Add("Max", new Person("Max", "Muster"));
            peopleDictionary.Add("Tim", new Person("Tim", "Smith"));

            // Now we can use element initializers to initialize dictionaries, too.
            peopleDictionary = new Dictionary<string, Person>
            {
                ["Max"] = new Person("Max", "Muster"),
                ["Tim"] = new Person("Tim", "Smith")
            };
        }
    }
}
