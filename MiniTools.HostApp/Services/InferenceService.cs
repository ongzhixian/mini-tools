using Microsoft.ML.Probabilistic.Algorithms;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Math;
using Microsoft.ML.Probabilistic.Models;
using Range = Microsoft.ML.Probabilistic.Models.Range;

namespace MiniTools.HostApp.Services;

internal class InferenceService
{
    public static void TrueSkillExample()
    {
        // Based on https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/matchup-app-infer-net

        // The winner and loser in each of 6 samples games
        var winnerData = new[] { 0, 0, 0, 1, 3, 4 };
        var loserData = new[] { 1, 3, 4, 2, 1, 2 };

        // Define the statistical model as a probabilistic program
        var game = new Range(winnerData.Length);
        var player = new Range(winnerData.Concat(loserData).Max() + 1);
        var playerSkills = Variable.Array<double>(player);
        playerSkills[player] = Variable.GaussianFromMeanAndVariance(6, 9).ForEach(player);

        var winners = Variable.Array<int>(game);
        var losers = Variable.Array<int>(game);

        using (Variable.ForEach(game))
        {
            // The player performance is a noisy version of their skill
            var winnerPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[winners[game]], 1.0);
            var loserPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[losers[game]], 1.0);

            // The winner performed better in this game
            Variable.ConstrainTrue(winnerPerformance > loserPerformance);
        }

        // Attach the data to the model
        winners.ObservedValue = winnerData;
        losers.ObservedValue = loserData;

        // Run inference
        var inferenceEngine = new InferenceEngine();
        var inferredSkills = inferenceEngine.Infer<Gaussian[]>(playerSkills);

        // The inferred skills are uncertain, which is captured in their variance
        var orderedPlayerSkills = inferredSkills
    .Select((s, i) => new { Player = i, Skill = s })
    .OrderByDescending(ps => ps.Skill.GetMean());

        foreach (var playerSkill in orderedPlayerSkills)
        {
            Console.WriteLine($"Player {playerSkill.Player} skill: {playerSkill.Skill}");
        }
    }

    public static void TwoCoinsExample()
    {
        // Example -- Infers what will happen when two fair coins are tossed
        Variable<bool> firstCoin = Variable.Bernoulli(0.5);
        Variable<bool> secondCoin = Variable.Bernoulli(0.5);

        Variable<bool> bothHeads = firstCoin & secondCoin;

        InferenceEngine engine = new InferenceEngine();
        Console.WriteLine("Probability both coins are heads: " + engine.Infer(bothHeads));

        // Reverse inferencing; if both coins are not heads, whats the prob of firstCoin being head
        bothHeads.ObservedValue = false;
        Console.WriteLine("Probability distribution over firstCoin: " + engine.Infer(firstCoin));

    }

    public static void GaussianExample()
    {
        // Single
        //Variable<double> x = Variable.GaussianFromMeanAndVariance(0, 1).Named("x");
        //Variable.ConstrainTrue(x > 0.5);
        //InferenceEngine engine = new InferenceEngine();
        //engine.Algorithm = new ExpectationPropagation();
        //Console.WriteLine("Dist over x=" + engine.Infer(x));

        // Loop
        //for (double thresh = 0; thresh <= 1; thresh += 0.1)
        //{
        //    Variable<double> x = Variable.GaussianFromMeanAndVariance(0, 1).Named("x");
        //    Variable.ConstrainTrue(x > thresh);
        //    InferenceEngine engine = new InferenceEngine();
        //    engine.Algorithm = new ExpectationPropagation();
        //    Console.WriteLine("Dist over x given thresh of " + thresh + "=" + engine.Infer(x));
        //}


        Variable<double> threshold = Variable.New<double>().Named("threshold");
        Variable<double> x = Variable.GaussianFromMeanAndVariance(0, 1).Named("x");
        Variable.ConstrainTrue(x > threshold);
        InferenceEngine engine = new InferenceEngine();
        engine.Algorithm = new ExpectationPropagation();

        for (double thresh = 0; thresh <= 1; thresh += 0.1)
        {
            threshold.ObservedValue = thresh;
            Console.WriteLine("Dist over x given thresh of " + thresh + "=" + engine.Infer(x));
        }

    }

    public static void GaussianArrayExample1()
    {
        // Sample data from standard Gaussian  
        double[] data = new double[100];
        for (int i = 0; i < data.Length; i++)
            data[i] = Rand.Normal(0, 1);

        // Create mean and precision random variables  
        Variable<double> mean = Variable.GaussianFromMeanAndVariance(0, 100);
        Variable<double> precision = Variable.GammaFromShapeAndScale(1, 1);

        for (int i = 0; i < data.Length; i++)
        {
            Variable<double> x = Variable.GaussianFromMeanAndPrecision(mean, precision);
            x.ObservedValue = data[i];
        }

        InferenceEngine engine = new InferenceEngine();// Retrieve the posterior distributions  
        Console.WriteLine("mean=" + engine.Infer(mean));
        Console.WriteLine("prec=" + engine.Infer(precision));

    }

    public static void GaussianArrayExample2()
    {
        double[] data = new double[100];
        for (int i = 0; i < data.Length; i++)
            data[i] = Rand.Normal(0, 1);

        // Create mean and precision random variables  
        Variable<double> mean = Variable.GaussianFromMeanAndVariance(0, 100);
        Variable<double> precision = Variable.GammaFromShapeAndScale(1, 1);

        Range dataRange = new Range(data.Length).Named("n");
        VariableArray<double> x = Variable.Array<double>(dataRange);
        x[dataRange] = Variable.GaussianFromMeanAndPrecision(mean, precision).ForEach(dataRange);

        for (int i = 0; i < data.Length; i++)
        {

            x.ObservedValue = data;
        }

        InferenceEngine engine = new InferenceEngine();// Retrieve the posterior distributions  

        engine.ShowFactorGraph = true;
        Console.WriteLine("mean=" + engine.Infer(mean));
        Console.WriteLine("prec=" + engine.Infer(precision));
    }

    public static void BayesPointExample()
    {
        double[] incomes = { 63, 16, 28, 55, 22, 20 };
        double[] ages = { 38, 23, 40, 27, 18, 40 };
        bool[] willBuy = { true, false, true, true, false, false };

        // Create x vector, augmented by 1
        Vector[] xdata = new Vector[incomes.Length];
        for (int i = 0; i < xdata.Length; i++)
            xdata[i] = Vector.FromArray(incomes[i], ages[i], 1);
        VariableArray<Vector> x = Variable.Observed(xdata);

        // Create target y  
        VariableArray<bool> y = Variable.Observed(willBuy, x.Range);


        Variable<Vector> w = Variable.Random(new VectorGaussian(Vector.Zero(3), PositiveDefiniteMatrix.Identity(3)));
        Range j = y.Range;
        double noise = 0.1;
        y[j] = Variable.GaussianFromMeanAndVariance(Variable.InnerProduct(w, x[j]), noise) > 0;


        InferenceEngine engine = new InferenceEngine(new ExpectationPropagation());
        VectorGaussian wPosterior = engine.Infer<VectorGaussian>(w);
        Console.WriteLine("Dist over w=\n" + wPosterior);

        double[] incomesTest = { 58, 18, 22 };
        double[] agesTest = { 36, 24, 37 };
        VariableArray<bool> ytest = Variable.Array<bool>(new Range(agesTest.Length));
        BayesPointMachine(incomesTest, agesTest, Variable.Random(wPosterior), ytest);
        Console.WriteLine("output=\n" + engine.Infer(ytest));
    }

    public static void BayesPointMachine(double[] incomes, double[] ages, Variable<Vector> w, VariableArray<bool> y)
    {
        // Create x vector, augmented by 1 
        Range j = y.Range; Vector[] xdata = new Vector[incomes.Length];
        for (int i = 0; i < xdata.Length; i++)
            xdata[i] = Vector.FromArray(incomes[i], ages[i], 1);

        VariableArray<Vector> x = Variable.Observed(xdata, j); // Bayes Point Machine
        
        double noise = 0.1;

        y[j] = Variable.GaussianFromMeanAndVariance(Variable.InnerProduct(w, x[j]), noise) > 0;

    }

    public static void BayesSelectionExample()
    {
        // Data from clinical trial  
        VariableArray<bool> controlGroup =
            Variable.Observed(new bool[] { false, false, true, false, false });
        VariableArray<bool> treatedGroup =
            Variable.Observed(new bool[] { true, false, true, true, true });
        Range i = controlGroup.Range; Range j = treatedGroup.Range;

        // Prior on being effective treatment  
        Variable<bool> isEffective = Variable.Bernoulli(0.5);

        Variable<double> probIfTreated, probIfControl;
        
        using (Variable.If(isEffective))
        { // Model if treatment is effective
            probIfControl = Variable.Beta(1, 1);
            controlGroup[i] = Variable.Bernoulli(probIfControl).ForEach(i);
            probIfTreated = Variable.Beta(1, 1);
            treatedGroup[j] = Variable.Bernoulli(probIfTreated).ForEach(j);
        }

        using (Variable.IfNot(isEffective))
        { // Model if treatment is not effective
            Variable<double> probAll = Variable.Beta(1, 1);
            controlGroup[i] = Variable.Bernoulli(probAll).ForEach(i);
            treatedGroup[j] = Variable.Bernoulli(probAll).ForEach(j);
        }

        InferenceEngine engine = new InferenceEngine();
        engine.SaveFactorGraphToFolder = "graphs";

        Console.WriteLine("Probability treatment has an effect = " + engine.Infer(isEffective));
        Console.WriteLine("Probability of good outcome if given treatment = "
                           + (float)engine.Infer<Beta>(probIfTreated).GetMean());
        Console.WriteLine("Probability of good outcome if control = "
                           + (float)engine.Infer<Beta>(probIfControl).GetMean());



    }



    public static void StringExample()
    {
        Variable<string> str1 = Variable.StringUniform().Named("str1");
        Variable<string> str2 = Variable.StringUniform().Named("str2");

        Variable<string> text = (str1 + " " + str2).Named("text");

        text.ObservedValue = "Hello uncertain world";

        var engine = new InferenceEngine();

        Console.WriteLine("str1: {0}", engine.Infer(str1));
        Console.WriteLine("str2: {0}", engine.Infer(str2));

        var distOfStr1 = engine.Infer<StringDistribution>(str1);
        foreach (var s in new[] { "Hello", "Hello uncertain", "Hello uncertain world" })
        {
            Console.WriteLine("P(str1 = '{0}') = {1}", s, distOfStr1.GetProb(s));
        }

    }
}
