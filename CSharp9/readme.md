# Storyboard

* Create new empty ASP.NET 5 Web API *CSharp9Demo*
  * With HTTPS
  * With Docker
* Remove *IIS Express* launch settings
* Add analysis level *preview* for code quality

## Top-Level Statements

* Remove *Startup.cs*
* Replace *Program.cs* with Snippet *0010 Top-level Statements*
* Try https://localhost:5001/health

> Talk about top-level statements

## Attributes on Local Functions

* Add *DummyAuthentication.cs* with Snippet *0020 DummyAuthenticationHandler*
* Add auth to DI with Snippet *0030 Authentication DI*
* Add `app.UseAuthentication();` and `app.UseAuthorization();` after `UseRouting`
* Replace `MapGet` with Snippet *0040 Secure Health*

> Talk about attributes on local functions

* Try https://localhost:5001/health-secure
* Change role to *SuperAdmin*, try again -> forbidden
  * Change role back

## Records

* Add *Repository.cs*
* Add base code with Snippet *0050 Repository Starter*
* Add basic record with Snippet *0060 Person Record*
* Add derived records with Snippet *0070 Derived Records*

> Talk about records in general (immutability)

* Add member to record `Person` with Snippet *0080 Record Member*

> Talk about the fact that records are just classes

* Add init-only property to record `Hero` with Snippet *0090 Init-only Props*

> Talk about init-only properties

## Target-typed `new`

* Add base repository code with Snippet *0100 Base Repository*
* Add initialization for heroes with Snippet *0110 Hero Initialize*

> Discuss advantages/disadvantages of target-typed `new`

## Relational Patterns

* Add LINQ query with relational pattern to record `HeroRepository` with Snippet *0120 Relational Pattern*

> Talk about relational patterns

* Add `switch` with relational pattern to `Hero` with Snippet *0130 Switch with Relational Pattern*
* Add recursive relational pattern to `HeroRepository` with Snippet *0140 Recursive Relational Pattern*

## Back to Records

* Create API Controller `HeroController`
* Add DTOs to `HeroController` with Snippet *0150 DTOs*

> Discuss attributes on init-only props

* Add `HeroRepository` to DI: `services.AddSingleton<HeroRepository>();`
* Add repository to constructor for DI (`HeroRepository repository`)
* Add a method for getting all heroes with Snippet *0160 Get all heroes*

> Repeat pattern matching with records

* Add `AutoMapper.Extensions.Microsoft.DependencyInjection` NuGet package
* Add *AutoMapper* to DI with Snippet *0170 Automapper setup*
* Add method for getting all heroes (short version) with Snippet *0180 Get all heroes (short)*

> Discuss why AutoMapper just works (record = class)
