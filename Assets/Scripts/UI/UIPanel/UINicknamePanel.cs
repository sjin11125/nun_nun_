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
    public UINicknamePanel(GameObject UIPrefab,Action action)
    {
        UINicknamePanel r = UIPrefab.GetComponent<UINicknamePanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        this.UIPrefab = r.InstantiatePrefab() as GameObject;
        this.Callback = action;
    }
  public override  void Start()
    {
        base.Start();
        OkBtn.OnClickAsObservable().Subscribe(_=> {

            //�г� üũ(����� ��������)
            ExistNickNameTxt.SetActive(false);
            FirebaseLogin.Instance.NickNameCheck(NickInputField.text).ContinueWith((task) => {
                Debug.Log("�г��� "+task.Result);
                switch (task.Result.ToString())
                {
                    case "Success":
                        break;

                    case "Fail":
                        break;

                    default:
                        break;
                }
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
