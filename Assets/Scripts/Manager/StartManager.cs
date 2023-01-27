using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 using UnityEngine.SceneManagement;
public class StartManager : MonoBehaviour       //���� ���� ������ �ִ� ��ũ��Ʈ
{
    public GameObject CharacterPrefab;
    public static Card[] NuNiInformation;
    public static int ItemIndex;
    public static int ChaIndex;
    public GameObject Scroll;       //��ũ�ѿ� content �ֱ�

    public static bool isParsing = false;

    GameObject DogamCha;
    public static Sprite ChaImage_;

    //public Text ItemInfoText;
    //public TextMesh ItemInfoText;
    //int[] itemList = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public static Dictionary<int, bool> itemList = new Dictionary<int, bool> {
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

    void Awake()
    {
        if (isParsing .Equals( false))
        {
            DicParsingManager DPManager = new DicParsingManager();
            NuNiInformation = DPManager.Parse_character(1);    //���� ���� �Ľ�
            isParsing = true;
        }
        Canvas = GameObject.Find("Canvas");
        if (SceneManager.GetActiveScene().name.Equals("Game"))
        {
            StartManager.ChaIndex = 99;
            CharacterOpen();
        }
        else if(SceneManager.GetActiveScene().name.Equals("Main"))      //���� ������ ������ ���� �ʱ�ȭ
        {
            for (int i = 0; i < GameManager.Items.Length; i++)
            {
                GameManager.Items[i] = false;
            }
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Game"))
        {
            if (ChaIndex.Equals(99))
            {
                ItemInfo.text = "";
            }
            else
            {
                for (int i = 0; i < GameManager.AllNuniArray.Length; i++)
                {
                    if (GameManager.AllNuniArray[i].Item == ChaIndex)
                    {
                        ItemInfo.text = GameManager.AllNuniArray[i].Effect;
                    }
                }

            }
        }

    }

    public void CharacterOpen()
    {
        Transform[] child = Scroll.GetComponentsInChildren<Transform>();
        for (int i = 1; i < child.Length; i++)
        {
            Destroy(child[i].gameObject);
        }



        for (int j = 0; j < itemList.Count; j++)         //�����ϱ� �� ĳ���� ��Ÿ����
        {
            //Card[] NuniArray = GameManager.Instance.CharacterList.ToArray();
            foreach (var item in GameManager.Instance.CharacterList)
            {
                if (item.Value.Item.Equals(j))
                {
                    itemList[j] = true;
                }
            }
           



        }
        for (int j = 0; j < itemList.Count; j++)
        {
            DogamCha = Instantiate(CharacterPrefab);
            DogamCha.transform.SetParent(Scroll.transform);

            Transform[] ChaPrefabChilds = DogamCha.GetComponentsInChildren<Transform>();

            //���� ĳ���� ��ư 
            DogamCha.GetComponent<RectTransform>().name = j.ToString();
            DogamCha.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1);

            Button DogamChaButton = DogamCha.GetComponent<Button>();
            Image[] image = DogamChaButton.GetComponentsInChildren<Image>();

            if (itemList[j] .Equals( true))
            {
                image[3].sprite = ItenImage[j];//NuNiInformation[j].GetChaImange();   //ĳ���� �̸� �� �޾ƿͼ� �̹��� ã��

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
                

        }
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
