# Sinus Data Generator

1. Some intro

    ```csharp
    Console.WriteLine("Welcome to the sinus data generator");

    do
    {
    }
    while(true);

    record GenerationOptions(int NumberOfValues, double Frequency, double Amplitude, double OutlierPercentage, double OutlierFactor);
    ```

2. `// Ask user for data generation options`

    ```csharp
    // Ask user for data generation options
    Console.WriteLine("Please enter the number of values to generate:");
    int numberOfValues = int.Parse(Console.ReadLine()!);

    Console.WriteLine("Please enter the frequency of the sinus curve:");
    double frequency = double.Parse(Console.ReadLine()!);

    Console.WriteLine("Please enter the amplitude of the sinus curve:");
    double amplitude = double.Parse(Console.ReadLine()!);

    Console.WriteLine("Please enter the percentage of outliers:");
    double outlierPercentage = double.Parse(Console.ReadLine()!);

    Console.WriteLine("Please enter the factor of outliers:");
    double outlierFactor = double.Parse(Console.ReadLine()!);

    // Generate data
    var options = new GenerationOptions(numberOfValues, frequency, amplitude, outlierPercentage, outlierFactor);
    ```

3. **Help me write a method that generates 500 random data points in C#. The generated data should follow a sinus curve with a slight, random deviation. However, a small number of values should be obvious, random outliers.**
   1. **In modern C#, you should use `Random.Shared` instead of `new Random()`.**
   2. Generate method call with Copilot

4. `// Print all values on the screen`

5. **I am looking for an easy way to generate a scatter plot in C#. The component should be able to generate plot images on Linux**
   1. Discuss current ScottPlot version
   2. Add *ScottPlot*

        ```xml
        <ItemGroup>
            <PackageReference Include="ScottPlot" Version="5.0.5-beta" />
            <PackageReference Include="SkiaSharp.NativeAssets.Linux.NoDependencies" Version="2.88.3" />
        </ItemGroup>
        ```

   3. Replace printing with scatter plot

        ```csharp
        var plt = new ScottPlot.Plot();
        var xValues = Enumerable.Range(0, numberOfValues).Select(i => (double)i).ToArray();
        plt.Add.Scatter(xValues, values).Label = "Original";
        plt.SavePng("quickstart.png", 1920, 1024);
        Console.WriteLine("Diagram saved to quickstart.png\n");
        ```

6. Change first prompt to **Please enter the number of values to generate (500 is used if you just press enter):**
   1. Remove input line, show that Copilot does not generate the code correctly.
   2. Change first line to `var input = Console.ReadLine()!;`
   3. Show that Copilot correctly generates `int numberOfValues = string.IsNullOrWhiteSpace(input) ? 500 : int.Parse(input);`
   4. Repeat the change process for all parameters

        ```csharp
        // Ask user for data generation options
        Console.WriteLine("Please enter the number of values to generate (500 is used if you just press enter):");
        var input = Console.ReadLine()!;
        int numberOfValues = string.IsNullOrWhiteSpace(input) ? 500 : int.Parse(input);

        Console.WriteLine("Please enter the frequency of the sinus curve (1 is used if you just press enter):");
        input = Console.ReadLine()!;
        double frequency = string.IsNullOrWhiteSpace(input) ? 1 : double.Parse(input);

        Console.WriteLine("Please enter the amplitude of the sinus curve (3 is used if you just press enter):");
        input = Console.ReadLine()!;
        double amplitude = string.IsNullOrWhiteSpace(input) ? 3 : double.Parse(input);

        Console.WriteLine("Please enter the percentage of outliers (5 is used if you just press enter):");
        input = Console.ReadLine()!;
        double outlierPercentage = string.IsNullOrWhiteSpace(input) ? 5 : double.Parse(input);

        Console.WriteLine("Please enter the factor of outliers (20 is used if you just press enter):");
        input = Console.ReadLine()!;
        double outlierFactor = string.IsNullOrWhiteSpace(input) ? 20 : double.Parse(input);
        ```

7. **I have the following code for asking the user for data generation parameters. Can you help me create a helper functions that removes code duplication?** (add previously shown code)

    ```csharp
    static T GetUserInput<T>(string prompt, T defaultValue)
    {
        Console.WriteLine(prompt);
        var input = Console.ReadLine(); 
        
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue; 

        return (T)Convert.ChangeType(input, typeof(T));
    }
    ```

8. **What does this code do?** (add previously shown code)

9. **In C#, I have a list of double values. The values are sensor readings. I plotted the readings and saw that some values are obviously outliers. Can you help me write a method that identifies outliers and replaces them with the average of the two neighboring values?From looking at the data, I think it would be a good idea to identify the outliers by calculating the average absolute distance between sensor readings. Every value that is more than three time the average distance away from the previous reading can be considered an outlier. Please generate just C# code without description.**
    1. Generated code:

        ```csharp
        static List<double> RemoveOutliers(List<double> sensorReadings)
        {
            if (sensorReadings == null || sensorReadings.Count < 3)
            {
                throw new ArgumentException("Invalid sensor readings list.");
            }

            double totalDistance = 0;
            for (int i = 1; i < sensorReadings.Count; i++)
            {
                totalDistance += Math.Abs(sensorReadings[i] - sensorReadings[i - 1]);
            }

            double averageDistance = totalDistance / (sensorReadings.Count - 1);
            double outlierThreshold = averageDistance * 3;

            List<double> processedReadings = new List<double>(sensorReadings);
            for (int i = 1; i < sensorReadings.Count - 1; i++)
            {
                double prevReading = sensorReadings[i - 1];
                double currentReading = sensorReadings[i];
                double nextReading = sensorReadings[i + 1];

                if (Math.Abs(currentReading - prevReading) > outlierThreshold &&
                    Math.Abs(currentReading - nextReading) > outlierThreshold)
                {
                    processedReadings[i] = (prevReading + nextReading) / 2;
                }
            }

            return processedReadings;
        }
        ```

    2. Generate method call with Copilot.

        ```csharp
        // Generate data
        var options = new GenerationOptions(numberOfValues, frequency, amplitude, outlierPercentage, outlierFactor);
        var values = GenerateSinusData(options.NumberOfValues, options.OutlierPercentage, options.Amplitude, options.Frequency);
        var removed = RemoveOutliers(values);

        var plt = new ScottPlot.Plot();
        var xValues = Enumerable.Range(0, numberOfValues).Select(i => (double)i).ToArray();
        plt.Add.Scatter(xValues, values).Label = "Original";
        plt.Add.Scatter(xValues, removed).Label = "Removed once";
        ```

10. **I have the following C# method to remove outliers from sensor value. Now I need to come up with some unit tests. Can you suggest tests?**





Generate 10 text messages in which ficticious users ask to reset their password. The request must contain the user name. Answer with JSON in the following format:&nbsp;  
&nbsp;  
{ "prompt": "<insert generated message here>", "completion": "{\"request\": \"reset-pwd\", \"instance\": \"<insert generated user name here>\" " }
