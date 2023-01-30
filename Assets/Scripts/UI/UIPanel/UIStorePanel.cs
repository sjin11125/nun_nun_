using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIStorePanel : UIBase
{
    // Start is called before the first frame update

    //상점 버튼들
    [SerializeField]
    public List<StoreMenu> StoreMenu;
    [SerializeField]
    public List<GameObject> StoreViews;

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
                        }

                        break;

                    case StoreMenuType.Nuni:
                        StoreViews[0].SetActive(false);
                        StoreViews[1].SetActive(false);
                        StoreViews[2].SetActive(true);
                        Exit(item.Content.transform);
                        try
                        {

                            foreach (var items in GameManager.Instance.NuniInfo)
                            {
                                GameObject NuniInfoObj = Instantiate(item.Prefab, item.Content.transform) as GameObject;
                                StoreButton NuniInfo = NuniInfoObj.GetComponent<StoreButton>();

                                NuniInfo.SetNuniData(items.Value);
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