
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace MiniTools.HostApp.Services;

internal class MlnetTFTextClassificationExample
{
    /// <summary>
    /// Class to hold original sentiment data.
    /// </summary>
    public class MovieReview
    {
        public string ReviewText { get; set; }
    }

    /// <summary>
    /// Class to hold the variable length feature vector. Used to define the
    /// column names used as input to the custom mapping action.
    /// </summary>
    public class VariableLength
    {
        /// <summary>
        /// This is a variable length vector designated by VectorType attribute.
        /// Variable length vectors are produced by applying operations such as 'TokenizeWords' on strings
        /// resulting in vectors of tokens of variable lengths.
        /// </summary>
        [VectorType]
        public int[] VariableLengthFeatures { get; set; }
    }

    /// <summary>
    /// Class to hold the fixed length feature vector. Used to define the
    /// column names used as output from the custom mapping action,
    /// </summary>
    public class FixedLength
    {
        /// <summary>
        /// This is a fixed length vector designated by VectorType attribute.
        /// </summary>
        [VectorType(Config.FeatureLength)]
        public int[] Features { get; set; }
    }

    /// <summary>
    /// Class to contain the output values from the transformation.
    /// </summary>
    public class MovieReviewSentimentPrediction
    {
        [VectorType(2)]
        public float[] Prediction { get; set; }
    }

    static class Config
    {
        public const int FeatureLength = 600;
    }


    public void DoWork()
    {
        const string dataSetPath = @"D:\src\github\mini-tools\DataSets\";
        string _modelPath = Path.Combine(dataSetPath, "sentiment_model");

        MLContext mlContext = new MLContext();

        // Create a dictionary to encode words as integers using mapping in CSV file
        var lookupMap = mlContext.Data.LoadFromTextFile(Path.Combine(_modelPath, "imdb_word_index.csv"),
            columns: new[]
                {
                    new TextLoader.Column("Words", DataKind.String, 0),
                    new TextLoader.Column("Ids", DataKind.Int32, 1),
                },
            separatorChar: ','
            );

        // Resize variable length word integer array to an integer array of fixed size
        // Because the fixed size is what the model expects (consumes); cannot handle variable size
        Action<VariableLength, FixedLength> ResizeFeaturesAction = (s, f) =>
        {
            var features = s.VariableLengthFeatures;
            Array.Resize(ref features, Config.FeatureLength);
            f.Features = features;
        };

        // Load model

        TensorFlowModel tensorFlowModel = mlContext.Model.LoadTensorFlowModel(_modelPath);

        // extract its input and output schema; for information only
        DataViewSchema schema = tensorFlowModel.GetModelSchema();
        Console.WriteLine(" =============== TensorFlow Model Schema =============== ");
        var featuresType = (VectorDataViewType)schema["Features"].Type;
        Console.WriteLine($"Name: Features, Type: {featuresType.ItemType.RawType}, Size: ({featuresType.Dimensions[0]})");
        var predictionType = (VectorDataViewType)schema["Prediction/Softmax"].Type;
        Console.WriteLine($"Name: Prediction/Softmax, Type: {predictionType.ItemType.RawType}, Size: ({predictionType.Dimensions[0]})");

        // The input schema is the fixed-length array of integer encoded words.
        // The output schema is a float array of probabilities indicating whether a review's sentiment is negative, or positive
        // These values sum to 1, as the probability of being positive is the complement of the probability of the sentiment being negative.


        // 1.   Split the text into individual words using TokenizeIntoWords (this uses spaces to parse the text/string into words.
        //      It creates a new column and splits each input string to a vector of substrings based on the user-defined separator.
        // 2.   Map each word to an integer value. The array of integer makes up the input features.
        // 3.   Resize variable length vector to fixed length vector.
        // 4.   Passes the data to TensorFlow for scoring
        // 5.   Retrieves the 'Prediction' from TensorFlow and copies to a column
        IEstimator<ITransformer> pipeline = mlContext.Transforms.Text.TokenizeIntoWords("TokenizedWords", "ReviewText")
            .Append(mlContext.Transforms.Conversion.MapValue("VariableLengthFeatures", lookupMap, lookupMap.Schema["Words"], lookupMap.Schema["Ids"], "TokenizedWords"))
            .Append(mlContext.Transforms.CustomMapping(ResizeFeaturesAction, "Resize"))
            .Append(tensorFlowModel.ScoreTensorFlowModel("Prediction/Softmax", "Features"))
            .Append(mlContext.Transforms.CopyColumns("Prediction", "Prediction/Softmax"));

        // Create an executable model from the estimator pipeline
        IDataView dataView = mlContext.Data.LoadFromEnumerable(new List<MovieReview>());
        ITransformer model = pipeline.Fit(dataView);

        PredictSentiment(mlContext, model);
    }

    void PredictSentiment(MLContext mlContext, ITransformer model)
    {
        var engine = mlContext.Model.CreatePredictionEngine<MovieReview, MovieReviewSentimentPrediction>(model);
        var review = new MovieReview()
        {
            ReviewText = "this film is really good"
        };
        var sentimentPrediction = engine.Predict(review);

        Console.WriteLine("Number of classes: {0}", sentimentPrediction.Prediction.Length);
        Console.WriteLine("Is sentiment/review positive? {0}", sentimentPrediction.Prediction[1] > 0.5 ? "Yes." : "No.");


    }


}
