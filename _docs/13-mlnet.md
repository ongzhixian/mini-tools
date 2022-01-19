# ML.Net


# mlnet CLI

Use for generating sample C# project.

mlnet classification --dataset "C:\src\github.com\ongzhixian\mini-trade\sample-data\sentiment labelled sentences\yelp_labelled.txt" --label-col 1 --has-header false --train-time 10

Parameters
classification  for the ML task of classification
--dataset       uses the dataset file yelp_labelled.txt as training and testing dataset 
                (internally the CLI will either use cross-validation or split it in two datasets, 
                one for training and one for testing)
                where the objective/target column you want to predict (commonly called 'label') 
--label-col     is the column with index 1 (that is the second column, since the index is zero-based)
--has-header    does not use a file header with column names since this particular dataset file doesn't have a header
--train-time    the targeted exploration/train time for the experiment is 10 seconds


Analyze sentiment using the ML.NET CLI
https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis-cli
https://archive.ics.uci.edu/ml/datasets/Sentiment+Labelled+Sentences


# Examples

Binary classification
    Given data with 2 columns:
    Sentiment
    Positive/Negative
    Classify sentiments as positive or negative comment.

Multi-category classification
    Given data with 4 columns
    ID
    Area
    Title 
    Description
    We want to be able to determine "Area" (Label) from "Title" and "Description" (Features)

Regression
    Given data with 4 columns
    vendor_id,          CMT
    rate_code,          1
    passenger_count,    1
    trip_time_in_secs,  1271
    trip_distance,      3.8
    payment_type,       CRD
    fare_amount         17.5
    We want to determine fare_amount using features (vendor_id, rate_code, passenger_count, trip_distance, payment_type)
    
K-means Clustering (unsuperivsed)
    Given data with 2 columns    
    sepal length in centimeters
    sepal width in centimeters
    petal length in centimeters
    petal width in centimeters
    type of iris flower(ignored for this example)
    We want to determine the how many groups (type of flower) data set can be divided 
    using the other features.

Matrix Factorization (regression model)
    Given the following columns
    userId
    movieId
    rating
    timestamp

    One Class Matrix Factorization 	
        Use this when you only have userId and movieId. This style of recommendation is based upon the co-purchase scenario, or products frequently bought together, which means it will recommend to customers a set of products based upon their own purchase order history.
        https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/getting-started/MatrixFactorization_ProductRecommendation
        
    Field Aware Factorization Machines 	Use this to make recommendations when you have more Features beyond userId, productId, and rating (such as product description or product price). This method also uses a collaborative filtering approach.
        https://github.com/dotnet/machinelearning-samples/tree/main/samples/csharp/end-to-end-apps/Recommendation-MovieRecommender

Transfer Learning (image classification w/tensorflow)
    Given a set of cracked and uncracked concrete images
    Train a model that can determine if a given image is a cracked or uncracked concrete.
    Tensorflow model: 101-layer variant of the Residual Network (ResNet) v2 model. 
    This is a built-in pre-train model in `SciSharp.TensorFlow.Redist` package.
    model is trained to classify images into a thousand categories. 
    The model takes an image of size 224 x 224 as input and outputs the class probabilities for each of the classes it's trained on.
    
Model Composition (image classification -- another example of transfer learning w/tensorflow (inception))
    Given a set of object images, use Tensorflow Inception deep-learning model to classify the images.
    Loads a pre-trained Inception model (.pb file) rather than use a built-in pre-train model.

Time Series
    Given data with following columns:    
    dteday  : The date of the observation.
    year    : The encoded year of the observation (0=2011, 1=2012).
    cnt     : The total number of bike rentals for that day.
    We want to predict bike demand given features.
    Uses Access database as data source.

