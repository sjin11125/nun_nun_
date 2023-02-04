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


                    Card Value = nuni.GetComponent<Card>();
                    Value = new Card(GameManager.Instance.NuniInfo[this_nuni.cardImage]);
                    Value.isLock = "T";
                    //StartCoroutine(NuniSave(this_nuni));          //���� ��ũ��Ʈ�� ������Ʈ
                    return;
                }

            }

        }
        settigPanel.GetComponent<AudioController>().Sound[0].Play();
    }
    
}
