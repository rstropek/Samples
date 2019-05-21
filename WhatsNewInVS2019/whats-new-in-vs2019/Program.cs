using System;
using System.Linq;

namespace WhatsNewInVS2019
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(BuildFullName("Mr.", "John", "Doe"));

            Console.WriteLine(IsValidPerson(new[] { "John", "Doe", "40" }));

            var (dummy, dummy2, age) = BuildPerson(new[] { "John", "Doe", "40" });
            Console.WriteLine(age);

            Console.WriteLine(CalculateAverageAge("John,Doe,40\nJane,Smith,20"));

            var app = new Appelation();
            // var (salutation, prefixTitle, postfixTitle) = app;

            Debugging.DoSomethingWithTickets();

            var d = Enumerable.Range(0, 5).Select(_ => new Debugging()).ToArray();
            foreach (var item in d)
            {
                item.LetSomethingHappen();
            }
        }

        static string BuildFullName(string salutation, string lastName, string firstName)
        {
            var result = $"{salutation} {firstName} {lastName}";
            return result;
        }

        static bool IsValidPerson(string[] data)
        {
            bool result;

            var person = new
            {
                FirstName = data[0],
                LastName = data[1],
                Age = int.Parse(data[2])
            };

            result = string.IsNullOrEmpty(person.FirstName) &&
                string.IsNullOrEmpty(person.LastName) &&
                person.Age >= 0 && person.Age < 150;

            return result;
        }

        static double CalculateAverageAge(string csv) =>
            csv.Split("\n")
                .Select(line => line.Split(','))
                .Select(line => new { FirstName = line[0], LastName = line[1], Age = int.Parse(line[2]) })
                .Average(person => person.Age);

        static (string FirstName, string LastName, int Age) BuildPerson(string[] data) =>
            (data[0], data[1], int.Parse(data[2]));

        class Appelation
        {
            public string Salutation { get; set; }
            public string TitlePrefix { get; set; }
            public string TitlePostfix { get; set; }
        }
    }
}
