using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.AddressableAssets;

public class StoreButton : MonoBehaviour
{
    // Start is called before the first frame update

    public Text Name, Info, Effect1, Effect2;
    public Image Image;
    public Button BuyBtn;
    public Text MoneyText, ShinMoneyText;

    public string Building_Image;



    public void SetStoreData(BuildingParse buildingInfo)
    {
        if (Name != null)
            Name.text = buildingInfo.Building_name;

        if (Info != null)
            Info.text = buildingInfo.Info;

        if (Effect1 != null)
            Effect1.text = buildingInfo.Reward[0].ToString();

        if (Effect2 != null)
            Effect2.text = buildingInfo.Reward[1].ToString();

        if (MoneyText != null)
            MoneyText.text = buildingInfo.Cost[0].ToString();

        if (ShinMoneyText != null)
            ShinMoneyText.text = buildingInfo.ShinCost[0].ToString();

        // Building_Image = buildingInfo.Building_Image;

        Addressables.LoadAssetAsync<Sprite>(buildingInfo.Building_Image).Completed += (image) =>
        {            //어드레서블로 이미지 불러서 넣기
            Image.sprite = image.Result;

        };
        if (BuyBtn != null)
        {
            BuyBtn.OnClickAsObservable().Subscribe(_ => {                //구매버튼

            }).AddTo(this);
        }
    }

    public void SetNuniData(Card nuniInfo)
    {
        if (Name != null)
            Name.text = nuniInfo.cardName;

        Addressables.LoadAssetAsync<Sprite>(nuniInfo.cardImage).Completed += (image) =>
        {            //어드레서블로 이미지 불러서 넣기
            Image.sprite = image.Result;

        };
        if (BuyBtn != null)
        {
            BuyBtn.OnClickAsObservable().Subscribe(_ => {                //구매버튼

            }).AddTo(this);
        }
    }
}
