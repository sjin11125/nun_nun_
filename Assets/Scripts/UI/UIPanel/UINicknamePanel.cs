using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

public class UINicknamePanel : UIBase
{
    // Start is called before the first frame update
    public Action Callback;
    public Button OkBtn;
    public GameObject ExistNickNameTxt;
    public InputField NickInputField;
    public UINicknamePanel(GameObject UIPrefab)
    {
        UINicknamePanel r = UIPrefab.GetComponent<UINicknamePanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        this.UIPrefab = r.InstantiatePrefab() as GameObject;
    }
  public override  void Start()
    {
        base.Start();
        OkBtn.OnClickAsObservable().Subscribe(_=> {

            //�г� üũ(����� ��������)
            ExistNickNameTxt.SetActive(false);
            FirebaseLogin.Instance.NickNameCheck(NickInputField.text).ContinueWith((task) => {
                Debug.Log("�г��� "+(string)task.Result);
                UnityMainThreadDispatcher.Instance().Enqueue(()=> {

                    SendMessage itemFriend = JsonUtility.FromJson<SendMessage>(task.Result.ToString());

                    switch (itemFriend.message)
                    {
                        case "Success":

                            LoadingSceneController.Instance.LoadScene(SceneName.Main);
                            Destroy(this.gameObject);
                            break;

                        case "Fail":
                            ExistNickNameTxt.SetActive(true);
                            break;

                        default:
                            break;
                    }
                });
            
            }); //������ ���� �г� �ִ��� üũ
           // LoadingSceneController.Instance.LoadScene(SceneName.Main);
            //Callback.Invoke();//�ݹ� ����
        });
    }

    public void SetExistNickname()
    {
        ExistNickNameTxt.SetActive(true);
    }
}
