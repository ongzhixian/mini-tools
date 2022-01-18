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

# References

How to install GPU support in Model Builder
https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/install-gpu-model-builder
https://developer.nvidia.com/cuda-10.1-download-archive-update2