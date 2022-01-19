using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Data.SqlClient;

namespace MiniTools.HostApp.Services;

internal class MlnetTimeSeriesExample
{
    public class ModelInput
    {

        //RentalDate: The date of the observation.
        //Year: The encoded year of the observation(0=2011, 1=2012).
        //TotalRentals: The total number of bike rentals for that day.

        public DateTime RentalDate { get; set; }

        public float Year { get; set; }

        public float TotalRentals { get; set; }
    }

    public class ModelOutput
    {

        //ForecastedRentals: The predicted values for the forecasted period.
        //LowerBoundRentals: The predicted minimum values for the forecasted period.
        //UpperBoundRentals: The predicted maximum values for the forecasted period.

        public float[] ForecastedRentals { get; set; }

        public float[] LowerBoundRentals { get; set; }

        public float[] UpperBoundRentals { get; set; }
    }

    public void DoWork()
    {
        string rootDir = @"D:\src\github\mini-tools\DataSets";
        string dbFilePath = Path.Combine(rootDir, "bike-demand", "DailyDemand.mdf");
        string modelPath = Path.Combine(rootDir, "bike-demand-Model.zip");
        var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbFilePath};Integrated Security=True;Connect Timeout=30;";

        MLContext mlContext = new MLContext();

        // Load
        DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();

        // Why CAST? 
        // ML.NET algorithms expect data to be of type Single.
        // Therefore, numerical values coming from the database that are not of type Real,
        // a single-precision floating-point value, have to be converted to Real.
        string query = "SELECT RentalDate, CAST(Year as REAL) as Year, CAST(TotalRentals as REAL) as TotalRentals FROM Rentals";

        DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, connectionString, query);

        IDataView dataView = loader.Load(dbSource);

        // The dataset contains two years worth of data.
        // data from the first year is used for training,
        // the second year is held out to compare the actual values against the forecast produced by the model.
        // Filter the data using the FilterRowsByColumn transform.
        // To get data for the first year, (ie. only the values in the Year column less than 1 are selected), set the upperBound parameter to 1.
        // To get data for the second year, (values greater than or equal to 1 are selected) set the lowerBound parameter to 1.

        IDataView firstYearData = mlContext.Data.FilterRowsByColumn(dataView, "Year", upperBound: 1);
        IDataView secondYearData = mlContext.Data.FilterRowsByColumn(dataView, "Year", lowerBound: 1);

        // Train

        // Define a pipeline that uses the SsaForecastingEstimator to forecast values in a time-series dataset.

        // Singular Spectrum Analysis(SSA).
        // SSA works by decomposing a time-series into a set of principal components.
        // These components can be interpreted as the parts of a signal that correspond to trends, noise, seasonality, and many other factors.
        // Then, these components are reconstructed and used to forecast values some time in the future.

        // The forecastingPipeline takes 365 data points for the first year and samples or
        // splits the time-series dataset into 30-day (monthly) intervals as specified by the seriesLength parameter. 
        // Each of these samples is analyzed through weekly or a 7-day window.
        // When determining what the forecasted value for the next period(s) is, the values from previous seven days are used to make a prediction.
        // The model is set to forecast seven periods into the future as defined by the horizon parameter.
        // Because a forecast is an informed guess, it's not always 100% accurate. 
        // Therefore, it's good to know the range of values in the best and worst-case scenarios
        // as defined by the upper and lower bounds.
        // In this case, the level of confidence for the lower and upper bounds is set to 95%.
        // The confidence level can be increased or decreased accordingly.
        // The higher the value, the wider the range is between the upper and lower bounds to achieve
        // the desired level of confidence.

        var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
            outputColumnName: "ForecastedRentals",
            inputColumnName: "TotalRentals",
            windowSize: 7,
            seriesLength: 30,
            trainSize: 365,
            horizon: 7,
            confidenceLevel: 0.95f,
            confidenceLowerBoundColumn: "LowerBoundRentals",
            confidenceUpperBoundColumn: "UpperBoundRentals");

        SsaForecastingTransformer forecaster = forecastingPipeline.Fit(firstYearData);

        Evaluate(secondYearData, forecaster, mlContext);

        var forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
        forecastEngine.CheckPoint(mlContext, modelPath);

        Forecast(secondYearData, 7, forecastEngine, mlContext);

    }


    void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
    {
        // Make predictions
        IDataView predictions = model.Transform(testData);

        // Actual values
        IEnumerable<float> actual =
            mlContext.Data.CreateEnumerable<ModelInput>(testData, true)
                .Select(observed => observed.TotalRentals);

        // Predicted values
        IEnumerable<float> forecast =
            mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true)
                .Select(prediction => prediction.ForecastedRentals[0]);

        // Calculate error (actual - forecast)
        var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

        // Get metric averages
        var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
        var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

        // Output metrics
        Console.WriteLine("Evaluation Metrics");
        Console.WriteLine("---------------------");
        Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
        Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");


        // Mean Absolute Error: Measures how close predictions are to the actual value.This value ranges between 0 and infinity. The closer to 0, the better the quality of the model.
        // Root Mean Squared Error: Summarizes the error in the model. This value ranges between 0 and infinity. The closer to 0, the better the quality of the model.

    }

    void Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecaster, MLContext mlContext)
    {

        ModelOutput forecast = forecaster.Predict();

        IEnumerable<string> forecastOutput =
            mlContext.Data.CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
                .Take(horizon)
                .Select((ModelInput rental, int index) =>
                {
                    string rentalDate = rental.RentalDate.ToShortDateString();
                    float actualRentals = rental.TotalRentals;
                    float lowerEstimate = Math.Max(0, forecast.LowerBoundRentals[index]);
                    float estimate = forecast.ForecastedRentals[index];
                    float upperEstimate = forecast.UpperBoundRentals[index];
                    return $"Date: {rentalDate}\n" +
                    $"Actual Rentals: {actualRentals}\n" +
                    $"Lower Estimate: {lowerEstimate}\n" +
                    $"Forecast: {estimate}\n" +
                    $"Upper Estimate: {upperEstimate}\n";
                });

        // Output predictions
        Console.WriteLine("Rental Forecast");
        Console.WriteLine("---------------------");
        foreach (var prediction in forecastOutput)
        {
            Console.WriteLine(prediction);
        }
    }
}
