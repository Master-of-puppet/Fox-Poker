﻿using UnityEngine;
using System.Collections;
using Puppet.Poker.Models;

public class PokerCardObject : MonoBehaviour 
{
    static string[] BACKGROUND = new string[] { "card_up", "card_empty" };
    static string[] RANK_IMAGE = new string[] { "char_ace", "char_2", "char_3", "char_4", "char_5", "char_6", "char_7", "char_8", "char_9", "char_10", "char_j", "char_q", "char_k" };
    static string[] SUIT_IMAGE = new string[] { "bitch_icon", "spade_icon", "diamond_icon", "heart_icon" };
    static string[] ICON_IMAGE = new string[] { "jack_icon", "queen_icon", "king_icon" };

    public UISprite 
        spriteBackground, 
        spriteRank, 
        spriteSuit, 
        spriteIcon;

    PokerCard card;

    public void SetDataCard(PokerCard card)
    {
        this.card = card;
        UpdateUI();
    }

    public void UpdateUI()
    {
        spriteBackground.spriteName = card.cardId < 0 ? BACKGROUND[0] : BACKGROUND[1];
        int rank = (int)card.GetRank();
        int suit = (int)card.GetSuit();
        NGUITools.SetActive(spriteIcon.gameObject, card.cardId >= 0);
        NGUITools.SetActive(spriteRank.gameObject, rank > 0);
        NGUITools.SetActive(spriteSuit.gameObject, suit >= 0);

        if (rank > 0)
            spriteRank.spriteName = RANK_IMAGE[rank - 1];
        if (suit >= 0)
            spriteSuit.spriteName = SUIT_IMAGE[suit];

        if (rank > 1 && rank < 11)
            spriteIcon.spriteName = spriteSuit.spriteName;
        else if (rank >= 11)
            spriteIcon.spriteName = ICON_IMAGE[rank - 11];
    }
}
