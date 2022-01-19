using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TimeSeries;

namespace MiniTools.HostApp.Services;

internal class MlnetAnomalyDetectionExample
{
    public class PhoneCallsData
    {
        [LoadColumn(0)]
        public string timestamp;

        [LoadColumn(1)]
        public double value;
    }

    public class PhoneCallsPrediction
    {
        //vector to hold anomaly detection results. Including isAnomaly, anomalyScore, magnitude, expectedValue, boundaryUnits, upperBoundary and lowerBoundary.
        [VectorType(7)]
        public double[] Prediction { get; set; }
    }

    const string dataSetPath = @"D:\src\github\mini-tools\DataSets";

    public void DoWork()
    {
        string _dataPath = Path.Combine(dataSetPath, "phone-calls", "phone-calls.csv");

        MLContext mlContext = new MLContext();

        // Load
        IDataView dataView = mlContext.Data.LoadFromTextFile<PhoneCallsData>(path: _dataPath, hasHeader: true, separatorChar: ',');


        // Detect seasonality for the series
        int period = DetectPeriod(mlContext, dataView);

        // Detect anomaly for the series with period information
        DetectAnomaly(mlContext, dataView, period);

        // Train

        // Eval

        // Use

    }

    int DetectPeriod(MLContext mlContext, IDataView phoneCalls)
    {
        Console.WriteLine("Detect period of the series");

        int period = mlContext.AnomalyDetection.DetectSeasonality(phoneCalls, nameof(PhoneCallsData.value));

        Console.WriteLine("Period of the series is: {0}.", period);

        return period;
    }

    void DetectAnomaly(MLContext mlContext, IDataView phoneCalls, int period)
    {
        Console.WriteLine("Detect anomaly points in the series");

        // Setup the parameters for SR-CNN
        var options = new SrCnnEntireAnomalyDetectorOptions()
        {
            Threshold = 0.3,
            Sensitivity = 64.0,
            DetectMode = SrCnnDetectMode.AnomalyAndMargin,
            Period = period,
        };

        // Detect anomaly using SR-CNN algorithm
        IDataView outputDataView =
            mlContext
                .AnomalyDetection.DetectEntireAnomalyBySrCnn(
                    phoneCalls,
                    nameof(PhoneCallsPrediction.Prediction),
                    nameof(PhoneCallsData.value),
                    options);

        IEnumerable<PhoneCallsPrediction> predictions = mlContext.Data.CreateEnumerable<PhoneCallsPrediction>(
            outputDataView, reuseRowObject: false);

        //                 12345   1234567   1234567890123   1234567890123   1234567890123
        Console.WriteLine("Index | Anomaly | ExpectedValue | UpperBoundary | LowerBoundary");

        var index = 0;

        foreach (var p in predictions)
        {
            if (p.Prediction[0] == 1)
            {
                Console.WriteLine("{0,5} | {1,7} | {2,13:F4} | {3,13:F4} | {4,13:F4}  <-- alert is on, detected anomaly", index,
                    p.Prediction[0], p.Prediction[3], p.Prediction[5], p.Prediction[6]);
            }
            else
            {
                Console.WriteLine("{0,5} | {1,7} | {2,13:F4} | {3,13:F4} | {4,13:F4}", index,
                    p.Prediction[0], p.Prediction[3], p.Prediction[5], p.Prediction[6]);
            }
            ++index;

        }

        Console.WriteLine("");
    }

}
