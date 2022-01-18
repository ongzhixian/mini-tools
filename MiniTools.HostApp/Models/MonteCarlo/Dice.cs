using System.Security.Cryptography;
using System.Text;

namespace MiniTools.HostApp.Models.MonteCarlo;

internal class Dice
{
    public static int Roll(byte numberSides)
    {
        if (numberSides <= 0)
            throw new ArgumentOutOfRangeException("numberSides");

        return RandomNumberGenerator.GetInt32(numberSides) + 1;
    }

    public static int OldRoll(byte numberSides)
    {
        byte[] randomNumber;

        if (numberSides <= 0)
            throw new ArgumentOutOfRangeException("numberSides");

        do
        {
            randomNumber = RandomNumberGenerator.GetBytes(1);
        }
        while (!IsFairRoll(randomNumber[0], numberSides));
        
        // Return the random number mod the number of sides.
        // The possible values are zero-based, so we add one.
        return (byte)((randomNumber[0] % numberSides) + 1);
    }

    private static bool IsFairRoll(byte roll, byte numSides)
    {
        // There are MaxValue / numSides full sets of numbers that can come up in a single byte.
        // For instance, if we have a 6 sided die, there are 42 full sets of 1-6 that come up.
        // The 43rd set is incomplete.
        int fullSetsOfValues = Byte.MaxValue / numSides;

        // If the roll is within this range of fair values, then we let it continue.
        // In the 6 sided die case, a roll between 0 and 251 is allowed.
        // (We use< rather than <= since the = portion allows through an extra 0 value).
        // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair to use.
        return roll < numSides * fullSetsOfValues;
    }
}



public class DiceTest
{
    public void Test()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    Console.WriteLine("Result: {0}", MiniTools.HostApp.Models.MonteCarlo.Dice.Roll(6));
        //}
        //Console.WriteLine("OLD ROLL");
        //for (int i = 0; i < 10; i++)
        //{
        //    Console.WriteLine("Result: {0}", MiniTools.HostApp.Models.MonteCarlo.Dice.OldRoll(6));
        //}

    }
}