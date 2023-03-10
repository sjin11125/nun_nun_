using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AddressableAssets;

public class UIStorePanel : UIBase
{
    // Start is called before the first frame update

    //상점 버튼들
    [SerializeField]
    public List<StoreMenu> StoreMenu;
    [SerializeField]
    public List<GameObject> StoreViews;
    [SerializeField]
    public StoreNuniInfo StoreNuniInfo;

    public GameObject UIMoneyPanel;

    public UIStorePanel(GameObject UIPrefab)
    {
        UIStorePanel r = UIPrefab.GetComponent<UIStorePanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        r.InstantiatePrefab();
    }

   override public void Start()
    {
        base.Start();

        foreach (var item in StoreMenu)
        {
            item.Btn.OnClickAsObservable().Subscribe(_=> {
                
               
                switch (item.Type)
                {
                    case StoreMenuType.Building:
                        StoreViews[0].SetActive(true);
                        StoreViews[1].SetActive(false);
                        StoreViews[2].SetActive(false);

                        Exit(item.Content.transform);

                        foreach (var items in GameManager.Instance.BuildingInfo)
                        {
                            GameObject BuildingInfoObj = Instantiate(item.Prefab,item.Content.transform) as GameObject;     //상점 버튼 프리팹 Instantiate
                            StoreButton BuildingInfo = BuildingInfoObj.GetComponent<StoreButton>();     

                            BuildingInfo.SetStoreData(items.Value);                                 //건물 데이터 세팅
                            if (BuildingInfo.BuyBtn != null)
                            {
                                BuildingInfo.BuyBtn.OnClickAsObservable().Subscribe(_ => {                //구매버튼 구독
                             
                                    if (GameManager.Instance.BuildingInfo[items.Value.Building_Image].Cost[0] <= GameManager.Instance.Money.Value //자원체크(돈있으면 결제, 없으면 돈없다는 패널뜸)
                                    && GameManager.Instance.BuildingInfo[items.Value.Building_Image].ShinCost[0] <= GameManager.Instance.ShinMoney.Value)
                                    {
                                        GameManager.Instance.Money.Value -= GameManager.Instance.BuildingInfo[items.Value.Building_Image].Cost[0]; //결제
                                        GameManager.Instance.ShinMoney.Value -= GameManager.Instance.BuildingInfo[items.Value.Building_Image].ShinCost[0];


                                    /*    Addressables.LoadAssetAsync<GameObject>(items.Value.Path).Completed += (gameobject) =>
                                        {            
                                            Building Newbuilding = gameobject.Result.GetComponent<Building>();

                                            Newbuilding.Placed = false;
                                            Newbuilding.Type = BuildType.Make;              //BuildType을 Make로
                                            Newbuilding._AddressableObj = gameobject.Result;
                                            //Newbuilding.area = gameobject.Result.GetComponent<Building>().area;


                                        }; */ //어드레서블에서 건물 프리팹 불러와 Instantiate함
                                        Building Newbuilding = new Building();
                                        Newbuilding.SetValueParse(GameManager.Instance.BuildingInfo[items.Value.Building_Image]);
                                        Newbuilding.Type = BuildType.Make;
                                        LoadManager.Instance.InstantiateBuilding(Newbuilding, ref LoadManager.Instance.Currnetbuildings, () => {            //건물 프리팹 Instantiate 하고 콜백으로 건축모드 ON

                                            Newbuilding = LoadManager.Instance.Currnetbuildings.GetComponent<Building>();
                                            GridBuildingSystem.OnEditMode.OnNext(Newbuilding);                  //건축모드 ON

                                            ClosePanel();                                                   //상점 창 닫기
                                        });
                                    }
                                    else
                                    {
                                        UIYesNoPanel MoneyPanel = new UIYesNoPanel(UIMoneyPanel);  //돈부족 패널 뜨기
                                    }
                                }).AddTo(this);
                            }
                        }
                        break;

                    case StoreMenuType.Str:
                        StoreViews[0].SetActive(false);
                        StoreViews[1].SetActive(true);
                        StoreViews[2].SetActive(false);
                        Exit(item.Content.transform);

                        foreach (var items in GameManager.Instance.StrInfo)
                        {
                            GameObject BuildingInfoObj = Instantiate(item.Prefab, item.Content.transform) as GameObject;
                            StoreButton BuildingInfo = BuildingInfoObj.GetComponent<StoreButton>();

                            BuildingInfo.SetStoreData(items.Value);

                            if (BuildingInfo.BuyBtn != null)
                            {
                                BuildingInfo.BuyBtn.OnClickAsObservable().Subscribe(_ => {                //구매버튼
                               
                                    if (GameManager.Instance.BuildingInfo[items.Value.Building_Image].Cost[0] <= GameManager.Instance.Money.Value //자원체크(돈있으면 결제, 없으면 돈없다는 패널뜸)
                                    && GameManager.Instance.BuildingInfo[items.Value.Building_Image].ShinCost[0] <= GameManager.Instance.ShinMoney.Value)
                                    {
                                        GameManager.Instance.Money.Value -= GameManager.Instance.StrInfo[items.Value.Building_Image].Cost[0]; //결제
                                        GameManager.Instance.ShinMoney.Value -= GameManager.Instance.StrInfo[items.Value.Building_Image].ShinCost[0];


                                        Addressables.LoadAssetAsync<GameObject>(items.Value.Path).Completed += (gameobject) =>
                                        {            //어드레서블로 이미지 불러서 넣기
                                            Building Newbuilding = gameobject.Result.GetComponent<Building>();

                                            Newbuilding.Placed = false;
                                            Newbuilding.Type = BuildType.Make;
                                            Newbuilding._AddressableObj = gameobject.Result;
                                            //Newbuilding.area = gameobject.Result.GetComponent<Building>().area;


                                            LoadManager.Instance.InstantiateBuilding(Newbuilding,ref LoadManager.Instance.Currnetbuildings, () => {


                                                GridBuildingSystem.OnEditMode.OnNext(Newbuilding);

                                                ClosePanel();
                                            });
                                        };  //어드레서블에서 건물 프리팹 불러와 Instantiate함
                                    }
                                    else
                                    {
                                        UIYesNoPanel MoneyPanel = new UIYesNoPanel(UIMoneyPanel);  //돈부족 패널 뜨기 //돈부족 패널 뜨기
                                    }
                                }).AddTo(this);
                            }
                        }

                        break;

                    case StoreMenuType.Nuni:
                        StoreViews[0].SetActive(false);
                        StoreViews[1].SetActive(false);
                        StoreViews[2].SetActive(true);
                        StoreNuniInfo.InfoPanel.SetActive(false);
                        Exit(item.Content.transform);
                        try
                        {

                            foreach (var items in GameManager.Instance.NuniInfo)
                            {
                                GameObject NuniInfoObj = Instantiate(item.Prefab, item.Content.transform) as GameObject;
                                StoreButton NuniInfo = NuniInfoObj.GetComponent<StoreButton>();


                                NuniInfo.SetNuniData(items.Value);
                                NuniInfo.BuyBtn.OnClickAsObservable().Subscribe(_=> {
                                    if (StoreNuniInfo != null)
                                    {
                                        StoreNuniInfo.InfoPanel.SetActive(true);

                                        StoreNuniInfo.NuniInfo.text = items.Value.Info;
                                        StoreNuniInfo.NuniName.text = items.Value.cardName;
                                        StoreNuniInfo.NuniEffect.text = items.Value.Effect;

                                        StoreNuniInfo.NuniImage.sprite = NuniInfo.Image.sprite;
                                    }
                                }).AddTo(this);
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("에러: "+e.Message);
                            throw;
                        }
                        break;

                    default:

                        break;
                }
            });
        } //상점 버튼들 구독
    }

    public void Exit(Transform View)
    {
        Transform[] Content_Child = View.GetComponentsInChildren<Transform>();
        for (int i = 1; i < Content_Child.Length; i++)
        {
            Destroy(Content_Child[i].gameObject);
        }

    }


}