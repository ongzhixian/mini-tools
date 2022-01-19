using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;

namespace MiniTools.HostApp.Services;

/// <summary>
/// Based on https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis
/// </summary>
internal class MlnetBinaryClassificationExample
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText;

        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment;
    }

    public class SentimentPrediction : SentimentData
    {

        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }

    string _dataPath = Path.Combine(@"D:/src/github/mini-tools/DataSets", "sentiment-labelled-sentences", "yelp_labelled.txt");



    public void DoWork()
    {
        MLContext mlContext = new MLContext();

        // Load
        TrainTestData splitDataView = LoadData(mlContext);


        // Train
        ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);

        // U
        Evaluate(mlContext, model, splitDataView.TestSet);

        UseModelWithSingleItem(mlContext, model);

        UseModelWithBatchItems(mlContext, model);

    }

    TrainTestData LoadData(MLContext mlContext)
    {
        IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);

        TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

        return splitDataView;
    }

    ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
    {
        // Use FeatureizeText() to convert SentimentText into a numeric key type into a column call "Features"
        // Then apply a learning algo (SdcaLogisticRegressionBinaryTrainer)
        var estimator = mlContext.Transforms.Text.FeaturizeText(
            outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

        Console.WriteLine("=============== Create and Train the Model ===============");
        var model = estimator.Fit(splitTrainSet);
        Console.WriteLine("=============== End of training ===============");
        Console.WriteLine();

        return model;
    }

    void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
    {
        Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
        IDataView predictions = model.Transform(splitTestSet);

        CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

        Console.WriteLine();
        Console.WriteLine("Model quality metrics evaluation");
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"     Auc: {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($" F1Score: {metrics.F1Score:P2}");
        Console.WriteLine("=============== End of model evaluation ===============");

        // The Accuracy metric gets the accuracy of a model, which is the proportion of correct predictions in the test set.
        // The AreaUnderRocCurve metric indicates how confident the model is correctly classifying the positive and negative classes.
        // You want the AreaUnderRocCurve to be as close to one as possible.
        // The F1Score metric gets the model's F1 score, which is a measure of balance between precision and recall.
        // You want the F1Score to be as close to one as possible.

    }

    void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
    {

        // Warning: PredictionEngine is not threadsafe
        // See: https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/serve-model-web-api-ml-net#register-predictionenginepool-for-use-in-the-application
        // for usage of PredictionEnginePool in ASP.NET Core Web API.

        PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

        SentimentData sampleStatement = new SentimentData
        {
            SentimentText = "This was a very bad steak"
        };

        var resultPrediction = predictionFunction.Predict(sampleStatement);

        Console.WriteLine();
        Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

        Console.WriteLine();
        Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");

        Console.WriteLine("=============== End of Predictions ===============");
        Console.WriteLine();

    }

    void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
    {
        IEnumerable<SentimentData> sentiments = new[]
        {
            new SentimentData
            {
                SentimentText = "This was a horrible meal"
            },
            new SentimentData
            {
                SentimentText = "I love this spaghetti."
            }
        };

        IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

        IDataView predictions = model.Transform(batchComments);

        // Use model to predict whether comment data is Positive (1) or Negative (0).
        IEnumerable<SentimentPrediction> predictedResults = 
            mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);

        Console.WriteLine();
        Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");

        foreach (SentimentPrediction prediction in predictedResults)
        {
            Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
        }
        Console.WriteLine("=============== End of predictions ===============");

    }



}
