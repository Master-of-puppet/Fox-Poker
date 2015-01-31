using UnityEngine;
using System.Collections;

public class PokerGameplayPlayerChat : MonoBehaviour {
    public UILabel txtContent;

    public static PokerGameplayPlayerChat Create(string chatContent, PokerPlayerUI player) {
        if (chatContent.Length > 57)
            chatContent = chatContent.Substring(0, 50) + "...";
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerChat"));
        obj.name = "Chat Content";
        obj.transform.parent = player.transform;
		obj.transform.localPosition = new Vector3(-70f,50f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<PokerGameplayPlayerChat>().txtContent.text = chatContent;
        obj.GetComponent<PokerGameplayPlayerChat>().ReDraw();
        return obj.GetComponent<PokerGameplayPlayerChat>();
    }

    private void ReDraw()
    {
        Invoke("DestroyMe", 5f);
    }
    private void DestroyMe() {
        if (gameObject != null)
            GameObject.Destroy(gameObject);
    }
	
}
