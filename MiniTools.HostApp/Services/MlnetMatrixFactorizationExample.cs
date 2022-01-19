using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace MiniTools.HostApp.Services;

internal class MlnetMatrixFactorizationExample
{
    public class MovieRating
    {
        [LoadColumn(0)]
        public float userId;
        [LoadColumn(1)]
        public float movieId;
        [LoadColumn(2)]
        public float Label;
    }

    public class MovieRatingPrediction
    {
        public float Label;
        public float Score;
    }

    const string dataSetPath = @"D:\src\github\mini-tools\DataSets";

    public void DoWork()
    {
        MLContext mlContext = new MLContext();

        // Load
        (IDataView trainingDataView, IDataView testDataView) = LoadData(mlContext);

        // Train
        ITransformer model = BuildAndTrainModel(mlContext, trainingDataView);

        // Eval
        EvaluateModel(mlContext, testDataView, model);

        // Usage
        UseModelForSinglePrediction(mlContext, model);

        SaveModel(mlContext, trainingDataView.Schema, model);
    }

    (IDataView training, IDataView test) LoadData(MLContext mlContext)
    {

        var trainingDataPath = Path.Combine(dataSetPath, "movie-ratings", "recommendation-ratings-train.csv");
        var testDataPath = Path.Combine(dataSetPath, "movie-ratings", "recommendation-ratings-test.csv");

        IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
        IDataView testDataView = mlContext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader: true, separatorChar: ',');

        return (trainingDataView, testDataView);
    }

    ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
    {
        IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "movieIdEncoded", inputColumnName: "movieId"));

        // Hyper-parameters
        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "userIdEncoded",
            MatrixRowIndexColumnName = "movieIdEncoded",
            LabelColumnName = "Label",
            NumberOfIterations = 20,
            ApproximationRank = 100
        };

        var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

        Console.WriteLine("=============== Training the model ===============");
        ITransformer model = trainerEstimator.Fit(trainingDataView);

        return model;

    }

    void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
    {
        Console.WriteLine("=============== Evaluating the model ===============");
        var prediction = model.Transform(testDataView);

        var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");

        Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
        Console.WriteLine("RSquared: " + metrics.RSquared.ToString());

    }

    void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
    {
        Console.WriteLine("=============== Making a prediction ===============");
        var predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);

        var testInput = new MovieRating { userId = 6, movieId = 10 };

        var movieRatingPrediction = predictionEngine.Predict(testInput);

        if (Math.Round(movieRatingPrediction.Score, 1) > 3.5)
        {
            Console.WriteLine("Movie " + testInput.movieId + " is recommended for user " + testInput.userId);
        }
        else
        {
            Console.WriteLine("Movie " + testInput.movieId + " is not recommended for user " + testInput.userId);
        }
    }

    void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
    {
        var modelPath = Path.Combine(dataSetPath, "MovieRecommenderModel.zip");

        Console.WriteLine("=============== Saving the model to a file ===============");
        mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
    }



}
