using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 using UnityEngine.SceneManagement;
using UniRx;
public class StartManager : MonoBehaviour       //엑셀 게임 데이터 넣는 스크립트
{
    public GameObject CharacterPrefab;
    public static int ItemIndex;
    public static int ChaIndex;
    public GameObject Scroll;       //스크롤에 content 넣기

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
    public Sprite[] ItemImages;         //아이템 이미지들
    public Sprite LockImage;

    string[] ItemInfos = { "배치된 얼음 중 원하는 얼음을 하나 삭제한다. ",
                          "등장한 얼음을 다른 곳에 킵 하고 다시 사용할 수 있게 한다.",
                          "배치하기 전 등장한 얼음을 삭제할 수 있다.",
                          "다음에 나올 얼음을 미리 볼 수 있다.",
                          "등장한 얼음을 랜덤으로 교체 할 수 있다.",
                          "다음에 나올 얼음 현재 배치해야 하는 얼음을 교체할 수 있다.",
                          "배치된 얼음의 색깔을 바꿀 수 있다.",//배치된 말의 색깔을 바꿀 수 있다.
                          "모든 종류의 얼음을 대체할 수 있는 얼음 하나 생성한다.",//모든 종류의 말을 대체할 수 있는 말 하나 생성한다.
                          "배치된 얼음중 아이템 기준 위아래로 얼음을 제거한다.",//배치된 타일중 아이템 기준 위아래로 타일제거한다.
                          "배치된 얼음 중 아이템 기준 양옆으로 얼음을 제거한다."};//배치된 타일 중 아이템 기준 양옆으로 타일 제거한다.
    public Sprite[] ItenImage;
    public TextMeshProUGUI ItemInfo;
    /* 아이템 목록
    * 0: 지우개               (황제)
    * 1: 킵                   (비서)
    * 2: 쓰레기통             (청소부)
    * 3: 미리보기             (탐정)
    * 4: 새로고침             (개발자)
    * 5: <=>                  (과학자)
    * 6: 가로3개              (팡팡)
    * 7: 세로3개              (펑펑)
    * 8: 모든 대체할수 있는 말(유니콘)
    * 9: 말의 색깔을 바꾼다   (마법사)
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
           // NuNiInformation = DPManager.Parse_character(1);    //도감 정보 파싱
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

                    if (!ItemDatas[ItemNumber].isSelected)     //선택이 안되어있다면?
                    {
                        if (ItemCount < 4)
                        {
                            ItemCount++;
                            ItemDatas[ItemNumber].isSelected = true;
                            ItemDatas[ItemNumber].CheckImage.SetActive(true);
                            itemList[ItemNumber] = true;
                        }
                    }
                    else                    //선택되었다면
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
           

    }
}
