﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UniRx;
public class UIFriendPanel : UIBase
{
    // Start is called before the first frame update


    public GameObject Content;


    [SerializeField]
    public List<FriendBtn> FriendBtns;

    public InputField SearchObject;
    public Text NoFriendTxt;

    public GameObject LoadingPanel;

    public UIFriendPanel(GameObject UIPrefab)
    {
        UIFriendPanel r = UIPrefab.GetComponent<UIFriendPanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        this.UIPrefab = r.InstantiatePrefab() as GameObject;
    }
    override public void Start()
    {

        base.Start();
       
        foreach (var FriendBtns in FriendBtns)
        {
            FriendBtns.Btn.OnClickAsObservable().Subscribe(_ => {
                Friend_Exit();      //목록 초기화
                NoFriendTxt.gameObject.SetActive(false);
                SearchObject.gameObject.SetActive(false);

                LoadingPanel.SetActive(true);

                switch (FriendBtns.FriendUIDef)
                {
                    case FriendDef.GetFriend:                   //친구 목록 가져오기
                       
                        FirebaseLogin.Instance.GetFriend(GameManager.Instance.PlayerUserInfo.Uid).ContinueWith((task) => {
                            if (!task.IsFaulted)
                            {
                                if (task.Result != null)
                                {
                                    Debug.Log("친구 목록 받아온 결과: " + task.Result);

                                    try
                                    {

                                        Newtonsoft.Json.Linq.JArray Result = Newtonsoft.Json.Linq.JArray.Parse(task.Result);
                                        UnityMainThreadDispatcher.Instance().Enqueue(() => {


                                        foreach (var item in Result)
                                        {
                                            Debug.Log("item: " + item.ToString());
                                            FriendInfo itemFriend = JsonUtility.FromJson<FriendInfo>(item.ToString());
                                            //Debug.Log("item: " + JsonUtility.ToJson(item))

                                         
                                                GameObject FriendUI = Instantiate(FriendBtns.Prefab, Content.transform);       //친구 UI 띄우기
                                                FriendUI.name = itemFriend.FriendName;
                                                FriendUI.GetComponent<FriendInfoUI>().SetFriendInfo(itemFriend);                //친구 버튼 세팅

                                            
                                            //LoadManager.Instance.MyFriends.Add(itemFriend.f_nickname, itemFriend);      //친구 딕셔너리에 추가
                                        }
                                            LoadingPanel.SetActive(false);
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.LogError(e.Message);
                                        throw;
                                    }

                                }
                                else
                                {
                                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                    {
                                        LoadingPanel.SetActive(false);

                                        NoFriendTxt.gameObject.SetActive(true);
                                        Debug.Log("task is null");
                                    });
                                }
                            }
                        });
                        break;
                    case FriendDef.RequestFriend:
                        FirebaseLogin.Instance.GetRequestFriend(GameManager.Instance.PlayerUserInfo.Uid).ContinueWith((task) => {
                            if (!task.IsFaulted)
                            {
                                if (task.Result != null)
                                {
                                    Debug.Log("친구 목록 받아온 결과: " + task.Result);

                                    try
                                    {

                                        Newtonsoft.Json.Linq.JArray Result = Newtonsoft.Json.Linq.JArray.Parse(task.Result);
                                        UnityMainThreadDispatcher.Instance().Enqueue(() => {


                                        foreach (var item in Result)
                                        {
                                            Debug.Log("item: " + item.ToString());
                                            FriendInfo itemFriend = JsonUtility.FromJson<FriendInfo>(item.ToString());
                                            //Debug.Log("item: " + JsonUtility.ToJson(item))

                                         
                                                GameObject FriendUI = Instantiate(FriendBtns.Prefab, Content.transform);       //친구 UI 띄우기
                                                FriendUI.name = itemFriend.FriendName;
                                                FriendUI.GetComponent<FriendPlusUI>().SetFriendInfo(itemFriend);                //친구 버튼 세팅

                                        
                                            //LoadManager.Instance.MyFriends.Add(itemFriend.f_nickname, itemFriend);      //친구 딕셔너리에 추가
                                        }
                                            LoadingPanel.SetActive(false);
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.LogError(e.Message);
                                        throw;
                                    }

                                }
                                else
                                {
                                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                    {

                                        LoadingPanel.SetActive(false);
                                        NoFriendTxt.gameObject.SetActive(true);
                                        Debug.Log("task is null");
                                    });
                                }
                            }
                        });
                        break;
                    case FriendDef.SearchFriend:
                        LoadingPanel.SetActive(false);              // 로딩창 끄기
                        SearchObject.gameObject.SetActive(true);            //검색창 띄우기

                        SearchObject.OnEndEditAsObservable().Subscribe(_ => {
                            LoadingPanel.SetActive(true);           //검색중이면 로딩창 켜기
                            Debug.Log("입력끝 " + SearchObject.text);
                            Friend_Exit();      //목록 초기화
                            FirebaseLogin.Instance.GetSearchFriend(SearchObject.text).ContinueWith((task) => {
                                if (!task.IsFaulted)
                                {
                                    if (task.Result != null)//누니 넣기
                                    {
                                        Debug.Log("친구 목록 받아온 결과: " + task.Result);

                                        try
                                        {
                                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                            {

                                                if (task.Result != null)
                                                {
                                                    FriendInfo SearchFriendInfo = JsonUtility.FromJson<FriendInfo>(task.Result);



                                                    GameObject FriendUI = Instantiate(FriendBtns.Prefab, Content.transform);       //친구 UI 띄우기
                                                    FriendUI.name = SearchObject.text;
                                                    FriendUI.GetComponent<FriendPlusUI>().SetFriendInfo(SearchFriendInfo);                //친구 버튼 세팅
                                                    LoadingPanel.SetActive(false);

                                                }
                                                else
                                                {
                                                    LoadingPanel.SetActive(false);
                                                    NoFriendTxt.gameObject.SetActive(true);
                                                }
                                            });

                                        }
                                        catch (Exception e)
                                        {
                                            Debug.LogError(e.Message);
                                            throw;
                                        }

                                    }
                                    else
                                    {
                                        Debug.Log("task is null");
                                    }
                                }
                            });
                        });
                        break;
                    case FriendDef.RecommendFriend:
                        break;
                    default:
                        break;
                }
            }).AddTo(this);
        }

    }
    public void Friend_Exit()           //목록 초기화
    {
        Transform[] child = Content.GetComponentsInChildren<Transform>();          
        for (int k = 1; k < child.Length; k++)
        {
            Destroy(child[k].gameObject);
        }
    }
}
