using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEngine.Networking;
using System;
using UniRx;
using UnityEngine.AddressableAssets;

public class LoadManager : MonoBehaviour
{
    public static bool isLoad = false;

    public GameObject buildings;
    public GameObject nunis;

   public bool isLoaded;      //�ǹ� �� �ҷ��Դ���

    public GameObject RewardPannel;     //�ϰ����� �ǳ�
    public BuildingSave buildingsave;

   public GameObject LoadingPanel;
    UiLoadingPanel UILoadingPanel;

    public  Dictionary<string, Building> MyBuildings = new Dictionary<string, Building>();          //내가 가지고 있는 빌딩들(id, Building)
    public  Dictionary<string, Card> MyNuni = new Dictionary<string, Card>();          //내가 가지고 있는 빌딩들(id, Building)

      public  Dictionary<string, GameObject> MyBuildingsPrefab = new Dictionary<string, GameObject>();          //내가 가지고 있는 빌딩들 오브젝트(id, Building)
      public  Dictionary<string, GameObject> MyNuniPrefab = new Dictionary<string, GameObject>();          //내가 가지고 있는 누니들 오브젝트(id, Building)
    
    public static Subject<Building> ReBuildingSubject = new Subject<Building>();
    public static Subject<Building> AddBuildingSubject = new Subject<Building>();
    public static Subject<Building> RemoveBuildingSubject = new Subject<Building>();

    public static GameObject Currnetbuildings;
    public static LoadManager _Instance;

    public Text MoneyText;
    public Text ShinMoneyText;

    public string FriendUid;

    public Button MyHomeBtn;

