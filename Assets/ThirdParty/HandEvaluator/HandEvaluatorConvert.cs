using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Puppet.Poker.Models;

namespace HoldemHand
{
    public class HandEvaluatorConvert
	{
        public static string ConvertPokerCardToString(PokerCard card) {
            string suit = "";
            string rank = "";
            switch (card.GetSuit()) { 
                case Puppet.ECardSuit.Bitch :
                    suit = "c";
                    break;
                case Puppet.ECardSuit.Diamond :
                    suit = "d";
                    break;
                case Puppet.ECardSuit.Heart :
                    suit = "h";
                    break;
                case Puppet.ECardSuit.Spade :
                    suit = "s";
                    break;
            }
            switch (card.GetRank()) {
                case Puppet.ECardRank.King:
                    rank = "K";
                    break;
                case Puppet.ECardRank.Queen:
                    rank = "Q";
                    break;
                case Puppet.ECardRank.Ace:
                    rank = "A";
                    break;
                case Puppet.ECardRank.Jack:
                    rank = "J";
                    break;
                default :
                    rank = ((int)card.GetRank()).ToString();
                    break;
            }
            return rank + suit;
        }
        public static string ConvertPokerCardsToString(List<PokerCard> cards) {
            string hand = "";
            foreach (PokerCard item in cards)
            {
                hand += ConvertPokerCardToString(item) + " ";
            }
            return hand;
        }
	}
}
