using Microsoft.ML;
using Microsoft.ML.Data;

namespace MiniTools.HostApp.Services;

internal class MlnetAnomalyDetectionExample2
{
    public class ProductSalesData
    {
        [LoadColumn(0)]
        public string Month;

        [LoadColumn(1)]
        public float numSales;
    }

    public class ProductSalesPrediction
    {
        //vector to hold alert,score,p-value values
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }

    public void DoWork()
    {
        const string dataSetPath = @"D:\src\github\mini-tools\DataSets";

        string _dataPath = Path.Combine(dataSetPath, "product-sales", "product-sales.csv");
        //assign the Number of records in dataset file to constant variable
        const int _docsize = 36;


        MLContext mlContext = new MLContext();

        // Load
        IDataView dataView = mlContext.Data.LoadFromTextFile<ProductSalesData>(path: _dataPath, hasHeader: true, separatorChar: ',');

        // Train

        // Eval
        // Use
        DetectSpike(mlContext, _docsize, dataView);

        DetectChangepoint(mlContext, _docsize, dataView);


    }

    IDataView CreateEmptyDataView(MLContext mlContext)
    {
        // Create empty DataView. We just need the schema to call Fit() for the time series transforms
        IEnumerable<ProductSalesData> enumerableData = new List<ProductSalesData>();
        return mlContext.Data.LoadFromEnumerable(enumerableData);
    }

    void DetectSpike(MLContext mlContext, int docSize, IDataView productSales)
    {
        // The confidence and pvalueHistoryLength parameters impact how spikes are detected.
        // confidence determines how sensitive your model is to spikes.
        //      The lower the confidence, the more likely the algorithm is to detect "smaller" spikes.
        // The pvalueHistoryLength parameter defines the number of data points in a sliding window.
        //      The value of this parameter is usually a percentage of the entire dataset.
        //      The lower the pvalueHistoryLength, the faster the model forgets previous large spikes.

        var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(
            outputColumnName: nameof(ProductSalesPrediction.Prediction), 
            inputColumnName: nameof(ProductSalesData.numSales), 
            confidence: 95, 
            pvalueHistoryLength: docSize / 4);

        ITransformer iidSpikeTransform = iidSpikeEstimator.Fit(CreateEmptyDataView(mlContext));

        IDataView transformedData = iidSpikeTransform.Transform(productSales);

        var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

        Console.WriteLine("Alert\tScore\tP-Value");

        // 
        //Alert indicates a spike alert for a given data point.
        //Score is the ProductSales value for a given data point in the dataset.
        //P - Value The "P" stands for probability.The closer the p - value is to 0, the more likely the data point is an anomaly.


        foreach (var p in predictions)
        {
            var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}";

            if (p.Prediction[0] == 1)
            {
                results += " <-- Spike detected";
            }

            Console.WriteLine(results);
        }
        Console.WriteLine("");

    }

    void DetectChangepoint(MLContext mlContext, int docSize, IDataView productSales)
    {
        // The detection of change points happens with a slight delay as the model needs
        // to make sure the current deviation is a persistent change and not just some random spikes
        // before creating an alert.
        // The amount of this delay is equal to the changeHistoryLength parameter.
        // By increasing the value of this parameter, change detection alerts on more persistent changes,
        // but the trade-off would be a longer delay.

        var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(ProductSalesPrediction.Prediction), inputColumnName: nameof(ProductSalesData.numSales), confidence: 95, changeHistoryLength: docSize / 4);

        var iidChangePointTransform = iidChangePointEstimator.Fit(CreateEmptyDataView(mlContext));

        IDataView transformedData = iidChangePointTransform.Transform(productSales);

        var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

        Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");
        
        //Alert indicates a change point alert for a given data point.
        //Score is the ProductSales value for a given data point in the dataset.
        //P - Value The "P" stands for probability.The closer the P - value is to 0, the more likely the data point is an anomaly.
        //  Martingale value is used to identify how "weird" a data point is, based on the sequence of P - values.
      

        foreach (var p in predictions)
        {
            var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}\t{p.Prediction[3]:F2}";

            if (p.Prediction[0] == 1)
            {
                results += " <-- alert is on, predicted changepoint";
            }
            Console.WriteLine(results);
        }
        Console.WriteLine("");
    }

}