    public GameObject TutoManager;
    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {


            MoneyText.text = GameManager.Instance.PlayerUserInfo.Money;
            ShinMoneyText.text = GameManager.Instance.PlayerUserInfo.ShinMoney;
        }
    }
    public static LoadManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                return null;
            }
            return _Instance;
        }
    }
    public void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }

    }
    //public GameObject 
    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();

        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }

   
    
   
    //��ȭ�ε�
    //ĳ���� �ε�
    void Start()
    {
       

        isLoaded = false;
        GameManager.items = 0;          //������ �ʱ�ȭ

        ReBuildingSubject.Subscribe(building=>                  //건물 리스트 새로고침
        {
            MyBuildings[building.Id] = building.DeepCopy();
        }).AddTo(this); 

        RemoveBuildingSubject.Subscribe(building=>                  //건물 리스트 빼기
        {
            MyBuildings.Remove(building.Id); 
        }).AddTo(this);  

        AddBuildingSubject.Subscribe(building=>                  //건물 리스트 더하기
        {
            MyBuildings.Add(building.Id, building);
        }).AddTo(this);
        Debug.Log("튜토는 " + GameManager.Instance.PlayerUserInfo.Tuto);
        if (MyHomeBtn != null)
        {

            MyHomeBtn.OnClickAsObservable().Subscribe(_ => {

                LoadingSceneController.Instance.LoadScene(SceneName.Main);

            });
        }
        if (SceneManager.GetActiveScene().name.Equals("Main"))          //메인씬이면
        {



            /* Action action ;
             LoadManager.Instance.buildingsave.BuildingReq(BuildingDef.getMyBuilding,null,action=()=> {
                 StartCoroutine(RewardStart());
                 }
             );          //오늘 재화 받을 수 있는지}) ;*/

            if (GameManager.Instance.PlayerUserInfo.Tuto < 14)
            {
                if (TutoManager==null)
                
                    return;
                
                TutoManager.SetActive(true);
                if (GameManager.Instance.PlayerUserInfo.Tuto > 9)//게임갔다오고난 후
                {
                    RandomSelect.isTuto = 1;
                }
                else
                {
                    RandomSelect.isTuto = 0;//게임튜토 실행
                }
            }
            else//튜토 다 끝낸 상태
            {
                TutoManager.SetActive(false);
                RandomSelect.isTuto = 1;
            }

            if (GameManager.Instance.PlayerUserInfo.Tuto > 3)
            {
                Camera.main.GetComponent<Transform>().position = new Vector3(0, 0, -10);
            }
        }
  

    }
    public void NuniLoad()
    {
        foreach (var item in MyNuni)
        {
            if (item.Value.isLock.Equals("T"))
            {
                Debug.Log(item.Value.cardImage) ;
                Addressables.LoadAssetAsync<GameObject>(GameManager.Instance.NuniInfo[item.Value.cardImage].Path).Completed += (gameobject) =>
                {
                    //GameObject BuildingPrefab = gameobject.Result;
                   // Currnetbuildings = Instantiate(gameobject.Result, new Vector3(0, 0, 0), Quaternion.identity, buildings.transform) as GameObject;

                    //callback.Invoke();
              
                GameObject nuni = Instantiate(gameobject.Result, nunis.transform);

                nuni.name = item.Value.Id;
                Card nuni_card = nuni.GetComponent<Card>();
                nuni_card.SetValue(item.Value);


                MyNuniPrefab.Add(item.Value.Id, nuni);         //현재 가지고 있는 누니 오브젝트 딕셔너리에 추가
                };
            }
        }
        Debug.Log("누니 불러옴");
    }
    public void BuildingLoad()
    {
        foreach (var item in MyBuildings)
        {
            if (item.Value.isLock.Equals("F"))          //��ġ�ȵǾ��ִ�?
                continue;

            //string BuildingName = LoadBuilding.Building_Image;        //���� ������ �ִ� ���� ����Ʈ���� ���� �̸� �θ���
            try
            {
                InstantiateBuilding(item.Value,()=> {
                    Building g_Building = MyBuildingsPrefab[item.Value.Id].GetComponent<Building>();       //건물 Instatiate



                    g_Building.Type = BuildType.Load;
                    g_Building.Place_Initial(g_Building.Type);

                    MyBuildings[g_Building.Id].SetValue(g_Building);
                    MyBuildings[g_Building.Id].area = g_Building.area;
                    GameManager.IDs.Add(g_Building.Id);
                });

               
                // g_Building.Rotation();

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }
        Debug.Log("건물 로드 끝");
      //  UILoadingPanel.DestroyGameObject();
    }
    public void InstantiateBuilding(Building building,Action callback)
    {
        try
        {

            //  GameObject BuildingPrefab = GameManager.BuildingPrefabData[building.Building_Image];           // �ش� �ǹ� ������

            if (building.Type != BuildType.Make)
            {
                Addressables.LoadAssetAsync<GameObject>(GameManager.Instance.BuildingInfo[building.Building_Image].Path).Completed += (gameobject) =>
                {            //어드레서블로 부르기
                    //GameObject BuildingPrefab = gameobject.Result;


                    Currnetbuildings = Instantiate(gameobject.Result, new Vector3(building.BuildingPosition.x, building.BuildingPosition.y, 0), Quaternion.identity, buildings.transform) as GameObject;
                    MyBuildingsPrefab.Add(building.Id, Currnetbuildings);                   //내 건물 프리팹 딕셔너리에 추가
               


                Building g_Building = MyBuildingsPrefab[building.Id].GetComponent<Building>();
                if (g_Building.isStr)       //건축물이라면
                    building.isStr = true;

                g_Building.SetValue(building);
                    Debug.Log(g_Building.Id+ "의 남은 보상 시간은 " + g_Building.RemainTime);

                    Currnetbuildings.name = g_Building.Id;
                    MyBuildings[g_Building.Id] = g_Building;
                    if (callback != null)
                        callback.Invoke();
                    // return g_Building;
                };
            }
            else
            {
                Addressables.LoadAssetAsync<GameObject>(GameManager.Instance.BuildingInfo[building.Building_Image].Path).Completed += (gameobject) =>
                {
                    //GameObject BuildingPrefab = gameobject.Result;
                    Currnetbuildings = Instantiate(gameobject.Result, new Vector3(0, 0, 0), Quaternion.identity, buildings.transform) as GameObject;

                    if (callback != null)
                        callback.Invoke();
                    
                };
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
       // return null;

    }
    public void RemoveBuilding(string Id)
    {
        Destroy(MyBuildingsPrefab[Id]);
      
        MyBuildingsPrefab.Remove(Id);

    }
    public void RemoveNuni(string Id)
    {
        Destroy(MyNuniPrefab[Id]);

        MyNuniPrefab.Remove(Id);

    }
 
}