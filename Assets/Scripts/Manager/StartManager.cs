using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 using UnityEngine.SceneManagement;
using UniRx;
public class StartManager : MonoBehaviour       //���� ���� ������ �ִ� ��ũ��Ʈ
{
    public GameObject CharacterPrefab;
    public static int ItemIndex;
    public static int ChaIndex;
    public GameObject Scroll;       //��ũ�ѿ� content �ֱ�

    public static bool isParsing = false;

    GameObject DogamCha;
    public static Sprite ChaImage_;

    //public Text ItemInfoText;
    //public TextMesh ItemInfoText;
    //int[] itemList = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public  Dictionary<int, bool> itemList = new Dictionary<int, bool> {
                                                    {0,false},
                                                    { 1,false},
                                                    { 2,false},
                                                    { 3,false},
                                                    { 4,false},
                                                    { 5,false},
                                                    { 6,false},
                                                    { 7,false},
                                                    { 8,false},
                                                    { 9,false}
                                                        };
    public Sprite[] ItemImages;         //������ �̹�����
    public Sprite LockImage;

    string[] ItemInfos = { "��ġ�� ���� �� ���ϴ� ������ �ϳ� �����Ѵ�. ",
                          "������ ������ �ٸ� ���� ŵ �ϰ� �ٽ� ����� �� �ְ� �Ѵ�.",
                          "��ġ�ϱ� �� ������ ������ ������ �� �ִ�.",
                          "������ ���� ������ �̸� �� �� �ִ�.",
                          "������ ������ �������� ��ü �� �� �ִ�.",
                          "������ ���� ���� ���� ��ġ�ؾ� �ϴ� ������ ��ü�� �� �ִ�.",
                          "��ġ�� ������ ������ �ٲ� �� �ִ�.",//��ġ�� ���� ������ �ٲ� �� �ִ�.
                          "��� ������ ������ ��ü�� �� �ִ� ���� �ϳ� �����Ѵ�.",//��� ������ ���� ��ü�� �� �ִ� �� �ϳ� �����Ѵ�.
                          "��ġ�� ������ ������ ���� ���Ʒ��� ������ �����Ѵ�.",//��ġ�� Ÿ���� ������ ���� ���Ʒ��� Ÿ�������Ѵ�.
                          "��ġ�� ���� �� ������ ���� �翷���� ������ �����Ѵ�."};//��ġ�� Ÿ�� �� ������ ���� �翷���� Ÿ�� �����Ѵ�.
    public Sprite[] ItenImage;
    public TextMeshProUGUI ItemInfo;
    /* ������ ���
    * 0: ���찳               (Ȳ��)
    * 1: ŵ                   (��)
    * 2: ��������             (û�Һ�)
    * 3: �̸�����             (Ž��)
    * 4: ���ΰ�ħ             (������)
    * 5: <=>                  (������)
    * 6: ����3��              (����)
    * 7: ����3��              (����)
    * 8: ��� ��ü�Ҽ� �ִ� ��(������)
    * 9: ���� ������ �ٲ۴�   (������)
    */
    public static Button[] LockButton;

    public static GameObject Canvas;

    //[SerializeField]
    public List<ItemInfo> ItemDatas;
    public int ItemCount;
    public Text Info;
    void Awake()
    {
        if (isParsing .Equals( false))
        {
           // DicParsingManager DPManager = new DicParsingManager();
           // NuNiInformation = DPManager.Parse_character(1);    //���� ���� �Ľ�
            isParsing = true;
        }
        Canvas = GameObject.Find("Canvas");
        if (SceneManager.GetActiveScene().name.Equals("Game"))
        {
            StartManager.ChaIndex = 99;
          
        }
    }
    private void Start()
    {
        CharacterOpen();
    }

    public void CharacterOpen()
    {
       

         //Card[] NuniArray = GameManager.Instance.CharacterList.ToArray();
            foreach (var item in GameManager.Instance.CharacterList)
            {
            int ItemNumber = GameManager.Instance.NuniInfo[item.Value.cardImage].Item;
                if (ItemNumber != 0)
                {
                ItemDatas[ItemNumber].Image.sprite = ItenImage[ItemNumber];

                ItemDatas[ItemNumber].ItemBuntton.OnClickAsObservable().Subscribe(_=> {


                    Info.text = GameManager.Instance.NuniInfo[item.Value.cardImage].Info;

                    if (!ItemDatas[ItemNumber].isSelected)     //������ �ȵǾ��ִٸ�?
                    {
                        if (ItemCount < 4)
                        {
                            ItemCount++;
                            ItemDatas[ItemNumber].isSelected = true;
                            ItemDatas[ItemNumber].CheckImage.SetActive(true);
                            itemList[ItemNumber] = true;
                        }
                    }
                    else                    //���õǾ��ٸ�
                    {
                        ItemCount--;
                        if (ItemCount < 0)
                            ItemCount = 0;

                        ItemDatas[ItemNumber].isSelected = false;
                        ItemDatas[ItemNumber].CheckImage.SetActive(false);
                        itemList[ItemNumber] = false;
                    }
                   
                }).AddTo(this);

            }
            }
           



        
       /* for (int j = 0; j < itemList.Count; j++)
        {
            DogamCha = Instantiate(CharacterPrefab, Scroll.transform) ;
            //DogamCha.transform.SetParent(Scroll.transform);

            Transform[] ChaPrefabChilds = DogamCha.GetComponentsInChildren<Transform>();

            //���� ĳ���� ��ư 
            DogamCha.GetComponent<RectTransform>().name = j.ToString();
            DogamCha.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1);

            Button DogamChaButton = DogamCha.GetComponent<Button>();
            Image[] image = DogamChaButton.GetComponentsInChildren<Image>();

          
            if (itemList[j] .Equals( true))
            {
                image[3].sprite = ItenImage[j];//NuNiInformation[j].GetChaImange();   //ĳ���� �̸� �� �޾ƿͼ� �̹��� ã��
                DogamChaButton.OnClickAsObservable().Subscribe(_ => {

                    ItemInfo.text = ItemInfos[j];

                });
            }
            else
            {
                image[3].sprite = LockImage;//NuNiInformation[j].GetChaImange();   //ĳ���� �̸� �� �޾ƿͼ� �̹��� ã��

                DogamCha.tag = "Lock";

            }
            int itemIndex = int.Parse(DogamCha.GetComponent<RectTransform>().name);
           
            if (GameManager.Items[itemIndex] ==true)                               //���õǾ��ִ��� Ȯ���ϱ�
                image[1].gameObject.SetActive(true);
            else
                image[1].gameObject.SetActive(false);

          
        }*/
    }





    public void RefreshButtonArray(Character[] CharactersArray)
    {
        List<Button> LockButtonList = new List<Button>();
        LockButton = new Button[CharactersArray.Length];        //
        for (int i = 0; i < CharactersArray.Length; i++)
        {
            if (CharactersArray[i].GetCharacter("isLock") .Equals( "F"))
            {

            }
        }
    }
}
