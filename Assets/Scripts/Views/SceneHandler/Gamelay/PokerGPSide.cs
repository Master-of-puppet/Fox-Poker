using Puppet;
using Puppet.Poker;
using Puppet.Poker.Datagram;
using Puppet.Service;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PokerGPSide : MonoBehaviour
{
    public PokerSide CurrentSide;

    public GameObject[] positionCardMainPlayer;
	public GameObject[] positionCardFaceCards;
	public GameObject[] positionCardBackCards;
	public GameObject[] positionCardGameEnd;
	public GameObject positionMoney,positionDealer;
	public GameObject btnSit;

    void Awake()
    {
        wasBuyChip = false;
    }

    IEnumerator Start()
    {
        btnSit.SetActive(false);

        while (PokerObserver.Game.gameDetails == null)
            yield return new WaitForEndOfFrame();

        bool active = false;
        if(PokerObserver.Game.MAX_PLAYER_IN_GAME == 5)
        {
            if (CurrentSide == PokerSide.Slot_1)
                active = true;
            else if (CurrentSide == PokerSide.Slot_3)
            {
                CurrentSide = PokerSide.Slot_2;
                active = true;
            }
            else if (CurrentSide == PokerSide.Slot_5)
            {
                CurrentSide = PokerSide.Slot_3;
                active = true;
            }
            else if (CurrentSide == PokerSide.Slot_6)
            {
                CurrentSide = PokerSide.Slot_4;
                active = true;
            }
            else if (CurrentSide == PokerSide.Slot_8)
            {
                CurrentSide = PokerSide.Slot_5;
                active = true;
            }
            else if (CurrentSide == PokerSide.Slot_2)
                CurrentSide = PokerSide.Slot_6;
            else if (CurrentSide == PokerSide.Slot_4)
                CurrentSide = PokerSide.Slot_7;
            else if (CurrentSide == PokerSide.Slot_7)
                CurrentSide = PokerSide.Slot_8;
        }
        else
        {
            active = true;
        }

        SetActiveButton(active);
    }

    void SetActiveButton(bool active)
    {
        if (PokerObserver.Game.MAX_PLAYER_IN_GAME == 5)
        {
            if (CurrentSide == PokerSide.Slot_6)
                active = false;       
            else if (CurrentSide == PokerSide.Slot_7)
                active = false;
            else if (CurrentSide == PokerSide.Slot_8)
                active = false;
            else if (CurrentSide == PokerSide.Slot_9)
                active = false;
        }
        btnSit.SetActive(active);
    }

    void OnEnable()
    {
        onPlayerPickSide += PlayerPickSide;
        onPlayerSitdown += PlayerSitdown;
        UIEventListener.Get(btnSit).onClick += OnClickSit;

        PokerObserver.Instance.onPlayerListChanged += Instance_onPlayerListChanged;
    }

    void OnDisable()
    {
        onPlayerPickSide -= PlayerPickSide;
        onPlayerSitdown -= PlayerSitdown;
        UIEventListener.Get(btnSit).onClick -= OnClickSit;

        PokerObserver.Instance.onPlayerListChanged -= Instance_onPlayerListChanged;
    }

    void Instance_onPlayerListChanged(ResponsePlayerListChanged data)
    {
        if(PokerObserver.Instance.IsMainPlayer(data.player.userName))
        {
            bool showSit = false;
            switch(data.GetActionState())
            {
                case PokerPlayerChangeAction.playerRemoved:
                case PokerPlayerChangeAction.waitingPlayerAdded:
                    showSit = true;
                    wasBuyChip = false;
                    break;
            }
            SetActiveButton(showSit);
        }
    }

    static event Action<int> onPlayerPickSide;
    static event Action<bool> onPlayerSitdown;
    static bool wasBuyChip = false;
    void PlayerPickSide(int slot)
    {
        if (wasBuyChip == false)
        {
            IDialogData dialog;
            
            if (PokerObserver.Game.CanBePlay)
            {
                dialog = new DialogBuyChip(PokerObserver.Game.SmallBlind, (betting, autoBuy) =>
                {
                    if (betting >= PokerObserver.Game.SmallBlind)
                    {
                        PokerObserver.Instance.SitDown(slot, betting);
                        Puppet.API.Client.APIPokerGame.SetAutoBuy(autoBuy);
                        wasBuyChip = true;
                    }

                    if (onPlayerSitdown != null)
                        onPlayerSitdown(!wasBuyChip);
                });
            }
            else
            {
                dialog = new DialogMessage("Thông báo", "Số tiền của bạn không đủ.");
            }
            DialogService.Instance.ShowDialog(dialog);
        }
    }

    void PlayerSitdown(bool success)
    {
        SetActiveButton(success);
    }

    void OnClickSit(GameObject go)
    {
        if(onPlayerPickSide != null)
            onPlayerPickSide((int)CurrentSide);
    }
}
