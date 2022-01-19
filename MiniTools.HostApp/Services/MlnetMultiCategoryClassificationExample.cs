using Microsoft.ML;
using Microsoft.ML.Data;

namespace MiniTools.HostApp.Services;

internal class MlnetMultiCategoryClassificationExample
{
    public class GitHubIssue
    {
        [LoadColumn(0)]
        public string ID { get; set; }
        [LoadColumn(1)]
        public string Area { get; set; }
        [LoadColumn(2)]
        public string Title { get; set; }
        [LoadColumn(3)]
        public string Description { get; set; }
    }

    public class IssuePrediction
    {
        [ColumnName("PredictedLabel")]
        public string Area;
    }

    const string _dataSetsPath = @"D:\src\github\mini-tools\DataSets";
    string _trainDataPath = Path.Combine(_dataSetsPath, "github-issues", "issues_train.tsv");
    string _testDataPath = Path.Combine(_dataSetsPath, "github-issues", "issues_test.tsv");
    string _modelPath = Path.Combine(_dataSetsPath, "multi-class-model.zip");

    MLContext _mlContext;
    PredictionEngine<GitHubIssue, IssuePrediction> _predEngine;
    ITransformer _trainedModel;
    IDataView _trainingDataView;

    public void DoWork()
    {
        _mlContext = new MLContext(seed: 0);

        // Load

        _trainingDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(_trainDataPath, hasHeader: true);

        // Train
        var pipeline = ProcessData();
        var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);

        // Eval
        Evaluate(_trainingDataView.Schema);

        // Usage 
        PredictIssue();

    }

    IEstimator<ITransformer> ProcessData()
    {
        // 1. Transform the predicator "Area" column into a numeric key type Label column
        // 2. transforms the text(Title and Description) columns into a numeric vector for each called TitleFeaturized and DescriptionFeaturized.
        // 3. combines all of the feature columns into the Features column using the Concatenate() method.
        //    By default, a learning algorithm processes only features from the Features column. 
        // Add AppendCacheCheckpoint to cache the DataView so when you iterate over the data multiple times, using the cache might get better performance
        // Use AppendCacheCheckpoint for small/medium datasets to lower training time.
        // Do NOT use it (remove .AppendCacheCheckpoint()) when handling very large datasets.

        var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Area", outputColumnName: "Label")
            .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Title", outputColumnName: "TitleFeaturized"))
            .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Description", outputColumnName: "DescriptionFeaturized"))
            .Append(_mlContext.Transforms.Concatenate("Features", "TitleFeaturized", "DescriptionFeaturized"))
            .AppendCacheCheckpoint(_mlContext);

        return pipeline;
    }

    IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
    {
        var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
        
        _trainedModel = trainingPipeline.Fit(trainingDataView);

        _predEngine = _mlContext.Model.CreatePredictionEngine<GitHubIssue, IssuePrediction>(_trainedModel);

        GitHubIssue issue = new GitHubIssue()
        {
            Title = "WebSockets communication is slow in my machine",
            Description = "The WebSockets communication used under the covers by SignalR looks like is going slow in my development machine.."
        };

        var prediction = _predEngine.Predict(issue);

        Console.WriteLine($"=============== Single Prediction just-trained-model - Result: {prediction.Area} ===============");

        return trainingPipeline;

    }
    void Evaluate(DataViewSchema trainingDataViewSchema)
    {
        var testDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(_testDataPath, hasHeader: true);

        var testMetrics = _mlContext.MulticlassClassification.Evaluate(_trainedModel.Transform(testDataView));

        Console.WriteLine($"*************************************************************************************************************");
        Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
        Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
        Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
        Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
        Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
        Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
        Console.WriteLine($"*************************************************************************************************************");

        // The following metrics are evaluated for multiclass classification:
        // Micro Accuracy - Every sample -class pair contributes equally to the accuracy metric.
        //      You want Micro Accuracy to be as close to one as possible.
        // Macro Accuracy - Every class contributes equally to the accuracy metric. Minority classes are given equal weight as the larger classes.
        //      You want Macro Accuracy to be as close to one as possible.
        // Log-loss - The smaller log loss is, the more accurate a classifier is.
        //      You want Log-loss to be as close to zero as possible.
        // Log-loss reduction - Ranges from[-inf, 1.00], where 1.00 is perfect predictions and 0 indicates mean predictions.
        //      You want Log-loss reduction to be as close to one as possible.

        SaveModelAsFile(_mlContext, trainingDataViewSchema, _trainedModel);

    }

    void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
    {
        mlContext.Model.Save(model, trainingDataViewSchema, _modelPath);
    }

    void PredictIssue()
    {
        ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

        GitHubIssue singleIssue = new GitHubIssue() { Title = "Entity Framework crashes", Description = "When connecting to the database, EF is crashing" };

        _predEngine = _mlContext.Model.CreatePredictionEngine<GitHubIssue, IssuePrediction>(loadedModel);

        var prediction = _predEngine.Predict(singleIssue);

        Console.WriteLine($"=============== Single Prediction - Result: {prediction.Area} ===============");

    }


}
