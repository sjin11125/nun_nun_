using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UniRx;

public class InventoryButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Image X_Image;     //건물 회수 버튼

    Building this_building;         //이 버튼에 해당하는 건물
    public Building temp_building
    {
        get { return this_building; }
        set { this_building = value.DeepCopy(); }
    }
    public Card this_nuni;         //이 버튼에 해당하는 누니


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

      

        settigPanel = GameObject.FindGameObjectWithTag("SettingPanel");
    }

  
    
}
