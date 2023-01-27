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

            //닉넴 체크(몇글자 이하인지)
            ExistNickNameTxt.SetActive(false);
            FirebaseLogin.Instance.NickNameCheck(NickInputField.text).ContinueWith((task) => {
                Debug.Log("닉넴은 "+(string)task.Result);
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
            
            }); //서버에 같은 닉넴 있는지 체크
           // LoadingSceneController.Instance.LoadScene(SceneName.Main);
            //Callback.Invoke();//콜백 실행
        });
    }

    public void SetExistNickname()
    {
        ExistNickNameTxt.SetActive(true);
    }
}
