using Microsoft.ML;
using Microsoft.ML.Data;

namespace MiniTools.HostApp.Services;

internal class MlnetTFModelCompositionExample
{
    public class ImageData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;
    }

    public class ImagePrediction : ImageData
    {
        public float[] Score;

        public string PredictedLabelValue;
    }

    //  struct for Inception model parameters
    struct InceptionSettings
    {
        public const int ImageHeight = 224;
        public const int ImageWidth = 224;
        public const float Mean = 117;
        public const float Scale = 1;
        public const bool ChannelsLast = true;
    }

    const string dataSetPath = @"D:\src\github\mini-tools\DataSets";
    static string _assetsPath = Path.Combine(dataSetPath, "object-images");
    static string _imagesFolder = Path.Combine(_assetsPath, "images");
    static string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
    static string _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
    static string _predictSingleImage = Path.Combine(_imagesFolder, "toaster3.jpg");
    static string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");


    public void DoWork()
    {

        MLContext mlContext = new MLContext();

        // Load


        // Train
        ITransformer model = GenerateModel(mlContext);


        // Eval

        // Use
        ClassifySingleImage(mlContext, model);
    }

    ITransformer GenerateModel(MLContext mlContext)
    {
        // 1ab, The image transforms transform the images into the model's expected format.
        // 2ab  Loads the TensorFlow model into memory, then processes the vector of pixel values through the TensorFlow model network.
        //      Applying inputs to a deep learning model, and generating an output using the model, is referred to as Scoring
        // 3    Map Labels into numeric "LabelKey" for trainer "LbfgsMaximumEntropy" to consume
        // 4.   Add trainer
        // 5.   Map predicted value back to string
        IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))

            .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel)
            .ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
            
            .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
            .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
            
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
            .AppendCacheCheckpoint(mlContext);

        IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);

        ITransformer model = pipeline.Fit(trainingData);

        IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: _testTagsTsv, hasHeader: false);
        IDataView predictions = model.Transform(testData);

        // Create an IEnumerable for the predictions for displaying results
        IEnumerable<ImagePrediction> imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
        
        DisplayResults(imagePredictionData);

        MulticlassClassificationMetrics metrics = mlContext.MulticlassClassification.Evaluate(
            predictions,
            labelColumnName: "LabelKey",
            predictedLabelColumnName: "PredictedLabel");

        Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
        Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");

        return model;
    }


    void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
    {
        foreach (ImagePrediction prediction in imagePredictionData)
        {
            Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
        }
    }

    void ClassifySingleImage(MLContext mlContext, ITransformer model)
    {
        var imageData = new ImageData()
        {
            ImagePath = _predictSingleImage
        };

        // Make prediction function (input = ImageData, output = ImagePrediction)
        var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
        var prediction = predictor.Predict(imageData);

        Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");

    }


}
