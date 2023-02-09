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

       // UIPanelName.text = "�ǹ� ����";
       // UIPanelText.text = "�ǹ��� �����Ͻðڽ��ϱ�?";

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
        Debug.Log("�ǹ� ����");

        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(building.gameObject.transform.position);
        BoundsInt areaTemp = building.area;
        areaTemp.position = positionInt;
        GridBuildingSystem.current.RemoveArea(areaTemp);
        GridBuildingSystem.current.temp = null;




        if (building.Type.Equals(BuildType.Make))     //�������� ��� ��ġX �ٷ� ����
        {
            GameManager.Instance.Money.Value += GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1];

            //CanvasManger.AchieveMoney += GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1];

            GameManager.Instance.ShinMoney.Value += GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1];


           // CanvasManger.AchieveShinMoney += GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1];
            Debug.Log("Money: "+ GameManager.Instance.Money.Value+"  ShinMoney: "+ GameManager.Instance.ShinMoney.Value);
            Destroy(building.transform.gameObject);
        }
        else                                //��ġ�ϰ� ����
        {
            GameManager.Instance.Money.Value += GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1] / 10;


            // CanvasManger.AchieveMoney += GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1] / 10;

            GameManager.Instance.ShinMoney.Value += GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1] / 3;
  
            Debug.Log("Money: " + GameManager.Instance.BuildingInfo[building.Building_Image].Cost[building.Level - 1] / 10 + "   ShinMoney:" + GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1] / 3);
            Debug.Log("Money: " + GameManager.Instance.Money.Value + "  ShinMoney: " + GameManager.Instance.ShinMoney.Value);

            //CanvasManger.AchieveShinMoney += GameManager.Instance.BuildingInfo[building.Building_Image].ShinCost[building.Level - 1] / 3;

            LoadManager.RemoveBuildingSubject.OnNext(building);           //���� ������ �ִ� �ǹ� ��Ͽ��� ����
            //LoadManager.Instance.buildingsave.BuildingReq(BuildingDef.removeValue, building);
            FirebaseScript.Instance.RemoveBuilding(building.BuildingToJson());

            Destroy(building.transform.gameObject);
        }
        this.gameObject.transform.parent.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
