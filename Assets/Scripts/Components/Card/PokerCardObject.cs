using UnityEngine;
using System.Collections;
using Puppet.Poker.Models;
using Puppet;

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
    public GameObject highlightObject;
	private int index=-1;

    public PokerCard card;
	void Start(){
		spriteBackground.depth =14;
	}

    public void SetDataCard(PokerCard card)
    {
        this.SetHighlight(false);
        this.card = card;
        UpdateUI();
    }

    public void SetDataCard(PokerCard card, int index)
    {
        SetDataCard(card);
		this.index = index;
    }

    public void SetIndexCard(int i)
    {
        spriteBackground.depth += i * 5;
        spriteRank.depth += i * 5;
        spriteSuit.depth += i * 5;
        spriteIcon.depth += i * 5;
    }

    public void UpdateUI()
    {
        //spriteBackground.spriteName = card.cardId < 0 ? BACKGROUND[0] : BACKGROUND[1];
		spriteBackground.spriteName = BACKGROUND[0];
        int rank = (int)card.GetRank();
        int suit = (int)card.GetSuit();

        NGUITools.SetActive(spriteIcon.gameObject, card.cardId >= 0);
        NGUITools.SetActive(spriteRank.gameObject, rank > 0);
        NGUITools.SetActive(spriteSuit.gameObject, suit >= 0);

        if (rank > 0)
            spriteRank.spriteName = RANK_IMAGE[rank - 1];

        if (suit >= 0)
            spriteSuit.spriteName = SUIT_IMAGE[suit];

        if (rank > 0 && rank < 11)
        {
            spriteIcon.spriteName = spriteSuit.spriteName;
            //spriteIcon.MakePixelPerfect();
            spriteIcon.width = 42;
            spriteIcon.height = 48;
			OnShowFaceCard(2);
        }
        else if (rank >= 11)
        {
            spriteIcon.spriteName = ICON_IMAGE[rank - 11];
            //spriteIcon.MakePixelPerfect();
            spriteIcon.width = 51;
            spriteIcon.height = 59;
			OnShowFaceCard(2);
        }

        if(card.cardId >= 0)
        {
            spriteSuit.color = spriteRank.color = card.IsRedCard() ? Color.red : Color.black;
            if (rank < 11)
                spriteIcon.color = spriteSuit.color;
        }

    }

    public void SetHighlight(bool state)
    {
        NGUITools.SetActive(highlightObject, state);
    }

	System.Collections.IEnumerator ChanceSprite(float time){
		iTween.RotateTo(spriteBackground.gameObject, iTween.Hash("islocal", true, "y", 0, "time", time));
		yield return new WaitForSeconds(time/6);
		spriteBackground.spriteName = BACKGROUND[1];
		spriteBackground.depth = 10;
		if(this.index>-1)
			SetIndexCard(this.index);

	}
	
	void OnShowFaceCard(float time){
		StartCoroutine(ChanceSprite(time));
	}

}
