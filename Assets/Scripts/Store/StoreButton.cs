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

    [SerializeField]
    public StoreNuniInfo StoreNuniInfo;

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

        gameObject.name = buildingInfo.Building_Image;
        // Building_Image = buildingInfo.Building_Image;

        Addressables.LoadAssetAsync<Sprite>(buildingInfo.Building_Image).Completed += (image) =>
        {            //어드레서블로 이미지 불러서 넣기
            Image.sprite = image.Result;

        };
       /* if (BuyBtn != null)
        {
            BuyBtn.OnClickAsObservable().Subscribe(_ => {                //구매버튼
                int money = int.Parse(GameManager.Instance.PlayerUserInfo.Money);
                int shinmoney = int.Parse(GameManager.Instance.PlayerUserInfo.ShinMoney);

                if (GameManager.Instance.BuildingInfo[buildingInfo.Building_Image].Cost[0] <= money //자원체크(돈있으면 결제, 없으면 돈없다는 패널뜸)
                && GameManager.Instance.BuildingInfo[buildingInfo.Building_Image].ShinCost[0] <= shinmoney)
                {
                    money -= GameManager.Instance.BuildingInfo[buildingInfo.Building_Image].Cost[0]; //결제
                    shinmoney -= GameManager.Instance.BuildingInfo[buildingInfo.Building_Image].ShinCost[0];


                    Addressables.LoadAssetAsync<GameObject>(buildingInfo.Path).Completed += (gameobject) =>
                    {            //어드레서블로 이미지 불러서 넣기
                        Building Newbuilding = gameobject.Result.GetComponent<Building>();

                        Newbuilding.Placed = false;
                        Newbuilding.Type = BuildType.Make;
                        Newbuilding.area = gameobject.Result.GetComponent<Building>().area;


                        LoadManager.Instance.InstantiateBuilding(Newbuilding,()=> {

                            GridBuildingSystem.OnEditMode.OnNext(Newbuilding);
                            this.gameObject.transform.parent.gameObject.SetActive(false);
                            Destroy(this.gameObject);
                        });
                    };  //어드레서블에서 건물 프리팹 불러와 Instantiate함
                }
                else
                {
                    //돈부족 패널 뜨기
                }
            }).AddTo(this);
        }*/
    }

    public void SetNuniData(CardInfo nuniInfo)
    {
        if (Name != null)
            Name.text = nuniInfo.cardName;

        Addressables.LoadAssetAsync<Sprite>(nuniInfo.cardImage).Completed += (image) =>
        {            //어드레서블로 이미지 불러서 넣기
            Image.sprite = image.Result;

        };
    }
}
