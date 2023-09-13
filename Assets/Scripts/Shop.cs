using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform shopUi; //게임씬으로 위치를 이동시킬거임
    public Animator anim; //루나의 애니를 설정함
    Player enterP; //코인정보받아옴
    public GameObject[] itemArr;
    public int[] itemPriceArr;
    public Transform[] itemPosArr;
    public Text talkText;
    public string[] talkArr;

    public void enter(Player player)
    {
        enterP = player;
        shopUi.anchoredPosition = Vector3.zero;//ui게임 씬 안으로 이동시키기
    }

    public void exit()
    {
        anim.SetTrigger("doHello");
        shopUi.anchoredPosition = Vector3.down * 1200;
        Debug.Log("test");
    }

    public void Buy(int index)
    {
        int price = itemPriceArr[index];
        if(enterP.coin < price)
        {
            StopCoroutine(talkIE());
            StartCoroutine(talkIE());
            return;
        }
        enterP.coin -= price;
        Instantiate(itemArr[index], itemPosArr[index].position, itemPosArr[index].rotation);
    }
    IEnumerator talkIE()
    {
        talkText.text = talkArr[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkArr[0];
    }
}
