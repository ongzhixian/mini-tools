using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace MiniTools.HostApp.Models.Housing;

public class HouseData
{
    public float Size { get; set; }
    public float Price { get; set; }
}

public class PricePrediction
{
    [ColumnName("Score")]
    public float Price { get; set; }

    public override string ToString()
    {
        return $"Price: {Price * 100:C}k";
    }
}


public interface IRegressionPredictor<in TDataType, out TPredictionType>
{
    TPredictionType Predict(TDataType data);
    void TrainModel();

    void LoadTrainingData();
}

internal class HousePricePredictor : IRegressionPredictor<HouseData, PricePrediction>
{
    readonly MLContext mlContext = new MLContext();

    IDataView? trainingData;

    ITransformer? model;

    public void LoadTrainingData()
    {
        // 1. Import or create training data
        HouseData[] houseData = {
               new HouseData() { Size = 1.1F, Price = 1.2F },
               new HouseData() { Size = 1.9F, Price = 2.3F },
               new HouseData() { Size = 2.8F, Price = 3.0F },
               new HouseData() { Size = 3.4F, Price = 3.7F } };

        trainingData = mlContext.Data.LoadFromEnumerable(houseData);
    }
    
    public void TrainModel()
    {

        // 2. Specify data preparation and model training pipeline
        var pipeline = mlContext.Transforms.Concatenate("Features", new[] { "Size" })
            .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Price", maximumNumberOfIterations: 100));

        // 3. Train model
        model = pipeline.Fit(trainingData);
    }

    public PricePrediction Predict(HouseData size)
    {

        // 4. Make a prediction
        //var size = new HouseData() { Size = 2.5F };
        var price = mlContext.Model.CreatePredictionEngine<HouseData, PricePrediction>(model).Predict(size);

        Console.WriteLine($"Predicted price for size: {size.Size * 1000} sq ft= {price.Price * 100:C}k");

        return price;

        // Predicted price for size: 2500 sq ft= $261.98k
    }

    //public void DoWork()
    //{
    //    MLContext mlContext = new MLContext();

    //    // 1. Import or create training data
    //    HouseData[] houseData = {
    //           new HouseData() { Size = 1.1F, Price = 1.2F },
    //           new HouseData() { Size = 1.9F, Price = 2.3F },
    //           new HouseData() { Size = 2.8F, Price = 3.0F },
    //           new HouseData() { Size = 3.4F, Price = 3.7F } };
    //    IDataView trainingData = mlContext.Data.LoadFromEnumerable(houseData);

    //    // 2. Specify data preparation and model training pipeline
    //    var pipeline = mlContext.Transforms.Concatenate("Features", new[] { "Size" })
    //        .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Price", maximumNumberOfIterations: 100));

    //    // 3. Train model
    //    var model = pipeline.Fit(trainingData);

    //    // 4. Make a prediction
    //    var size = new HouseData() { Size = 2.5F };
    //    var price = mlContext.Model.CreatePredictionEngine<HouseData, Prediction>(model).Predict(size);

    //    Console.WriteLine($"Predicted price for size: {size.Size * 1000} sq ft= {price.Price * 100:C}k");

    //    // Predicted price for size: 2500 sq ft= $261.98k

    //}


}
