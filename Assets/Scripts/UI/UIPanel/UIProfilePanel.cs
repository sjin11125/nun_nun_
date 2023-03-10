using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.AddressableAssets;

public class UIProfilePanel : UIBase
{
    public Image ProfileImage;
    public Text NickNameTxt;

    public GameObject Content;

    public GameObject NuniImagePrefab;

    public Button SaveBtn;
    public InputField InputField;
    public UIProfilePanel(GameObject UIPrefab)
    {
        UIProfilePanel r = UIPrefab.GetComponent<UIProfilePanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        this.UIPrefab = r.InstantiatePrefab() as GameObject;
    }
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();

     

        NickNameTxt.text = GameManager.Instance.PlayerUserInfo.Uid;

        InputField.text= GameManager.Instance.PlayerUserInfo.Message;

        ProfileImage.sprite = GameManager.Instance.ProfileImage.Value;
        InputField.OnValueChangedAsObservable().Subscribe(_ => {


        }).AddTo(this);

        foreach (var item in GameManager.Instance.CharacterList)
        {
           
              
                    GameObject ImageObj = Instantiate(NuniImagePrefab, Content.transform) as GameObject;
                    Image PrefabImage = ImageObj.GetComponentInChildren<Image>();
            Addressables.LoadAssetAsync<Sprite>(item.Value.cardImage).Completed += (image) =>
            {            //어드레서블로 이미지 불러서 넣기
                PrefabImage.sprite = image.Result;

            };

                    Button PrefabButton = ImageObj.GetComponentInChildren<Button>();

                    PrefabButton.OnClickAsObservable().Subscribe(_=> {              //누니 사진 버튼 누르면
                        Addressables.LoadAssetAsync<Sprite>(item.Value.cardImage).Completed += (image) =>
                        {            //어드레서블로 이미지 불러서 넣기
                            ProfileImage.sprite = image.Result;

                        };

                       // ProfileImage.sprite= GameManager.AllNuniArray[i].Image;


                    }).AddTo(this);
                 
               
            
        }
        
        SaveBtn.OnClickAsObservable().Subscribe(_ => {          //저장 버튼 누르면
            GameManager.Instance.ProfileImage.Value = ProfileImage.sprite;
            GameManager.Instance.PlayerUserInfo.Image = GameManager.Instance.ProfileImage.Value.name;
            GameManager.Instance.PlayerUserInfo.Message = InputField.text;
            FirebaseScript.Instance.SetUserInfo(GameManager.Instance.PlayerUserInfo); //서버에 저장
        }).AddTo(this);
    }

}
