using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UniRx;
using System;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
public class GameManager : Singleton<GameManager>
{
    public static string NewVersion = "";                   //최신버전
    public static string CurVersion = "1.4.6";                    //현재버전
    public static bool isUpdateDone = false;                    //업데이트를 완료했냐

    public Queue<string> Queue;

    public static bool isStart = false;
    public static GameManager _Instance;
    public static bool parse = false;


    [Header("건물")]
    //public Sprite[] DogamChaImageInspector;     //인스펙터에서 받아 온 건물 이미지


    public static Dictionary<string, GameObject> BuildingPrefabData;    //모든 빌딩 프리팹 딕셔너리


    public static bool isEdit = false;
    public static bool isInvenEdit = false;
    public static Button InvenButton;

    public static List<string> IDs;        //건물 아이디

    public Dictionary<string, BuildingParse> BuildingInfo = new Dictionary<string, BuildingParse>();        //건물 정보
    public Dictionary<string, BuildingParse> StrInfo = new Dictionary<string, BuildingParse>();        //설치물 정보
    //----------------------------------------------------이까지 건물----------------------------------------------------


    //----------------------------------------------------여기서부터 누니--------------------------------------------------
    [Header("누니")]

    public GameObject[] CharaterPrefabInspector;        // 인스펙터에서 받아 온 모든 누니들의 프리팹
    public static Dictionary<string, GameObject> CharacterPrefab;       //모든 캐릭터 누니 딕셔너리

    public static Card[] AllNuniArray;              //엑셀에서 받아 온 모든 누니 정보 배열
    public Dictionary<string, CardInfo> NuniInfo = new Dictionary<string, CardInfo>();

    public Dictionary<string, Card> CharacterList;      //현재가지고 있는 누니 리스트
                                                        //public static Card[] CharacterArray;               //현재 가지고 있는 캐릭터 배열


    public static bool[] Items = new bool[10];     //현재 가지고 잇는 아이템 유무
    public static int items = 0;
    public static bool isStore = false;

    public GameObject Dont;
    public static bool nuniDialogParse = false;

    public static List<NuniDialog> NuniDialog;          //누니 상호작용 대화 
    public Card CurrentNuni;
    //-----------------------------------여기서부터 재화---------------------------------
    public static int Money;            //재화
    public static int ShinMoney;
    public static int Zem;


    //---------------------------------------------------------------------------------------------
    //--------------------------------여기서부터 플레이어 정보-------------------------------------
    [Header("유저정보")]
    [SerializeField]
    public UserInfo PlayerUserInfo;         //플레이어 유저 정보

    public static string NickName;      //플레이어 닉네임

    public ReactiveProperty<Sprite> ProfileImage = new ReactiveProperty<Sprite>();       //플레이어 프로필 이미지



    public static bool isReward;        //일괄수확 가능한지

    //----------------------------------------------------------------------------------------------
    //------------------------------여기서부터 게임 정보----------------------------------------------
    [Header("게임정보")]
    public static int BestScore;                //불러온 최고점수
    public static bool isBScore;                    //스코어 업데이트

    public static bool isMoveLock = false;      //창 떴을 때 이동 못하게하는 변수
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
    // Start is called before the first frame update

    //--------------------------------------------------------------------퀘스트---------------------------------------------------



    public static bool gameMusicOn = true;
    public static bool mainMusicOn = true;
    public static bool gameSoundOn = true;
    public static bool mainSoundOn = true;

    //--------------------------------------------공지----------------------------------------
    [Header("공지")]
    public static Notice[] Notice;

    //--------------------------------------------업적---------------------------------------
    public Dictionary<string, AchieveInfo> AchieveInfos = new Dictionary<string, AchieveInfo>();      //업적 정보 딕셔너리
    public Dictionary<string, MyAchieveInfo> MyAchieveInfos = new Dictionary<string, MyAchieveInfo>();      //내 업적 정보 딕셔너리


   
    void Start()
    {
        //DontDestroyOnLoad(gameObject);  // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.


        //
        BuildingPrefabData = new Dictionary<string, GameObject>();      //전체 빌딩 프리팹 리스트 (가지고 있지 않은 것도 포함)

        CharacterPrefab = new Dictionary<string, GameObject>();

        CharacterList = new Dictionary<string, Card>();
        IDs = new List<string>();                   //퀘스트 
        NuniDialog = new List<NuniDialog>();



        for (int i = 0; i < CharaterPrefabInspector.Length; i++)
        {
            CharacterPrefab.Add(CharaterPrefabInspector[i].name, CharaterPrefabInspector[i]);
        }

        if (SceneManager.GetActiveScene().name.Equals("Main"))
        {
            //Dont.SetActive(true);
        }

        //게임 시작했을 때 엑셀에서 모든 누니 정보들 파싱해 배열에 넣기


        DicParsingManager DPManager = new DicParsingManager();
        AllNuniArray = DPManager.Parse_character(1);            //누니 정보 파싱


    }


    public string IDGenerator()
    {
        string alpha = "qwertyuipoasdfjkl123456789!@#$%^&*()";
        string id = "";

        bool isCount = false;
        do
        {

            for (int i = 0; i < 5; i++)
            {
                id += alpha[UnityEngine.Random.Range(0, 24)];
            }
            for (int i = 0; i < IDs.Count; i++)
            {
                if (IDs[i].Equals(id))
                {
                    isCount = false;
                }
                else
                {
                    isCount = true;
                    IDs.Add(id);
                }
            }
        } while (isCount.Equals(true));
        return id;
    }



    public void GameSave()
    {

        FirebaseLogin.Instance.SetUserInfo(GameManager.Instance.PlayerUserInfo);
    }
    public void UpdateMyAchieveInfo(string id, int count)
    {
        if (GameManager.Instance.MyAchieveInfos.ContainsKey(id))      //클리어한 업적 중에 해당 업적 Id가 있으면
        {
            GameManager.Instance.MyAchieveInfos[id].Count += count;
            if (GameManager.Instance.MyAchieveInfos[id].Count
                >= GameManager.Instance.AchieveInfos[id].Count[GameManager.Instance.MyAchieveInfos[id].Index])
            {
                GameManager.Instance.MyAchieveInfos[id].isReward[GameManager.Instance.MyAchieveInfos[id].Index] = "true";
            }
        }
        else
        {
            MyAchieveInfo NewMyAchieveInfo = new MyAchieveInfo(new string[] { "false", "false", "false", "false", "false" },
                                                                id, 0, count, GameManager.Instance.PlayerUserInfo.Uid);

            GameManager.Instance.MyAchieveInfos.Add(id, NewMyAchieveInfo);

            // GameManager.Instance.MyAchieveInfos[achieveId[0][i]].CountRP.Value += 5;
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)//비활성화
        {
            GameSave();
        }
    }
    public IObservable<int> CreateCountDownObservable(int countTime)
    {
        return Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)) // 0초 이후 1초 간격으로 실행
            .Select(x => (int)(countTime - x)) // x는 시작하고 나서의 시간(초)
            .TakeWhile(x => x > 0); // 0초 초과 동안 OnNext 0이 되면 OnComplete
    }
}
