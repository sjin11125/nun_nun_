using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using UniRx;
using UnityEngine.AddressableAssets;

public class PlayerInfo : MonoBehaviour                 //�÷��̾� ������ ��ũ��Ʈ
{
    public static string Id;            //�÷��̾� ���̵�
    public static string NickName;      //�÷��̾� �г���
    public static string SheetsNum;     //�÷��̾� �ǹ� ���� ����ִ� �������� ��Ʈ id
    public static string Info;          //���¸޼���

    public Image ProfileImage;      //�� ������ �̹���

    public InputField InfoInput;        //���ټҰ� ����

    void Start()
    {  //GameManager.Instance.ProfileImage.Subscribe((value) => ProfileImage.sprite = value). ;// = GameManager.AllNuniArray[i].Image;
        GameManager.Instance.ProfileImage.AsObservable().Subscribe((value) =>
        {
            ProfileImage.sprite = value;

        });
        Addressables.LoadAssetAsync<Sprite>(GameManager.Instance.PlayerUserInfo.Image).Completed += (image) =>
        {            //��巹����� �̹��� �ҷ��� �ֱ�
            ProfileImage.sprite = image.Result;

        };

    }

  


    public void EditInfo()                  //���ټҰ� ����
    {
        GameManager.Instance.PlayerUserInfo.Message = InfoInput.text;


        FirebaseScript.Instance.SetUserInfo(GameManager.Instance.PlayerUserInfo);
    }
}
