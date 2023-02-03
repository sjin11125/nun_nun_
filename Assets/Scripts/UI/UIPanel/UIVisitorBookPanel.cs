using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using System;

public class UIVisitorBookPanel : UIBase
{
    //
    //InputField
    public InputField MessageInputField;
    public GameObject MessagePrefab;
    public Transform Content;
    public GameObject LoadingPanel;

    string Uid;

    public UIVisitorBookPanel(GameObject UIPrefab)
    {
        UIVisitorBookPanel r = UIPrefab.GetComponent<UIVisitorBookPanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        r.InstantiatePrefab();
    }

    override public void Start()
    {
        base.Start();
        LoadingPanel.SetActive(true);


        if (SceneManager.GetActiveScene().name=="Main")             //�� ���� �� �̶��
            Uid = GameManager.Instance.PlayerUserInfo.Uid;
        else                //ģ�����̶��
            Uid = LoadManager.Instance.FriendUid;


        FirebaseLogin.Instance.GetVisitorBook(Uid).ContinueWith((task) => {

            if (!task.IsFaulted)
            {
                if (task.Result != null)
                {
                    Debug.Log("task.Result: " + task.Result);
                    Newtonsoft.Json.Linq.JArray Result = Newtonsoft.Json.Linq.JArray.Parse(task.Result);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        foreach (var item in Result)
                        {

                            VisitorMessage Message = Instantiate(MessagePrefab, Content).GetComponent<VisitorMessage>();
        

                            VisitorBookInfo TempInfo = JsonUtility.FromJson<VisitorBookInfo>(item.ToString());

                            Message.SetMessage(TempInfo.FriendName, TempInfo.FriendMessage, TempInfo.FriendTime, TempInfo.FriendImage);

                        }

                        LoadingPanel.SetActive(false);
                    });

                }
            }

        });//ģ�� uid�� ���� �θ���


        MessageInputField.OnEndEditAsObservable().Subscribe(_ => {
            VisitorBookInfo newMessage = new VisitorBookInfo(GameManager.Instance.PlayerUserInfo.Uid,
                                                            MessageInputField.text,
                                                            DateTime.Now.ToString(),
                                                            GameManager.Instance.PlayerUserInfo.Image);
            newMessage.Uid= Uid;

            FirebaseLogin.Instance.SetVisitorBook(newMessage);

            VisitorMessage Message = Instantiate(MessagePrefab, Content).GetComponent<VisitorMessage>();
            Message.SetMessage(newMessage.FriendName, newMessage.FriendMessage, newMessage.FriendTime, newMessage.FriendImage);

        });  //InputField ����
    }

}
