using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class CardInfo
{
    public string cardName;
    public string cardImage;
    public int Cost;
    public int Item;        //무슨 아이템인지(0~4)
    //public string isLock;
    //public string Level;        //레벨
    //public string Star;     //별
    //public string Gauge;        //게이지
    public string Info;     //누니설명
    public string Effect;   //보유효과

    public string[] Building;     //보유시 영향을 주는 건물
    public string Gold;   //보유효과
    public string Weight;       //가중치

    public string Path;

    public string[] Dialog;
}
public class Card : MonoBehaviour
{
    public string cardName;
    public string cardImage;
    public int Cost;
    public int Item;        //무슨 아이템인지(0~4)
    public string isLock;
    //public string Level;        //레벨
    //public string Star;     //별
    //public string Gauge;        //게이지
    public string Info;     //누니설명
    public string Effect;   //보유효과
    public string Id;           //고유 Id

    public string[] Building;     //보유시 영향을 주는 건물
    public string Gold;   //보유효과
    public string Weight;       //가중치

    public bool isDialog = false;               //대사 말하고 있나

    public Button NuniBtn;
    public GameObject DialogObject;
    public Text DialogTxt;
    public GameObject _AddressableObj;
    public IObservable<int> CountDownObservable => _countDownObservable.AsObservable<int>();
    public IConnectableObservable<int> _countDownObservable;
    readonly Subject<int> TimerStreams = new Subject<int>();

    WonderAI WonderScript;
    BoundsInt StartPos = new BoundsInt();
    BoundsInt DestPos = new BoundsInt();
    Vector3 DestPosition;
    public Card(Cardsave cardSave)
    {
        cardImage = cardSave.cardImage;
        isLock = cardSave.isLock;
        Id = cardSave.Id;
    }
    public Card(Card c)
    {
        isLock = c.isLock;
        cardName = c.cardName;
        cardImage = c.cardImage;
        Cost = c.Cost;
        Item = c.Item;
        //Level = c.Level;
        //Star = c.Star;
        //Gauge = c.Gauge;
        Info = c.Info;
        Effect = c.Effect;
        Building = c.Building;
        Gold = c.Gold;
        Weight = c.Weight;

    }
    public void SetValue(Card c)
    {
        isLock = c.isLock;
        cardName = c.cardName;
        cardImage = c.cardImage;
        Cost = c.Cost;
        Item = c.Item;
        //Level = c.Level;
        //Star = c.Star;
        //Gauge = c.Gauge;
        Info = c.Info;
        Effect = c.Effect;
        Building = c.Building;
        Gold = c.Gold;
        Weight = c.Weight;
    }
    public Card(CardInfo c)
    {
        cardName = c.cardName;
        cardImage = c.cardImage;
        Cost = c.Cost;
        Item = c.Item;
        //Level = c.Level;
        //Star = c.Star;
        //Gauge = c.Gauge;
        Info = c.Info;
        Effect = c.Effect;
        Building = c.Building;
        Gold = c.Gold;
        Weight = c.Weight;

    }
   


    private void Start()
    {
        gameObject.transform.position = new Vector3(UnityEngine.Random.Range(-9.5f, 10f),
                                                    UnityEngine.Random.Range(-9.5f, 10.5f)); //처음 시작할땐 랜덤 위치



        WonderScript = gameObject.GetComponent<WonderAI>();
        
        StartPos.position = GridBuildingSystem.current.gridLayout.WorldToCell(gameObject.transform.position);
        StartPos.size = new Vector3Int(1,1,1);



        DestPos.position = GridBuildingSystem.current.gridLayout.WorldToCell(new Vector3(UnityEngine.Random.Range(-9.5f, 10f),
                                                    UnityEngine.Random.Range(-9.5f, 10.5f)));
        DestPos.size = new Vector3Int(1,1,1);
        
        WonderScript.SetPos(StartPos, DestPos);
        ;       //길찾기 알고리즘 시작
        StartCoroutine(Move(WonderScript.Astar()));

        NuniBtn.OnClickAsObservable().Subscribe(_ =>
        {
            if (!isDialog)
            {
                isDialog = true;
                DialogObject.SetActive(true); //대화창 생성
                DialogTxt.text = GameManager.Instance.NuniInfo[cardImage].Dialog[UnityEngine.Random.Range(0, GameManager.Instance.NuniInfo[cardImage].Dialog.Length - 1)];

                // _countDownObservable.Connect();
                StartCoroutine(Wait(3));


            }
        }).AddTo(this);
       
    }

    IEnumerator Wait(int time)
    {
        yield return new WaitForSecondsRealtime(3.0f);
        isDialog = false;
        DialogObject.SetActive(false); //대화창 생성
    }
    
    IEnumerator Move(List<Node> MoveNodes)
    {
        WaitForSecondsRealtime waitTime=   new WaitForSecondsRealtime(0.5f);

        while (MoveNodes.Count>0)
        {

            yield return waitTime;
            Vector2 target = GridBuildingSystem.current.gridLayout.CellToWorld(new Vector3Int( MoveNodes[0].X, MoveNodes[0].Y,1));
            Debug.Log("변경 전 이동하는 위치는 "+ new Vector3Int(MoveNodes[0].X, MoveNodes[0].Y, 1));
            Debug.Log("변경 후 이동하는 위치는 " + target);
            transform.position = target;
            MoveNodes.RemoveAt(0);
        }
        
    }

    private void OnDestroy()
    {
        Addressables.ReleaseInstance(_AddressableObj);
    }
}
