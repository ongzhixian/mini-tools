using System.Security.Cryptography;

namespace MiniTools.HostApp.Models.MonteCarlo;


public interface ICard
{
    int Id { get; init; }
}
//internal record Card<T> : ICard where T: struct
//{
//    public int Id { get; set; }
//    public T Value { get; set; }
//    public string Description { get; set; } = string.Empty;
//}


public enum PokerCardSuit
{
    Spade,
    Heart,
    Club,
    Diamond
}

public enum PokerCardRank
{
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}

public enum PokerHand
{
    NO_PAIR,
    ONE_PAIR,
    TWO_PAIRS,
    THREE_OF_A_KIND,
    STRAIGHT,
    FLUSH,
    FULL_HOUSE,
    FOUR_OF_A_KIND,
    STRAIGHT_FLUSH,
    FIVE_OF_A_KIND
}

public record PokerCard : ICard
{
    public int Id { get; init; }

    public PokerCardSuit Suit { get; init; }
    public PokerCardRank Rank { get; init; }

}

public record PokerDeck
{
    IList<PokerCard> Cards { get; set; }

    public PokerDeck()
    {
        int i = 0;
        Cards = new List<PokerCard>();
        foreach (PokerCardSuit suit in Enum.GetValues(typeof(PokerCardSuit)))
            foreach (PokerCardRank rank in Enum.GetValues(typeof(PokerCardRank)))
                Cards.Add(new PokerCard
                {
                    Id = i++,
                    Suit = suit,
                    Rank = rank
                });
    }

    public void Shuffle()
    {
        int count = Cards.Count;
        IList<PokerCard> newPile = new List<PokerCard>();

        for (int i = 0; i < count; i++)
        {
            var idx = RandomNumberGenerator.GetInt32(Cards.Count);
            newPile.Add(Cards[idx]);
            Cards.Remove(Cards[idx]);
        }

        Cards = newPile;
    }

    public void PrintCards()
    {
        Console.WriteLine("Total cards: {0}", Cards.Count);
        foreach (PokerCard card in Cards)
        {
            Console.WriteLine(card);
        }
    }

    public PokerCard DrawCard()
    {
        var card = Cards[0];
        Cards.Remove(card);
        return card;
    }

    public static PokerHand? HandValue(IList<PokerCard> cards)
    {
        if (cards.Count != 5)
            return null;

        PokerHand result = PokerHand.NO_PAIR;

        //NO_PAIR,                                                  RG  RGC
        //ONE_PAIR,         // TWO_OF_A_KIND        ok              1   2
        //TWO_PAIRS,        // TWO TWO_OF_A_KIND    ok              2   2,2
        //THREE_OF_A_KIND,  //ok                                    1   3
        //STRAIGHT,         Five cards in sequence                  0   0
        //FLUSH,            Five cards of same suit                 ?   ?
        //FULL_HOUSE,       //ok                                    2   2,3
        //FOUR_OF_A_KIND,   //ok                                    1   4
        //STRAIGHT_FLUSH,   Five cards in sequence of same suit     0   0
        //FIVE_OF_A_KIND    //ignore                                1   5

        IEnumerable<IGrouping<PokerCardRank, PokerCard>>? rankGroups = cards.GroupBy(r => r.Rank).Where(g => g.Count() > 1);

        var rankedGroupsCount = rankGroups.Count();

        int maxGroup = 0;
        bool hasRankedGroup = rankedGroupsCount > 0;
        if (hasRankedGroup)
            maxGroup = rankGroups.Max(g => g.Count());

        var ranked = cards.OrderBy(q => q.Rank);
        var isStraight = ranked.Zip(ranked.Skip(1), (a, b) => (a.Rank + 1) == b.Rank).All(x => x) ||
            (ranked.Select(q => q.Rank).Intersect(new List<PokerCardRank>
            {
                PokerCardRank.Ace,
                PokerCardRank.King,
                PokerCardRank.Queen,
                PokerCardRank.Jack,
                PokerCardRank.Ten
            }).Count() == 5);

        var isFlush = cards.Zip(cards.Skip(1), (a, b) => a.Suit == b.Suit).All(x => x);

        // Eval

        if (isFlush && isStraight)
            return PokerHand.STRAIGHT_FLUSH;

        if ((rankedGroupsCount == 1) && (maxGroup == 4))
            return PokerHand.FOUR_OF_A_KIND;

        if ((rankedGroupsCount == 2) && (maxGroup == 3))
            return PokerHand.FULL_HOUSE;

        if (isFlush)
            return PokerHand.FLUSH;

        if (isStraight)
            return PokerHand.STRAIGHT;

        if ((rankedGroupsCount == 1) && (maxGroup == 3))
            return PokerHand.THREE_OF_A_KIND;

        if (rankedGroupsCount == 2)
            return PokerHand.TWO_PAIRS;

        if (rankedGroupsCount == 1)
            return PokerHand.ONE_PAIR;

        return PokerHand.NO_PAIR;


        //if (isStraight)
        //    result = PokerHand.STRAIGHT;

        //if (isFlush)
        //    result = PokerHand.FLUSH;


        //if (rankGroups.Count() == 2)
        //{
        //    if (rankGroups.Any(x => x.Count() == 3))
        //        result = PokerHand.FULL_HOUSE;
        //    else
        //        result = PokerHand.TWO_PAIRS;
        //}

        //if (rankGroups.Count() == 1)
        //{
        //    switch (rankGroups.First().Count())
        //    {
        //        case 2:
        //            result = PokerHand.ONE_PAIR;
        //            break;
        //        case 3:
        //            result = PokerHand.THREE_OF_A_KIND;
        //            break;
        //        case 4:
        //            result = PokerHand.FOUR_OF_A_KIND;
        //            break;
        //        default:
        //            break;
        //    }

        //}

        //return result;
    }
}

