using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

namespace MiniTools.HostApp.Services;

internal class MlnetAnomalyDetectionExample3
{
    private class TimeSeriesData
    {
        public float Value;

        public TimeSeriesData(float value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"Value {Value}";
        }
    }

    private class SrCnnAnomalyDetection
    {
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }

    public void DoWork2()
    {
        var ml = new MLContext();
        ITransformer model;
        using (var file = File.OpenRead("pred.zip"))
            model = ml.Model.Load(file, out DataViewSchema schema);
        var engine = model.CreateTimeSeriesEngine<TimeSeriesData, SrCnnAnomalyDetection>(ml);

        for (int index = 0; index < 15; index++)
        {
            // Anomaly detection.
            PrintPrediction(5, engine.Predict(new TimeSeriesData(5)));
        }

        PrintPrediction(10, engine.Predict(new TimeSeriesData(10)));

    }

    public void DoWork()
    {
        var ml = new MLContext();

        // Generate sample series data with an anomaly
        List<TimeSeriesData> data = GenerateSampleData();
        foreach (var item in data)
            Console.WriteLine(item);


        // Load (Convert data to IDataView)
        var dataView = ml.Data.LoadFromEnumerable(data);

        // Train
        // Setup the estimator arguments
        string outputColumnName = nameof(SrCnnAnomalyDetection.Prediction);
        string inputColumnName = nameof(TimeSeriesData.Value);

        // The transformed model.
        ITransformer model = ml.Transforms.DetectAnomalyBySrCnn(
            outputColumnName, 
            inputColumnName, 
            16, // window size (64)
            5,  // backAddWindowSize (5)
            5,  // lookaheadWindowSize (5)
            3,  // averagingWindowSize (3)
            8,  // judgementWindowSize (21)
            0.35 // threshold
            ).Fit(dataView);

        // Create a time series prediction engine from the model.
        var engine = model.CreateTimeSeriesEngine<TimeSeriesData, SrCnnAnomalyDetection>(ml);

        // Create non-anomalous data and check for anomaly.
        for (int index = 0; index < 20; index++)
        {
            // Anomaly detection.
            PrintPrediction(5, engine.Predict(new TimeSeriesData(5)));
        }

        PrintPrediction(10, engine.Predict(new TimeSeriesData(10)));

        engine.CheckPoint(ml, "pred.zip");

        using (var file = File.OpenRead("pred.zip"))
            model = ml.Model.Load(file, out DataViewSchema schema);

        Console.WriteLine("CHECKED");

        for (int index = 0; index < 5; index++)
        {
            // Anomaly detection.
            PrintPrediction(5, engine.Predict(new TimeSeriesData(5)));
        }

        for (int index = 0; index < 15; index++)
        {
            // Anomaly detection.
            PrintPrediction(5, engine.Predict(new TimeSeriesData(5)));
        }



    }

    private static void PrintPrediction(float value, SrCnnAnomalyDetection
    prediction) =>
    Console.WriteLine("{0}\t{1}\t{2:0.00}\t{3:0.00}", value, prediction
    .Prediction[0], prediction.Prediction[1], prediction.Prediction[2]);

    private static List<TimeSeriesData> GenerateSampleData()
    {
        var data = new List<TimeSeriesData>();
        for (int index = 0; index < 20; index++)
        {
            data.Add(new TimeSeriesData(5));
        }
        data.Add(new TimeSeriesData(10));
        for (int index = 0; index < 5; index++)
        {
            data.Add(new TimeSeriesData(5));
        }

        return data;
    }
}


