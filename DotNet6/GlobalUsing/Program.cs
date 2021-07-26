// Note: no `using System` necessary
var dates = new DateOnly[] { new(2021, 1, 1), new(2022, 1, 1) };

// Note: no `using System.Text` necessary
var builder = new StringBuilder();

// Note: no `using System.Linq` necessary
var datesString = dates.Aggregate(
    new StringBuilder(),
    (sb, d) => sb.AppendLine(d.ToString("o")),
    sb => sb.ToString());

// Note: no `Console` necessary
WriteLine(datesString);
