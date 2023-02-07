using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AddressableAssets;

public class UIStorePanel : UIBase
{
    // Start is called before the first frame update

    //���� ��ư��
    [SerializeField]
    public List<StoreMenu> StoreMenu;
    [SerializeField]
    public List<GameObject> StoreViews;
    [SerializeField]
    public StoreNuniInfo StoreNuniInfo;

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
                            GameObject BuildingInfoObj = Instantiate(item.Prefab,item.Content.transform) as GameObject;
                            StoreButton BuildingInfo = BuildingInfoObj.GetComponent<StoreButton>();

                            BuildingInfo.SetStoreData(items.Value);
                            if (BuildingInfo.BuyBtn != null)
                            {
                                BuildingInfo.BuyBtn.OnClickAsObservable().Subscribe(_ => {                //���Ź�ư
                                    int money = int.Parse(GameManager.Instance.PlayerUserInfo.Money);
                                    int shinmoney = int.Parse(GameManager.Instance.PlayerUserInfo.ShinMoney);

                                    if (GameManager.Instance.BuildingInfo[items.Value.Building_Image].Cost[0] <= money //�ڿ�üũ(�������� ����, ������ �����ٴ� �гζ�)
                                    && GameManager.Instance.BuildingInfo[items.Value.Building_Image].ShinCost[0] <= shinmoney)
                                    {
                                        money -= GameManager.Instance.BuildingInfo[items.Value.Building_Image].Cost[0]; //����
                                        shinmoney -= GameManager.Instance.BuildingInfo[items.Value.Building_Image].ShinCost[0];


                                        Addressables.LoadAssetAsync<GameObject>(items.Value.Path).Completed += (gameobject) =>
                                        {            //��巹����� �̹��� �ҷ��� �ֱ�
                                            Building Newbuilding = gameobject.Result.GetComponent<Building>();

                                            Newbuilding.Placed = false;
                                            Newbuilding.Type = BuildType.Make;
                                            //Newbuilding.area = gameobject.Result.GetComponent<Building>().area;


                                            LoadManager.Instance.InstantiateBuilding(Newbuilding, () => {


                                                GridBuildingSystem.OnEditMode.OnNext(Newbuilding);

                                                ClosePanel();
                                            });
                                        };  //��巹������ �ǹ� ������ �ҷ��� Instantiate��
                                    }
                                    else
                                    {
                                        //������ �г� �߱�
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
                                BuildingInfo.BuyBtn.OnClickAsObservable().Subscribe(_ => {                //���Ź�ư
                                    int money = int.Parse(GameManager.Instance.PlayerUserInfo.Money);
                                    int shinmoney = int.Parse(GameManager.Instance.PlayerUserInfo.ShinMoney);

                                    if (GameManager.Instance.BuildingInfo[items.Value.Building_Image].Cost[0] <= money //�ڿ�üũ(�������� ����, ������ �����ٴ� �гζ�)
                                    && GameManager.Instance.BuildingInfo[items.Value.Building_Image].ShinCost[0] <= shinmoney)
                                    {
                                        money -= GameManager.Instance.StrInfo[items.Value.Building_Image].Cost[0]; //����
                                        shinmoney -= GameManager.Instance.StrInfo[items.Value.Building_Image].ShinCost[0];


                                        Addressables.LoadAssetAsync<GameObject>(items.Value.Path).Completed += (gameobject) =>
                                        {            //��巹����� �̹��� �ҷ��� �ֱ�
                                            Building Newbuilding = gameobject.Result.GetComponent<Building>();

                                            Newbuilding.Placed = false;
                                            Newbuilding.Type = BuildType.Make;
                                            //Newbuilding.area = gameobject.Result.GetComponent<Building>().area;


                                            LoadManager.Instance.InstantiateBuilding(Newbuilding, () => {


                                                GridBuildingSystem.OnEditMode.OnNext(Newbuilding);

                                                ClosePanel();
                                            });
                                        };  //��巹������ �ǹ� ������ �ҷ��� Instantiate��
                                    }
                                    else
                                    {
                                        //������ �г� �߱�
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
                            Debug.LogError("����: "+e.Message);
                            throw;
                        }
                        break;

                    default:

                        break;
                }
            });
        } //���� ��ư�� ����
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