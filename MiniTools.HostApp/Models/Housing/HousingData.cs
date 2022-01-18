using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace MiniTools.HostApp.Models.Housing;

public class HousingData
{
    [LoadColumn(0)]
    public float Size { get; set; }

    [LoadColumn(1, 3)]
    [VectorType(3)]
    public float[] HistoricalPrices { get; set; }

    [LoadColumn(4)]
    [ColumnName("Label")]
    public float CurrentPrice { get; set; }
}

public class RegressionMlExample
{
    HousingData[] housingData = new HousingData[]
    {
        new HousingData
        {
            Size = 600f,
            HistoricalPrices = new float[] { 100000f, 125000f, 122000f },
            CurrentPrice = 170000f
        },
        new HousingData
        {
            Size = 1000f,
            HistoricalPrices = new float[] { 200000f, 250000f, 230000f },
            CurrentPrice = 225000f
        },
        new HousingData
        {
            Size = 1000f,
            HistoricalPrices = new float[] { 126000f, 130000f, 200000f },
            CurrentPrice = 195000f
        }
    };

    public void MakeModel()
    {
        // Create MLContext
        MLContext mlContext = new MLContext();

        // Load Data
        // Using in-memory; for other methods of loading data see:
        // https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/load-data-ml-net
        IDataView data = mlContext.Data.LoadFromEnumerable<HousingData>(housingData);
        

        // Define data preparation estimator
        EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> pipelineEstimator =
            mlContext.Transforms.Concatenate("Features", new string[] { "Size", "HistoricalPrices" })
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.Regression.Trainers.Sdca());

        // Train model
        ITransformer trainedModel = pipelineEstimator.Fit(data);

        // Save model
        mlContext.Model.Save(trainedModel, data.Schema, "model.zip");

        using FileStream stream = File.Create("./onnx_model.onnx");
        mlContext.Model.ConvertToOnnx(trainedModel, data, stream);

    }
}
