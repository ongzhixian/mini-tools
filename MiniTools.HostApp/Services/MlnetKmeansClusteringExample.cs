using Microsoft.ML;
using Microsoft.ML.Data;

namespace MiniTools.HostApp.Services;

internal class MlnetKmeansClusteringExample
{
    public class IrisData
    {
        [LoadColumn(0)]
        public float SepalLength;

        [LoadColumn(1)]
        public float SepalWidth;

        [LoadColumn(2)]
        public float PetalLength;

        [LoadColumn(3)]
        public float PetalWidth;
    }

    public class ClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;

        [ColumnName("Score")]
        public float[] Distances;
    }

    const string _dataSetPath = @"D:\src\github\mini-tools\DataSets";
    string _dataPath = Path.Combine(_dataSetPath, "iris", "iris.data");
    string _modelPath = Path.Combine(_dataSetPath, "iris", "IrisClusteringModel.zip");

    public void DoWork()
    {
        var mlContext = new MLContext(seed: 0);

        // Load
        IDataView dataView = mlContext.Data.LoadFromTextFile<IrisData>(_dataPath, hasHeader: false, separatorChar: ',');

        // Train
        string featuresColumnName = "Features";
        var pipeline = mlContext.Transforms
            .Concatenate(featuresColumnName, "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")
            .Append(mlContext.Clustering.Trainers.KMeans(featuresColumnName, numberOfClusters: 3));
        var model = pipeline.Fit(dataView);
        using (var fileStream = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            mlContext.Model.Save(model, dataView.Schema, fileStream);
        }

        // Eval

        // Usage
        var predictor = mlContext.Model.CreatePredictionEngine<IrisData, ClusterPrediction>(model);
        var prediction = predictor.Predict(new IrisData
        {
            SepalLength = 5.1f,
            SepalWidth = 3.5f,
            PetalLength = 1.4f,
            PetalWidth = 0.2f
        });
        Console.WriteLine($"Cluster: {prediction.PredictedClusterId}");
        Console.WriteLine($"Distances: {string.Join(" ", prediction.Distances)}");
    }
}
