using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

//var resultStatic = System.Text.Json.JsonSerializer.Serialize(
//    "Hello World!", typeof(string), null);
//Console.WriteLine(resultStatic);

var a = Assembly.Load("System.Text.Json");
var t = a.GetType("System.Text.Json.JsonSerializer");
var mi = t.GetMethods().First(m => m.Name == "Serialize" && !m.IsGenericMethod);
var resultDynamic = mi.Invoke(null, new object[] { "Hello World!", typeof(string), null });
Console.WriteLine(resultDynamic);
