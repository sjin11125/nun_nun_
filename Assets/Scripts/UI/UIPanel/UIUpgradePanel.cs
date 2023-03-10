using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UniRx;
using UnityEngine.AddressableAssets;

public class UIUpgradePanel : UIBase
{
    public Text UpgradeTextCost;
    public Text UpgradeTextBefore;
    public Text UpgradeTextAfter;

    public Building building;
    public GameObject NoEffectPanel;
    public GameObject NoMoneyPanel;

    int MoneyCost = 0;
    int ShinMoneyCost = 0;
    public UIUpgradePanel(GameObject UIPrefab, Building building)
    {
        /*  base.Awake();
          this.UIPrefab = UIPrefab;
          this.building = building;*/

        UIUpgradePanel r = UIPrefab.GetComponent<UIUpgradePanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;
        r.building = building;

        r.InstantiatePrefab();


    }
  

    private void Start()
    {
        if (UIYesBtn != null)
        {

            UIYesBtn.onClick.AsObservable().Subscribe(_ =>
            {
                Upgrade(building);

            }).AddTo(this);

        }
        if (UINoBtn != null)
        {

            UINoBtn.onClick.AsObservable().Subscribe(_ =>
            {
                this.gameObject.transform.parent.gameObject.SetActive(false);
                Destroy(this.gameObject);

            }).AddTo(this);
        }
        if (UICloseBtn != null)
        {

            UICloseBtn.onClick.AsObservable().Subscribe(_ =>
            {
                this.gameObject.transform.parent.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }).AddTo(this);
        }

                UpgradeTextBefore.text =GameManager.Instance.BuildingInfo[building.Building_Image].Reward [building.Level - 1].ToString();     //???? ?? ȹ?? ??ȭ
                Debug.Log("??????: " + GameManager.Instance.BuildingInfo[building.Building_Image].Reward[building.Level - 1]);

                MoneyCost = GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level];
                ShinMoneyCost= GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level];

                UpgradeTextAfter.text = GameManager.Instance.BuildingInfo[building.Building_Image].Reward[building.Level].ToString();                       //???? ?? ȹ?? ??ȭ
                Debug.Log("??????: " + GameManager.Instance.BuildingInfo[building.Building_Image].Reward[building.Level - 1]);

                UpgradeTextCost.text = "????: " + MoneyCost.ToString() + ",   ?????? ????: " + ShinMoneyCost.ToString() + " ?? ?Ҹ??˴ϴ?.";
       

            
        

       
    }

    public void Upgrade(Building building)
    {
        bool isUp=false;
        if (building.Level < 2)
        {
            if (building.Building_Image == "building_level(Clone)" ||
                   building.Building_Image == "village_level(Clone)" ||
                   building.Building_Image == "flower_level(Clone)")
            {
                Debug.Log("?ش? ?ǹ?????");
                foreach (var item in GameManager.Instance.CharacterList)
                {
                    if (item.Value.cardName=="??????????")
                    {
                        Debug.Log("?ش? ?????̽?");
                        isUp = true;
                        break;
                    }
                }
           
            }
            //GameObject UPPannel = Instantiate(UpgradePannel);
            if (building.Building_Image == "syrup_level(Clone)" ||
             building.Building_Image == "fashion_level(Clone)" ||
             building.Building_Image == "school_level(Clone)")
            {
                Debug.Log("?ش? ?ǹ?????22");
               foreach (var item in GameManager.Instance.CharacterList)
                {
                    if (item.Value.cardName=="????Ʈ????")
                    {
                        Debug.Log("?ش? ?????̽?");
                        isUp = true;
                        break;
                    }
                }
            }
            if (isUp == true)               //?ش? ???? ???? ?? ???׷??̵? O
            {
                MoneyCheck();
            }
            else               //?ش? ???? ???? ?? ???׷??̵? X
            {
                UIYesNoPanel UiMoneyCheckPanel = new UIYesNoPanel(NoEffectPanel);
            }

        }
        return;
    }

    void MoneyCheck()
    {


        if (GameManager.Instance.ShinMoney.Value >= ShinMoneyCost &&           //??ȭ üũ
           GameManager.Instance.Money.Value >= MoneyCost)
        {
            GameManager.Instance.ShinMoney.Value -= ShinMoneyCost;
            GameManager.Instance.Money.Value -= MoneyCost;

            building.Level += 1;
            Addressables.LoadAssetAsync<Sprite>(building.Building_Image + building.Level.ToString()).Completed += (image) =>
            {            //???巹?????? ?̹??? ?ҷ??? ?ֱ?
                building.BuildingImage.sprite = image.Result;

            };
            //  building.BuildingImage.sprite = GameManager.GetDogamChaImage(building.Building_Image + building.Level.ToString());//?ǹ??̹??? ?ٲ?
        }
        else                                             //??ȭ?? ????
        {
            UIYesNoPanel UiMoneyCheckPanel = new UIYesNoPanel(NoMoneyPanel);

        }
    }
}