Anomaly Detection (time series)
    Time series anomaly detection is the process of detecting time-series data outliers; 
    points on a given input time-series where the behavior isn't what was expected, or "weird". 
    These anomalies are typically indicative of some events of interest in the problem domain: 
    a cyber-attack on user accounts, power outage, bursting RPS on a server, memory leak, etc.

    To find anomaly on time series, you should first determine the period of the series. 
    Then, the time series can be decomposed (decomposition) into several components as Y = T + S + R, where 
    Y is the original series, 
    T is the trend component, 
    S is the seasonal component, and 
    R is the residual component of the series.
    Detection is performed on the residual component to find the anomalies.
    mlnet uses SR-CNN algorithm -- an advanced and novel algorithm that is based on Spectral Residual (SR) 
    and Convolutional Neural Network(CNN) to detect anomaly on time-series

    Given data columns:
    timestamp
    value       (number of phone calls on date)

    Note: For anomaly detection, it does not follow the LTU (loadData, train, eval/use) pattern

Anomaly Detection 2 (non time-series)

    
    IID = Independent and Identically Distributed (aka random sample)
    Identically Distributed means that there are no overall trends–the distribution doesn’t fluctuate and all items in the sample are taken from the same probability distribution.
    Independent             means that the sample items are all independent events. In other words, they aren’t connected to each other in any way.
    In statistics, we usually say “random sample,” but in probability it’s more common to say “IID.” 
    A collection of random variables are IID if each random variable has the same probability distribution as the others and all are mutually independent
    IID Spike Detection or IID Change point Detection algorithms are suited for independent and identically distributed datasets. 
    They assume that your input data is a sequence of data points that are independently sampled from one stationary distribution.
    
    A stationary process (or a strict/strictly stationary process or strong/strongly stationary process) 
    is a stochastic process whose unconditional joint probability distribution does not change when shifted in time.
    Consequently, parameters such as mean and variance also do not change over time

    Anomaly Detection
        Spikes
        The goal of spike detection is to identify sudden yet temporary bursts that significantly differ from the majority of the time series data values. 
        It's important to detect these suspicious rare items, events, or observations in a timely manner to be minimized
        Change points
        Change points are persistent changes in a time series event stream distribution of values, like level changes and trends. 
        These persistent changes last much longer than spikes and could indicate catastrophic event(s).
        Change points are not usually visible to the naked eye (hence ML)


Object detection (note detection; not classification)
    Object detection is a computer vision problem. 
    While closely related to image classification, object detection performs image classification at a more granular scale. 
    Object detection both locates and categorizes entities within images.

    Tiny YOLOv2 pre-trained model
    Netron (optional)

    Tiny YOLOv2 is trained on the Pascal VOC dataset and is made up of 15 layers that can predict 20 different classes of objects. 
    Because Tiny YOLOv2 is a condensed version of the original YOLOv2 model, a tradeoff is made between speed and accuracy. 
    The different layers that make up the model can be visualized using tools like Netron.

    Inspecting the model would yield a mapping of the connections between all the layers that make up the neural network,
     where each layer would contain the name of the layer along with the dimensions of the respective input / output. 
     The data structures used to describe the inputs and outputs of the model are known as tensors. 
     Tensors can be thought of as containers that store data in N-dimensions. 
     In the case of Tiny YOLOv2, the name of the input layer is image and it expects a tensor of dimensions 3 x 416 x 416. 
     The name of the output layer is grid and generates an output tensor of dimensions 125 x 13 x 13

     The YOLO model takes an image 3(RGB) x 416px x 416px. 
     The model takes this input and passes it through the different layers to produce an output. 
     The output divides the input image into a 13 x 13 grid, with each cell in the grid consisting of 125 values.

    The model segments an image into a 13 x 13 grid, where each grid cell is 32px x 32px. 
    Each grid cell contains 5 potential object bounding boxes. A bounding box has 25 elements:

    1.  x the x position of the bounding box center relative to the grid cell it's associated with.
    2.  y the y position of the bounding box center relative to the grid cell it's associated with.
    3.  w the width of the bounding box.
    4.  h the height of the bounding box.
    5.  o the confidence value that an object exists within the bounding box, also known as objectness score.
    20. p1-p20 class probabilities for each of the 20 classes predicted by the model.

    In total, the 25 elements describing each of the 5 bounding boxes make up the 125 elements contained in each grid cell.
    The output generated by the pre-trained ONNX model is a float array of length 21125, 
    representing the elements of a tensor with dimensions 125 x 13 x 13. 
    In order to transform the predictions generated by the model into a tensor, 
    some post-processing work is required to parse the output (YoloParser).

    When the model makes a prediction (scoring), it divides the 416px x 416px input image into a grid of cells the size of 13 x 13. 
    Each cell contains is 32px x 32px. 
    Within each cell, there are 5 bounding boxes each containing 5 features (x, y, width, height, confidence). 
    In addition, each bounding box contains the probability of each of the classes, which in this case is 20. 
    Therefore, each cell contains 125 pieces of information (5 features + 20 class probabilities).

    Anchors are pre-defined height and width ratios of bounding boxes. 
    Most object or classes detected by a model have similar ratios. 
    This is valuable when it comes to creating bounding boxes. 
    Instead of predicting the bounding boxes, the offset from the pre-defined dimensions is calculated 
    therefore reducing the computation required to predict the bounding box. 
    Typically these anchor ratios are calculated based on the dataset used. 
    In this case, because the dataset is known and the values have been pre-computed, the anchors can be hard-coded.


