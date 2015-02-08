using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet.Poker.Datagram;

public class PokerPotManager : MonoBehaviour
{
    #region UnityEditor
    public UITable tablePot;
    public GameObject topLeftPosition,topRightPosition;
    #endregion
    List<PokerPotItem> currentPots = new List<PokerPotItem>();
    PokerGameplayPlaymat playmat;



    void Awake()
    {
        playmat = GameObject.FindObjectOfType<PokerGameplayPlaymat>();
    }


    public void UpdatePot(List<ResponseUpdatePot.DataPot> dataPots) 
    {
        StartCoroutine(_UpdatePot(dataPots));
    }
    IEnumerator _UpdatePot(List<ResponseUpdatePot.DataPot> dataPots)
    {
        foreach (ResponseUpdatePot.DataPot data in dataPots)
        {
            bool isCreate = false;
            PokerPotItem thisPot = currentPots.Find(p => data.id == p.Pot.id);
            if (thisPot != null)
            {
                thisPot.SetValue(data);
            }
            else
            {
                thisPot = PokerPotItem.Create(data);
                isCreate = true;
                switch (currentPots.Count)
                {
                    case 6:
                        thisPot.transform.parent = topLeftPosition.transform;
                        break;
                    case 7:
                        thisPot.transform.parent = topRightPosition.transform;
                        break;
                    default :
                        thisPot.transform.parent = tablePot.transform;
                        break;
                }
                thisPot.transform.localScale = Vector3.one;
                thisPot.transform.localPosition = Vector3.zero;              
                currentPots.Add(thisPot);
            }

            #region EFFECT MOVE CHIP
            if (data.isNew)
            {
                if(isCreate)
                    thisPot.SetAlpha(0);
                tablePot.Reposition();
                yield return new WaitForEndOfFrame();

                List<PokerPotItem> listPotItems = new List<PokerPotItem>();
                for (int i = 0; i < data.contributors.Length; i++)
                {
                    PokerPlayerUI uiPlayer = playmat.GetPlayerController(data.contributors[i]);
                    if (uiPlayer != null)
                    {
                        PokerPotItem pot = NGUITools.AddChild(uiPlayer.side.positionMoney, playmat.prefabBetObject).GetComponent<PokerPotItem>();
                        pot.gameObject.transform.parent = thisPot.transform;
                        pot.OnMove();
                        listPotItems.Add(pot);
                    }
                }
                foreach (PokerPotItem pot in listPotItems)
                    iTween.MoveTo(pot.gameObject, iTween.Hash("islocal", true, "time", .5f, "position", Vector3.zero));

                StartCoroutine(PlaySound());

                yield return new WaitForSeconds(.6f);
                for (int i = listPotItems.Count - 1; i >= 0; i--)
                {
                    if (listPotItems[i] != null && listPotItems[i].gameObject != null)
                        GameObject.Destroy(listPotItems[i].gameObject);
                }
                listPotItems.Clear();
                thisPot.SetAlpha(1);
                yield return new WaitForEndOfFrame();
            }
            #endregion
        }
        yield return new WaitForEndOfFrame();
        tablePot.Reposition();
    }

    IEnumerator PlaySound()
    {
        PuSound.Instance.Play(SoundType.RaiseCost);
        yield return new WaitForSeconds(0.2f);
        PuSound.Instance.Play(SoundType.RaiseCost);
        yield return new WaitForSeconds(0.2f);
        PuSound.Instance.Play(SoundType.RaiseCost);
    }

    public void DestroyAllPot() 
    {
        foreach (PokerPotItem item in currentPots)
            GameObject.Destroy(item.gameObject);
        currentPots.Clear();
    }
}
