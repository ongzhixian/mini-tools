using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTools.HostApp.Models.MonteCarlo;


public interface ICard
{
    int Id { get; set; }
}
internal record Card<T> : ICard where T: struct
{
    public int Id { get; set; }
    public T Value { get; set; }
    public string Description { get; set; } = string.Empty;
}


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

public record PokerCard
{
    public PokerCardSuit Suit;
    public PokerCardRank Rank;
}

public record PokerDeck
{
    IList<PokerCard> Cards { get; set; }

    public PokerDeck()
    {
        Cards = new List<PokerCard>();
        foreach (PokerCardSuit suit in Enum.GetValues(typeof(PokerCardSuit)))
            foreach (PokerCardRank rank in Enum.GetValues(typeof(PokerCardRank)))
                Cards.Add(new PokerCard
                {
                    Suit = suit,
                    Rank = rank
                });
    }

    public void PrintCards()
    {
        foreach (PokerCard card in Cards)
        {
            Console.WriteLine(card);
        }    
    }
}