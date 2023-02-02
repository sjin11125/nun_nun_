using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft;
using UniRx;

public class UIAchievePanel : UIBase
{
    // Start is called before the first frame update
    [SerializeField]
    public List<AchieveMenu> AchieveMenus;

    public GameObject LoadingPanel;
    public GameObject Content;
    public UIAchievePanel(GameObject UIPrefab)
    {
        UIAchievePanel r = UIPrefab.GetComponent<UIAchievePanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        r.InstantiatePrefab();
    }
    override public void Start()
    {
        base.Start();
        //처음에 업적 창 열면 색깔 업적 부터 뜨게함
        LoadingPanel.SetActive(true);
        foreach (var info in GameManager.Instance.AchieveInfos)
        {
            if (info.Value.Id[0] != 'C')
                continue;

            GameObject AchieveInfoObj = Instantiate(AchieveMenus[0].Prefab, Content.transform) as GameObject;
            AchieveScroll AchieveInfo = AchieveInfoObj.GetComponent<AchieveScroll>();



            AchieveInfo.SetData(info.Value);
        }
        LoadingPanel.SetActive(false);

       
        foreach (var item in AchieveMenus)
        {
            item.Btn.OnClickAsObservable().Subscribe(_=> {
                Exit();
                LoadingPanel.SetActive(true);
                switch (item.Type)
                {
                    case AchieveMenuType.Color:
                        LoadingPanel.SetActive(true);
                        foreach (var info in GameManager.Instance.AchieveInfos)
                        {
                            if (info.Value.Id[0] != 'C')
                                continue;

                            GameObject AchieveInfoObj = Instantiate(item.Prefab, Content.transform) as GameObject;
                            AchieveScroll AchieveInfo = AchieveInfoObj.GetComponent<AchieveScroll>();



                            AchieveInfo.SetData(info.Value);
                        }
                        LoadingPanel.SetActive(false);
                        break;

                    case AchieveMenuType.Ect:
                        LoadingPanel.SetActive(true);
                        foreach (var info in GameManager.Instance.AchieveInfos)
                        {
                            if (info.Value.Id[0] != 'E')
                                continue;

                            GameObject AchieveInfoObj = Instantiate(item.Prefab, Content.transform) as GameObject;
                            AchieveScroll AchieveInfo = AchieveInfoObj.GetComponent<AchieveScroll>();

                            AchieveInfo.SetData(info.Value);
                        }
                        LoadingPanel.SetActive(false);
                        break;

                    case AchieveMenuType.Shape:
                        LoadingPanel.SetActive(true);
                        foreach (var info in GameManager.Instance.AchieveInfos)
                        {
                            if (info.Value.Id[0] != 'S')
                                continue;

                            GameObject AchieveInfoObj = Instantiate(item.Prefab, Content.transform) as GameObject;
                            AchieveScroll AchieveInfo = AchieveInfoObj.GetComponent<AchieveScroll>();

                            AchieveInfo.SetData(info.Value);
                        }
                        LoadingPanel.SetActive(false);
                        break;

                    default:
                        break;
                }

            }).AddTo(this);
        }
        LoadingPanel.SetActive(false);
       //Newtonsoft.Json.dese
    }
    public void Exit()
    {
        Transform[] Content_Child = Content.GetComponentsInChildren<Transform>();
        for (int i = 1; i < Content_Child.Length; i++)
        {
            Destroy(Content_Child[i].gameObject);
        }
    }
    //Json.Linq.JObject

}
