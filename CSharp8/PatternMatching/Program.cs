using System;

// Read the original proposal (incl. syntax specs) at
// https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/patterns.md

namespace RecursivePatterns
{
    enum HeroType
    {
        NuclearAccident,
        FailedScienceExperiment,
        Alien,
        Mutant,
        Other
    };

    enum HeroTypeCategory
    {
        Accident,
        SuperPowersFromBirth,
        Other
    }

    class Person
    {
        public Person(string name)
        {
            Name = name;
        }

        public string GenerateName() => Name.ToUpper();

        public string Name { get; set; }
    }

    // Note that Hero derives from Person
    class Hero : Person
    {
        public Hero(string name, bool canFly, HeroType heroType = HeroType.Other) : base(name)
        {
            CanFly = canFly;
            HeroType = heroType;
        }

        public bool CanFly { get; set; }

        public HeroType HeroType { get; set; }

        public HeroTypeCategory HeroTypeCategory
        {
            // Note Property Pattern in the following Switch Expression
            get => this switch
            {
                { HeroType: HeroType.NuclearAccident } => HeroTypeCategory.Accident,
                { HeroType: HeroType.FailedScienceExperiment } => HeroTypeCategory.Accident,
                { HeroType: HeroType.Alien } => HeroTypeCategory.SuperPowersFromBirth,
                { HeroType: HeroType.Mutant } => HeroTypeCategory.SuperPowersFromBirth,
                _ => HeroTypeCategory.Other
            };
        }
    }

    class Program
    {
        static void Main()
        {
            var people = new Person[]
            {
                new Hero("Superman", true, HeroType.Alien),
                new Person("John Doe"),
                new Hero("Flash", false, HeroType.FailedScienceExperiment),
            };

            RecursivePattern(people);

            SwitchExpression(people);
        }

        private static void RecursivePattern(Person[] people)
        {
            foreach (var person in people)
            {
                // C# 7-style pattern matching. Note that `hero` is accessed inside
                // the if-expression
                if (person is Hero hero && hero.CanFly)
                {
                    Console.WriteLine($"We have flying hero {hero.Name}");
                }

                // C# 8-style recursive pattern mathing.
                if (person is Hero { CanFly: true, Name: var name })
                {
                    Console.WriteLine($"We have flying hero {name}");
                }

                // Note that you can use recursive patterns in assignments, too.
                var canFly = person is Hero { CanFly: true };
                if (canFly)
                {
                    Console.WriteLine("The guy can fly");
                }
            }

            var t = (Name: "Superman", CanFly: true);

            // Patterns work with tuples, too. Here we find out if CanFly is true.
            if (t is (_, true))
            {
                Console.WriteLine("The guy from the tuple can fly");
            }

            // The next example combines pattern matching on a tuple with
            // recursive pattern matching.
            if (t is ("Superman", _) { CanFly: true } sman)
            {
                Console.WriteLine("The superman from the tuple can fly");
            }
        }

        private static void SwitchExpression(Person[] people)
        {
            foreach (var person in people)
            {
                // Note that you can now use switch as an expression. Very useful
                // in combination with pattern matching.
                // Be *CAREFUL* when editing this code in the current VS2019 preview.
                // Unfortunately, it crashes regularly when working with switch expressions.
                Console.WriteLine(person switch
                {
                    Hero { Name: var n, CanFly: true } => $"Hero {n} that can fly",
                    Hero h => $"Hero {h.Name} {h.CanFly}",
                    Person p => $"Person {p.Name}",
                    _ => "Who is that?!?"
                });
            }
        }
    }
}
