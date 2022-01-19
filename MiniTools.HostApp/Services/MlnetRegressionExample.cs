using Microsoft.ML;
using Microsoft.ML.Data;

namespace MiniTools.HostApp.Services;

internal class MlnetRegressionExample
{
    public class TaxiTrip
    {
        [LoadColumn(0)]
        public string VendorId;

        [LoadColumn(1)]
        public string RateCode;

        [LoadColumn(2)]
        public float PassengerCount;

        [LoadColumn(3)]
        public float TripTime;

        [LoadColumn(4)]
        public float TripDistance;

        [LoadColumn(5)]
        public string PaymentType;

        [LoadColumn(6)]
        public float FareAmount;
    }

    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }

    const string dataPath = @"D:\src\github\mini-tools\DataSets";

    string _trainDataPath = Path.Combine(dataPath, "taxi-fare", "taxi-fare-train.csv");
    string _testDataPath = Path.Combine(dataPath, "taxi-fare", "taxi-fare-test.csv");
    string _modelPath = Path.Combine(dataPath, "taxi-fare", "taxi-fare-Model.zip");

    public void DoWork()
    {
        MLContext mlContext = new MLContext(seed: 0);

        // Load
        // Train
        var model = Train(mlContext, _trainDataPath);

        // Eval
        Evaluate(mlContext, model);

        // Usage
        TestSinglePrediction(mlContext, model);

    }

    ITransformer Train(MLContext mlContext, string dataPath)
    {
        IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(dataPath, hasHeader: true, separatorChar: ',');

        // Copy FareCount to "Label" column
        // Convert cateorical data into numerical feature using OneHotEncoding (which assigns different numeric key values to the different values in each of the columns)
        // Make concat the multiple features into "Features" column
        var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "FareAmount")
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: "VendorId"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: "RateCode"))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: "PaymentType"))
            .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PassengerCount", "TripDistance", "PaymentTypeEncoded"))
            .Append(mlContext.Regression.Trainers.FastTree());

        var model = pipeline.Fit(dataView);
        
        return model;

    }

    void Evaluate(MLContext mlContext, ITransformer model)
    {
        IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(_testDataPath, hasHeader: true, separatorChar: ',');

        var predictions = model.Transform(dataView);

        var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

        Console.WriteLine();
        Console.WriteLine($"*************************************************");
        Console.WriteLine($"*       Model quality metrics evaluation         ");
        Console.WriteLine($"*------------------------------------------------");
        Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
        Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");

        // RSquared is another evaluation metric of the regression models.RSquared takes values between 0 and 1.
        //      The closer its value is to 1, the better the model is.
        // RMS is one of the evaluation metrics of the regression model.
        //      The lower it is, the better the model is. 

    }

    void TestSinglePrediction(MLContext mlContext, ITransformer model)
    {
        var predictionFunction = mlContext.Model.CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(model);

        var taxiTripSample = new TaxiTrip()
        {
            VendorId = "VTS",
            RateCode = "1",
            PassengerCount = 1,
            TripTime = 1140,
            TripDistance = 3.75f,
            PaymentType = "CRD",
            FareAmount = 0 // To predict. Actual/Observed = 15.5
        };

        var prediction = predictionFunction.Predict(taxiTripSample);

        Console.WriteLine($"**********************************************************************");
        Console.WriteLine($"Predicted fare: {prediction.FareAmount:0.####}, actual fare: 15.5");
        Console.WriteLine($"**********************************************************************");
    }



}
