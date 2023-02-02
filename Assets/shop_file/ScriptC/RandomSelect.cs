using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

public class RandomSelect : MonoBehaviour
{
    public List<Card> deck;  // 카드 덱
    public int total = 0;  // 카드들의 가중치 총 합

    public static int isTuto;
    public List<Card> result = new List<Card>();  // 랜덤하게 선택된 카드를 담을 리스트

    public Transform parent;
    public GameObject BuyResultObject;
    public Text ResultText;
    public Image ResultImage;

    void Start()
    {
        deck = new List<Card>();
        foreach (var item in GameManager.Instance.NuniInfo)
        {
            total += int.Parse(item.Value.Weight);
            deck.Add(new Card(item.Value));
        }

       /* for (int i = 0; i < GameManager.AllNuniArray.Length; i++)
        {
            // deck[i] = GameManager.AllNuniArray[i];
            Card c = new Card(GameManager.AllNuniArray[i]);
            c.SetValue(GameManager.AllNuniArray[i]);
            deck.Add(c);
        }*/
        ShopBuyScript.isfirst = true;
    }

    public void ResultSelect()
    {

        //셀력결과
        // 가중치 랜덤을 돌리면서 결과 리스트에 넣어줍니다.
        Card resultNuni = RandomCard();

        Addressables.LoadAssetAsync<Sprite>(resultNuni.cardImage).Completed += (image) =>
        {            //어드레서블로 이미지 불러서 넣기

            result.Add(resultNuni);
            // 비어 있는 카드를 생성하고
            //CardUI cardUI = Instantiate(cardprefab, parent).GetComponent<CardUI>();
            // 생성 된 카드에 결과 리스트의 정보를 넣어줍니다.
            //Card Nuni = cardUI.CardUISet(RandomCard());

            ResultText.text = resultNuni.cardName;
            ResultImage.sprite = image.Result;

            BuyResultObject.SetActive(true);

            resultNuni.isLock = "T";          //누니 잠금 품
            resultNuni.Id = GameManager.Instance.IDGenerator();
            GameManager.Instance.CharacterList.Add(resultNuni.Id, resultNuni);     //나온 결과를 리스트에 반영
                                                                                   //전체 누니 배열을 수정


            Cardsave cardsave = new Cardsave(GameManager.Instance.PlayerUserInfo.Uid, resultNuni.cardImage, resultNuni.isLock, resultNuni.Id);

            cardsave.Uid = GameManager.Instance.PlayerUserInfo.Uid;

            FirebaseLogin.Instance.SetNuni(cardsave);//파이어베이스에 업데이트

            ShopBuyScript.isfirst = false;
        };
    }
   /* IEnumerator NuniSave(Card nuni)                //누니 구글 스크립트에 저장
    {
        
        WWWForm form1 = new WWWForm();
        form1.AddField("order", "nuniSave");
        form1.AddField("player_nickname", GameManager.NickName);
        form1.AddField("nuni", nuni.cardName+":T") ;



        yield return StartCoroutine(Post(form1));                        //구글 스크립트로 초기화했는지 물어볼때까지 대기


    }
    IEnumerator Post(WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(GameManager.URL, form)) // 반드시 using을 써야한다
        {
            yield return www.SendWebRequest();
            //Debug.Log(www.downloadHandler.text);
            if (www.isDone) NuniResponse(www.downloadHandler.text);
            //else print("웹의 응답이 없습니다.");
        }

    }*/
    void NuniResponse(string json)                          //누니 불러오기
    {
        //List<QuestInfo> Questlist = new List<QuestInfo>();

        if (json .Equals( "null"))
        {
            return;
        }
        if (string.IsNullOrEmpty(json))
        {
            
            return;
        }
                       //누니 이름 받아서 겜메 모든 누니 배열에서 누니 정보 받아서 넣기

      
    }

    public Card RandomCard()
    {
        // 이렇게하면 가중치 랜덤함수 (확률이 다름)

        if (isTuto .Equals( 0))
        {
            return new Card(GameManager.Instance.NuniInfo["snow"]);
        }
        else
        {
            int weight = 0;
            int selectNum = 0;
            selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));

            foreach (var item in GameManager.Instance.NuniInfo)
            {
                weight += int.Parse(item.Value.Weight);

                if (selectNum <= weight)
                {
                    Card temp = new Card(item.Value);
                    return temp;
                }
            }
           /* for (int i = 0; i < GameManager.AllNuniArray.Length; i++)
            {
                weight += int.Parse(GameManager.AllNuniArray[i].Weight);
                if (selectNum <= weight)
                {
                    Card temp = new Card(GameManager.AllNuniArray[i]);
                    return temp;
                }

            }*/
            return null;
        }
    }

}

