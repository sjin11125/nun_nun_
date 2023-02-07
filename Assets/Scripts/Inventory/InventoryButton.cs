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


    public Card this_nuni;         //�� ��ư�� �ش��ϴ� ����
    public Building temp_building;        //�� ��ư�� �ش��ϴ� �ǹ�

    public Button button;        //�� ��ũ��Ʈ�� ���� ������Ʈ�� ��ư ������Ʈ

    private GameObject settigPanel;

    public void SetButtonImage(Sprite image)
    {
        this.GetComponent<Image>().sprite = image;
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
