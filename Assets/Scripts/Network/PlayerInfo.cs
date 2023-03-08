using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using UniRx;
using UnityEngine.AddressableAssets;

public class PlayerInfo : MonoBehaviour                 //플레이어 프로필 스크립트
{
    public static string Id;            //플레이어 아이디
    public static string NickName;      //플레이어 닉네임
    public static string SheetsNum;     //플레이어 건물 정보 들어있는 스프레드 시트 id
    public static string Info;          //상태메세지

    public Image ProfileImage;      //내 프로필 이미지

    public InputField InfoInput;        //한줄소개 수정

    void Start()
    {  //GameManager.Instance.ProfileImage.Subscribe((value) => ProfileImage.sprite = value). ;// = GameManager.AllNuniArray[i].Image;
        GameManager.Instance.ProfileImage.AsObservable().Subscribe((value) =>
        {
            ProfileImage.sprite = value;

        });
        Addressables.LoadAssetAsync<Sprite>(GameManager.Instance.PlayerUserInfo.Image).Completed += (image) =>
        {            //어드레서블로 이미지 불러서 넣기
            ProfileImage.sprite = image.Result;

        };

    }

  


    public void EditInfo()                  //한줄소개 수정
    {
        GameManager.Instance.PlayerUserInfo.Message = InfoInput.text;


        FirebaseScript.Instance.SetUserInfo(GameManager.Instance.PlayerUserInfo);
    }
}