Text Classification (w/TensorFlow)
    pre-trained TensorFlow model to classify sentiment in website comments. 
    The binary sentiment classifier is a C# console application developed using Visual Studio.
    
    saved_model.pb: the TensorFlow model itself. The model takes a fixed length (size 600) 
        integer array of features representing the text in an IMDB review string, and outputs two probabilities which sum to 1: 
        the probability that the input review has positive sentiment, and the probability that the input review has negative sentiment.
    imdb_word_index.csv: a mapping from individual words to an integer value. 
        The mapping is used to generate the input features for the TensorFlow model.




Aside:  If you are unsure about which Features might be the most relevant for your machine learning task, 
        you can also make use of Feature Contribution Calculation (FCC) and permutation feature importance (PFI), 
        which ML.NET provides to discover the most influential Features.


# Onnx

The Open Neural Network Exchange (ONNX) is an open source format for AI models. 
ONNX supports interoperability between frameworks. 
This means you can train a model in one of the many popular machine learning frameworks like PyTorch, 
convert it into ONNX format and consume the ONNX model in a different framework like ML.NET. 

The pre-trained Tiny YOLOv2 model is stored in ONNX format, a serialized representation of the layers and learned patterns of those layers. 
In ML.NET, interoperability with ONNX is achieved with the ImageAnalytics and OnnxTransformer NuGet packages. 

The ImageAnalytics package contains a series of transforms that take an image and encode it into numerical values 
that can be used as input into a prediction or training pipeline. 

The OnnxTransformer package leverages the ONNX Runtime to load an ONNX model and use it to make predictions based on input provided.


# Neural Networks

Deep learning is a subset of machine learning. 
To train deep learning models, large quantities of data are required. 
Patterns in the data are represented by a series of layers. 
The relationships in the data are encoded as connections between the layers containing weights. 
The higher the weight, the stronger the relationship. 
Collectively, this series of layers and connections are known as artificial neural networks. 
The more layers in a network, the "deeper" it is, making it a deep neural network.

There are different types of neural networks, the most common being 
Multi-Layered Perceptron (MLP), 
Convolutional Neural Network (CNN) and 
Recurrent Neural Network (RNN). 

The most basic is the MLP, which maps a set of inputs to a set of outputs. 
This neural network is good when the data does not have a spatial or time component. 

The CNN makes use of convolutional layers to process spatial information contained in the data. 
A good use case for CNNs is image processing to detect the presence of a feature in a region of an image 
(for example, is there a nose in the center of an image?). 

RNNs allow for the persistence of state or memory to be used as input. 
RNNs are used for time-series analysis, where the sequential ordering and context of events is important.


# References

How to install GPU support in Model Builder
https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/install-gpu-model-builder
https://developer.nvidia.com/cuda-10.1-download-archive-update2