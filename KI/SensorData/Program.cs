Console.WriteLine("Welcome to the sinus data generator");

do
{
    // Ask user for data generation options
    int numberOfValues = GetUserInput("Please enter the number of values to generate (500 is used if you just press enter):", 500);
    double frequency = GetUserInput("Please enter the frequency of the sinus curve (1 is used if you just press enter):", 1.0);
    double amplitude = GetUserInput("Please enter the amplitude of the sinus curve (3 is used if you just press enter):", 3.0);
    double outlierPercentage = GetUserInput("Please enter the percentage of outliers (2 is used if you just press enter):", 2.0);
    double outlierFactor = GetUserInput("Please enter the factor of outliers (20 is used if you just press enter):", 20.0);

    // Generate data
    var options = new GenerationOptions(numberOfValues, frequency, amplitude, outlierPercentage, outlierFactor);
    var values = GenerateSinusData(options.NumberOfValues, options.OutlierPercentage, options.Amplitude, options.Frequency);
    var removed = RemoveOutliers(values);
    var removed2 = RemoveOutliers(removed);

    var plt = new ScottPlot.Plot();
    var xValues = Enumerable.Range(0, numberOfValues).Select(i => (double)i).ToArray();
    plt.Add.Scatter(xValues, values).Label = "Original";
    plt.Add.Scatter(xValues, removed).Label = "Removed once";
    plt.Add.Scatter(xValues, removed2).Label = "Removed twice";
    plt.SavePng("quickstart.png", 1920, 1024);
    Console.WriteLine("Diagram saved to quickstart.png\n");
}
while (true);

static T GetUserInput<T>(string prompt, T defaultValue)
{
    Console.WriteLine(prompt);
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        return defaultValue;

    return (T)Convert.ChangeType(input, typeof(T));
}

static List<double> GenerateSinusData(int numberOfPoints = 500, double outlierPercentage = 2, double amplitude = 1, double frequency = 1)
{
    List<double> sinusData = new List<double>();

    for (int i = 0; i < numberOfPoints; i++)
    {
        // Generate data points that follow the sinus curve  
        double x = 2 * Math.PI * i / numberOfPoints;
        double y = amplitude * Math.Sin(frequency * x);
        sinusData.Add(y);
    }

    // Calculate the number of outliers based on the percentage  
    int numberOfOutliers = (int)Math.Round(numberOfPoints * outlierPercentage / 100);

    // Add random outliers  
    for (int i = 0; i < numberOfOutliers; i++)
    {
        int randomIndex = Random.Shared.Next(numberOfPoints);
        double randomValue = amplitude * (2 * Random.Shared.NextDouble() - 1); // Random value between -amplitude and amplitude  
        sinusData[randomIndex] = randomValue;
    }

    return sinusData;
}

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

record GenerationOptions(int NumberOfValues, double Frequency, double Amplitude, double OutlierPercentage, double OutlierFactor);
