using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UniRx;

public class InventoryButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Image X_Image;     //�ǹ� ȸ�� ��ư

    Building this_building;         //�� ��ư�� �ش��ϴ� �ǹ�
    public Building temp_building
    {
        get { return this_building; }
        set { this_building = value.DeepCopy(); }
    }
    public Card this_nuni;         //�� ��ư�� �ش��ϴ� ����
    GridBuildingSystem gridBuildingSystem;

    public GameObject buildings;
    public GameObject nunis;

    private GameObject settigPanel;
    public Button button;

    public void SetButtonImage(Sprite image)
    {
        this.GetComponent<Image>().sprite = image;
    }
    public Building SetBuildingInfo( Building building)
    {
        this_building=building;
        return this_building;
    }

    public void SetNoImage(bool isLock)
    {
        if (isLock)
            X_Image.gameObject.SetActive(false);
        else
            X_Image.gameObject.SetActive(true);
    }
    public void SetSelectedImage(bool isSelected)
    {
        if (isSelected)
            this.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
        else
            this.GetComponent<Image>().color = new Color(1f, 1f, 1f);
    }
    void Start()
    {
        if (temp_building != null)
        {

            if (temp_building.isLock.Equals("F"))
            {
                X_Image.gameObject.SetActive(true);
            }
            else
            {
                X_Image.gameObject.SetActive(false);
            }
        }
        if (this_nuni!=null)
        {
            if (this_nuni.isLock.Equals("F"))
            {
                X_Image.gameObject.SetActive(true);
            }
            else
            {
                X_Image.gameObject.SetActive(false);
            }
        }
        /*button.OnClickAsObservable().Subscribe(_=>{

           /if (temp_building.isLock == "T")         //�ش� �ǹ��� ��ġ�Ǿ�����
            {
                temp_building.Type = BuildType.Load;
                GridBuildingSystem.OnEditMode.OnNext(temp_building);  //�Ǽ���� ON (Ÿ�� �ʱ�ȭ)
                LoadManager.Instance.RemoveBuilding(temp_building.Id); //�ش� ������ ����

                temp_building.isLock = "F";                            //��ġ�ȵ� ���·� �ٲٱ�
                temp_building.BuildingPosition_x = "0";
                temp_building.BuildingPosition_y = "0";

                LoadManager.Instance.MyBuildings[temp_building.Id].SetValue(temp_building);     //���� ������Ʈ ���ֱ�
                Debug.Log(LoadManager.Instance.MyBuildings[temp_building.Id].isLock);
            }
            else                               //�ش� �ǹ��� ��ġ�ȵǾ�������
            {

                Building ActiveBuilding = LoadManager.Instance.InstatiateBuilding(temp_building);
                ActiveBuilding.Type = BuildType.Move;

                GridBuildingSystem.OnEditMode.OnNext(ActiveBuilding);  //�Ǽ���� ON
                temp_building.isLock = "T";                //��ġ�� ���·� �ٲٱ�
            }

            LoadManager.Instance.buildingsave.BuildingReq(BuildingDef.updateValue, temp_building);     //������ ����
           
        }).AddTo(this);*/

        if (gameObject.tag.Equals("Inven_Building"))
        {
            buildings = GameObject.Find("buildings");

           
                    //this_building = LoadManager.MyBuildings[this.gameObject.name];
                    gridBuildingSystem = buildings.GetComponentInChildren<GridBuildingSystem>();
                
            
            

        }
        else if(gameObject.tag .Equals( "Inven_Nuni"))
        {
            nunis= GameObject.Find("nunis"); 
            gridBuildingSystem = gameObject.transform.parent.parent.GetComponent<GridBuildingSystem>();

            if (this_nuni.isLock .Equals( "F"))
            {
                X_Image.gameObject.SetActive(true);
            }
            else
            {
                X_Image.gameObject.SetActive(false);
            }
        }

        settigPanel = GameObject.FindGameObjectWithTag("SettingPanel");
    }

  
    public void nuni_Click()
    {
        if (this_nuni.isLock.Equals("T") )     //���ϰ� ��ġ�� ����
        {
            this_nuni.isLock = "F";         //��ġ �ȵ� ���·� �ٲٱ�
            Transform[] nuni_child = nunis.GetComponentsInChildren<Transform>();
            X_Image.gameObject.SetActive(true);
            for (int i = 0; i < nuni_child.Length; i++)                     //���� ��Ͽ��� �ش� ���� ã�Ƽ� ���ֱ�
            {
                if (nuni_child[i].gameObject.name .Equals( this_nuni.cardImage+"(Clone)"))
                {
                    
                    Card nuni_childs = nuni_child[i].gameObject.GetComponent<Card>();
                    if (nuni_childs.isLock.Equals("T"))
                    {
                        nuni_childs.isLock = "F";
                        Destroy(nuni_child[i].gameObject);
                        Debug.Log(nuni_child[i].gameObject.name);
                       // StartCoroutine(NuniSave(this_nuni));          //���� ��ũ��Ʈ�� ������Ʈ
                    }
                   
                    return;
                }
            }
          

        }
        else                                    //���ϰ� ��ġ �ȵ� ����
        {
            
            this_nuni.isLock = "T";         //��ġ �� ���·� �ٲٱ�

            X_Image.gameObject.SetActive(false);
            foreach (var item in GameManager.Instance.CharacterList)
            {
                if (this_nuni.cardName.Equals(item.Value.cardName))
                {
                    item.Value.isLock = "T";
                    GameObject nuni = Instantiate(GameManager.CharacterPrefab[this_nuni.cardImage], nunis.transform) as GameObject;

                    for (int j = 0; j < GameManager.AllNuniArray.Length; j++)
                    {
                        if (GameManager.AllNuniArray[j].cardImage != this_nuni.cardImage)
                            continue;

                        Card Value = nuni.GetComponent<Card>();
                        Value.SetValue(GameManager.AllNuniArray[j]);
                    }

                    nuni.GetComponent<Card>().isLock = "T";
                    //StartCoroutine(NuniSave(this_nuni));          //���� ��ũ��Ʈ�� ������Ʈ
                    return;
                }

            }

        }
        settigPanel.GetComponent<AudioController>().Sound[0].Play();
    }
    /*IEnumerator NuniSave(Card nuni)                //���� ���� ��ũ��Ʈ�� ����
    {

        WWWForm form1 = new WWWForm();
        form1.AddField("order", "nuniUpdate");
        form1.AddField("player_nickname", GameManager.NickName);
        form1.AddField("nuni", nuni.cardName +":"+this_nuni.isLock);



        yield return StartCoroutine(Post(form1));                        //���� ��ũ��Ʈ�� �ʱ�ȭ�ߴ��� ��������� ���


    }
    IEnumerator Post(WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(GameManager.URL, form)) // �ݵ�� using�� ����Ѵ�
        {
            yield return www.SendWebRequest();
            //Debug.Log(www.downloadHandler.text);
           // if (www.isDone) NuniResponse(www.downloadHandler.text);
            //else print("���� ������ �����ϴ�.");
        }

    }*/


   /* public void Click()         //���๰ ��ư Ŭ������ ��
    {
    

        if (gridBuildingSystem.temp_gameObject!=null)
        {
            Building c = gridBuildingSystem.temp_gameObject.GetComponent<Building>();


            gridBuildingSystem.prevArea2 = c.area;
            gridBuildingSystem.ClearArea2();
            //gridBuildingSystem.CanTakeArea(c.area);
            Destroy(gridBuildingSystem.temp_gameObject);

            
        }

        if (GameManager.CurrentBuilding_Button ==null )      //�� ���� Ŭ���ߴ� ��ư�� ���� ��
        {
            GameManager.CurrentBuilding_Button = this;
        }
        else
        {
            if (GameManager.CurrentBuilding_Button.this_building.Id!=this.this_building.Id&& GameManager.CurrentBuilding_Button.this_building.isLock .Equals( "T"))
            {
                
            
                GameManager.CurrentBuilding_Button.this_building.isLock = "F";
                GameManager.CurrentBuilding_Button.X_Image.gameObject.SetActive(true);
                GameManager.CurrentBuilding_Button = this;
            }

        }
        Transform[] building_child = buildings.GetComponentsInChildren<Transform>();
     
        if (this_building.isLock.Equals("T"))      //���� ��ġ�� �����ΰ�
        {
         
            if (GameManager.CurrentBuilding != null)
            {
                Building c = GameManager.CurrentBuilding.GetComponent<Building>();


                gridBuildingSystem.prevArea2 = c.area;
                gridBuildingSystem.ClearArea2();
                //gridBuildingSystem.CanTakeArea(c.area);
            }
            for (int i = 0; i < building_child.Length; i++)
            {
                if (building_child[i].name .Equals( this_building.Id))
                {
                    //buildingprefab = building_child[i].gameObject;
                    GameManager.CurrentBuilding = building_child[i].gameObject;

        
                    Building b = GameManager.CurrentBuilding.GetComponent<Building>();
                    gridBuildingSystem.prevArea2 = b.area;
                    gridBuildingSystem.RemoveArea(b.area);
                    gridBuildingSystem.CanTakeArea(b.area);
                    //b.Remove(GameManager.CurrentBuilding.GetComponent<Building>());
                    Building c = building_child[i].gameObject.GetComponent<Building>();
                    gridBuildingSystem.RemoveArea(c.area);
                    Destroy(building_child[i].gameObject);

                }
            }
            GameManager.isEdit = false;
           
            //GameManager.CurrentBuilding = null;
            this_building.isLock = "F";         //��ġ �ȵ� ���·� �ٲٱ�
            X_Image.gameObject.SetActive(true);
           





            for (int i = 0; i < building_child.Length; i++)
            {
                if (building_child[i].gameObject.name .Equals(GameManager.CurrentBuilding.name))
                {
                    Building building_childs = building_child[i].gameObject.GetComponent<Building>();
                    Destroy(building_childs);
                }
            }

            for (int i = 1; i < building_child.Length; i++)
            {
                if (building_child[i].gameObject.name .Equals( gameObject.name))
                {
                    Building building_childs = building_child[i].gameObject.GetComponent<Building>();
                    building_childs.isLock = "F";
                    building_childs.BuildingPosition_x = "0";
                    building_childs.BuildingPosition_y = "0";

                    building_childs.save.BuildingReq(BuildingDef.updateValue, building_childs);
                    Destroy(building_child[i].gameObject);
                }
                
            }
            GameManager.CurrentBuilding = null;
            GameManager.CurrentBuilding_Button = null;
            gridBuildingSystem.GridLayerNoSetting();                //���� Ÿ�� �Ⱥ��̰�
        }
        else if(this_building.isLock .Equals( "F"))                     //���� ��ġ�� ���°� �ƴѰ�
        {
            X_Image.gameObject.SetActive(false);
            this_building.isLock = "T";         //��ġ �� ���·� �ٲٱ�
            GameManager.InvenButton =this.GetComponent<Button>();
            if (GameManager.CurrentBuilding!=null)
            {
                Building c = GameManager.CurrentBuilding.GetComponent<Building>();

             
                gridBuildingSystem.prevArea2 = c.area;
                gridBuildingSystem.ClearArea2();
                gridBuildingSystem.CanTakeArea(c.area);
            }

            for (int i = 0; i < GameManager.BuildingArray.Length; i++)
            {
                if (GameManager.BuildingArray[i].Building_Image.Equals( this_building.Building_Image))
                {
                    GameManager.CurrentBuilding =GameManager.BuildingPrefabData[this_building.Building_Image];
                    Building c = GameManager.CurrentBuilding.GetComponent<Building>();
                    
                    c.SetValue(this_building);

                    Building b = GameManager.CurrentBuilding.GetComponent<Building>();
                    gridBuildingSystem.prevArea2 = b.area;
                    gridBuildingSystem.ClearArea2();
                    gridBuildingSystem.CanTakeArea(b.area);
                    break;
                }
            }
            for (int i = 0; i < GameManager.StrArray.Length; i++)
            {
              if (GameManager.StrArray[i].Building_Image .Equals( this_building.Building_Image))
                {
                    GameManager.CurrentBuilding = GameManager.BuildingPrefabData[this_building.Building_Image];
                    Building c = GameManager.CurrentBuilding.GetComponent<Building>();

                    c.SetValue(this_building);

                    Building b = GameManager.CurrentBuilding.GetComponent<Building>();
                  
                    gridBuildingSystem.prevArea2 = b.area;
                    gridBuildingSystem.ClearArea2();
                    gridBuildingSystem.CanTakeArea(b.area);
                    break;
                }
            }

            //GameManager.CurrentBuilding.name = this_building.Building_Image;
            gridBuildingSystem.GridLayerSetting();          //���� Ÿ�� ���̰�
            GameManager.isInvenEdit = true;
            //gridBuildingSystem.Inven_Move(GameManager.CurrentBuilding.transform);
            GameManager.CurrentBuilding_Button = this;

        }
        settigPanel.GetComponent<AudioController>().Sound[0].Play();
    }*/
    
}
