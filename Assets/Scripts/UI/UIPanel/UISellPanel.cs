using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class UISellPanel : UIBase
{
    public Building building;

   public UISellPanel(GameObject UIPrefab, Building building)
    {
        UISellPanel r = UIPrefab.GetComponent<UISellPanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;
        r.building = building;

        r.InstantiatePrefab();
    }

    public void Start()
    {

       // UIPanelName.text = "건물 제거";
       // UIPanelText.text = "건물을 제거하시겠습니까?";

        if (UIYesBtn!=null)
        {
            
            UIYesBtn.onClick.AsObservable().Subscribe(_ =>
            {
                GridBuildingSystem.OnEditModeOff.OnNext(building);
                Remove(building);
             
            }).AddTo(this);

        }
        if (UINoBtn!=null)
        {

            UINoBtn.onClick.AsObservable().Subscribe(_ =>
            {
                this.gameObject.transform.parent.gameObject.SetActive(false);
                Destroy(this.gameObject);

            }).AddTo(this);
        }
        if (UICloseBtn!=null)
        {

            UICloseBtn.onClick.AsObservable().Subscribe(_ =>
            {
                this.gameObject.transform.parent.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }).AddTo(this);
        }
    }
    public void Remove(Building building)
    {
        Debug.Log("건물 제거");

        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(building.gameObject.transform.position);
        BoundsInt areaTemp = building.area;
        areaTemp.position = positionInt;
        GridBuildingSystem.current.RemoveArea(areaTemp);
        GridBuildingSystem.current.temp = null;




        if (building.Type.Equals(BuildType.Make))     //상점에서 사고 설치X 바로 제거
        {
            GameManager.Instance.Money.Value += GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1];

            //CanvasManger.AchieveMoney += GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1];

            GameManager.Instance.ShinMoney.Value += GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1];


           // CanvasManger.AchieveShinMoney += GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1];
            Debug.Log("Money: "+ GameManager.Instance.Money.Value+"  ShinMoney: "+ GameManager.Instance.ShinMoney.Value);
            Destroy(building.transform.gameObject);
        }
        else                                //설치하고 제거
        {
            GameManager.Instance.Money.Value += GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1] / 10;


            // CanvasManger.AchieveMoney += GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1] / 10;

            GameManager.Instance.ShinMoney.Value += GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1] / 3;
  
            Debug.Log("Money: " + GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1] / 10 + "   ShinMoney:" + GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1] / 3);
            Debug.Log("Money: " + GameManager.Instance.Money.Value + "  ShinMoney: " + GameManager.Instance.ShinMoney.Value);

            //CanvasManger.AchieveShinMoney += GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1] / 3;

            LoadManager.RemoveBuildingSubject.OnNext(building);           //현재 가지고 있는 건물 목록에서 제거
            //LoadManager.Instance.buildingsave.BuildingReq(BuildingDef.removeValue, building);
            FirebaseScript.Instance.RemoveBuilding(building.BuildingToJson());

            Destroy(building.transform.gameObject);
        }
        this.gameObject.transform.parent.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