public class CardTest
{
    public void DoTest()
    {

        MiniTools.HostApp.Models.MonteCarlo.PokerDeck d = new MiniTools.HostApp.Models.MonteCarlo.PokerDeck();
        d.PrintCards();
        d.Shuffle();
        d.PrintCards();

        Console.WriteLine("HAND");
        IList<PokerCard> hand = new List<PokerCard>();
        //hand.Add(d.DrawCard());
        //hand.Add(d.DrawCard());
        //hand.Add(d.DrawCard());
        //hand.Add(d.DrawCard());
        //hand.Add(d.DrawCard());

        //foreach (var item in hand)
        //{
        //    Console.WriteLine(item);
        //}

        //hand = new List<PokerCard>()
        //{
        //    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Jack, },
        //    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Ace},
        //    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Two},
        //    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Queen},
        //    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Jack}
        //};

        var pair = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Ace },
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Two },
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Jack }
});
        Console.WriteLine("Expect PAIR; Result is {0}", pair);

        var two_pair = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Two },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Jack }
});
        Console.WriteLine("Expect TWO PAIR; Result is {0}", two_pair);

        var three_kind = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Diamond, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Two },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Jack }
});
        Console.WriteLine("Expect Three of a kind; Result is {0}", three_kind);

        var four_kind = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Diamond, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Heart, Rank = PokerCardRank.Jack},
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Jack }
});
        Console.WriteLine("Expect four of a kind; Result is {0}", four_kind);


        var full_house = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Spade, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Diamond, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Heart, Rank = PokerCardRank.Queen},
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Jack }
});
        Console.WriteLine("Expect full house; Result is {0}", full_house);


        var flush = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Ace },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Two },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Three },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Jack }
});
        Console.WriteLine("Expect flush; Result is {0}", flush);



        var wheelStraight = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Ace },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Two },
    new PokerCard { Suit = PokerCardSuit.Diamond, Rank = PokerCardRank.Three },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Four },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Five }
});
        Console.WriteLine("Expect straight; Result is {0}", wheelStraight);



        var broadwayStraight = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.King },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Ace},
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Jack },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Ten }
});
        Console.WriteLine("Expect straight; Result is {0}", broadwayStraight);


        var zero = PokerDeck.HandValue(new List<PokerCard>()
{
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Queen },
    new PokerCard { Suit = PokerCardSuit.Diamond, Rank = PokerCardRank.King },
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Ace},
    new PokerCard { Suit = PokerCardSuit.Heart, Rank = PokerCardRank.Three},
    new PokerCard { Suit = PokerCardSuit.Club, Rank = PokerCardRank.Ten }
});
        Console.WriteLine("Expect zero; Result is {0}", zero);



    }
}