using UnityEngine;
using System.Collections;
using Puppet.Service;
using HoldemHand;
using Puppet;
[PrefabAttribute(Name = "Prefabs/Dialog/Gameplay/DialogGameplayRank", Depth = 20, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogGameplayRankView : BaseDialog<DialogGameplayRankModel, DialogGameplayRankView>
{
    public PokerGamePlayRank percentsTwoPair;
    public PokerGamePlayRank percents3ofaKind;
    public PokerGamePlayRank percentsStraight;
    public PokerGamePlayRank percentsFlush;
    public PokerGamePlayRank percentsFullhouse;
    public PokerGamePlayRank percents4ofaKind;
    public PokerGamePlayRank percentsStraightFlush;

    public override void ShowDialog(DialogGameplayRankModel data)
    {
        
        base.ShowDialog(data);
        initData();
    }
    void initData() {
        int count = 0;
        double[] player = new double[9];
        double[] opponent = new double[9];
        if (!Hand.ValidateHand(data.pocket + " " + data.boards))
        {
            Logger.Log("Validate False");
            return;
        }
        Hand.ParseHand(data.pocket + " " + data.boards, ref count);

        // Don't allow these configurations because of calculation time.
        if (count == 0 || count == 1 || count == 3 || count == 4 || count > 7)
        {
            Logger.Log("Parse False " + count);
            return;
        }
        Hand.HandPlayerOpponentOdds(data.pocket, data.boards, ref player, ref opponent);
        for (int i = 0; i < 9; i++)
        {
            switch ((Hand.HandTypes)i)
            {
                case Hand.HandTypes.HighCard:
                    break;
                case Hand.HandTypes.Pair:
                    break;
                case Hand.HandTypes.TwoPair:
                    percentsTwoPair.SetPercent( FormatPercent(player[i]),false);
                    break;
                case Hand.HandTypes.Trips:
                    percents3ofaKind.SetPercent(FormatPercent(player[i]), false);
                    break;
                case Hand.HandTypes.Straight:
                    percentsStraight.SetPercent(FormatPercent(player[i]), false);
                    break;
                case Hand.HandTypes.Flush:
                    percentsFlush.SetPercent(FormatPercent(player[i]), false);
                    break;
                case Hand.HandTypes.FullHouse:
                    percentsFullhouse.SetPercent(FormatPercent(player[i]), false);
                    break;
                case Hand.HandTypes.FourOfAKind:
                    percents4ofaKind.SetPercent(FormatPercent(player[i]), false);
                    break;
                case Hand.HandTypes.StraightFlush:
                    percentsStraightFlush.SetPercent(FormatPercent(player[i]), false);
                    break;
            }

        }
    }
    private string FormatPercent(double v)
    {
        if (v != 0.0)
        {
            if (v * 100.0 >= 1.0)
                return string.Format("{0:##0.0}%", v * 100.0);
            else
                return "<1%";
        }
        return "n/a";
    }
}
public class DialogGameplayRankModel : AbstractDialogData{
    public string pocket;
    public string boards;
    public DialogGameplayRankModel(string pocket, string boards)
    {
        this.pocket = pocket;
        this.boards = boards;
    }
    public override void ShowDialog()
    {
        DialogGameplayRankView.Instance.ShowDialog(this);
    }

    
}
