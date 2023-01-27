using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SettingMenu : MonoBehaviour
{


    Button mainButton;
    SettingMenuItem[] menuItems;
    bool isExpanded = false;

    Vector2 mainButtonPosition;
    int itemsCount;

    // Start is called before the first frame update
    void Start()
    {



        itemsCount = transform.childCount - 1;
        menuItems = new SettingMenuItem[itemsCount];
        for (int i = 0; i < itemsCount; i++)
        {
            menuItems[i] = transform.GetChild(i + 1).GetComponent<SettingMenuItem>(); // �̰� �޴��������� 



        }
        mainButton = transform.GetChild(0).GetComponent<Button>();
        mainButton.onClick.AddListener(ToggleMenu);
        mainButton.transform.SetAsLastSibling();
        mainButtonPosition = mainButton.transform.position;

        //�̰� �ϱ����� ����������
        for (int i = 0; i < itemsCount; i++)
        {

            if (gameObject.transform.GetChild(i).gameObject != mainButton)
            {

                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }



        // mainButtonPosition = mainButton.transform.position;

        ResetPositions();
    }

    void ResetPositions()
    {
        for (int i = 0; i < itemsCount; i++)
        {

            menuItems[i].trans.position = mainButtonPosition;
        }
    }

    void ToggleMenu()
    {
        isExpanded = !isExpanded;
        mainButtonPosition = mainButton.transform.position;
        if (isExpanded) // �����ۿ���
        {
            for (int i = 0; i < itemsCount; i++)
            {
                //menuItems[i].trans.position = mainButtonPosition + spacing * (i+1);

                gameObject.transform.GetChild(i).position = new Vector3(mainButtonPosition.x, gameObject.transform.GetChild(i).position.y, gameObject.transform.GetChild(i).position.z);
                
                if (gameObject.transform.GetChild(i).gameObject != mainButton)
                {

                    gameObject.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        else // ������ �ݱ�
        {
            for (int i = 0; i < itemsCount; i++)
            {
                //gameObject.transform.GetChild(i).localPosition = new Vector3(0, gameObject.transform.GetChild(i).localPosition.y, gameObject.transform.GetChild(i).localPosition.z);
               
                // menuItems[i].trans.position = mainButtonPosition;


                Invoke("closed", 0.3f);
            }
        }





    }

    void closed()
    {
        for (int i = 0; i < itemsCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject != mainButton)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }


    void OnDestory()
    {
        Debug.Log("������");
        mainButton.onClick.AddListener(ToggleMenu);
    }
    // Update is called once per frame
    void Update()
    {

    }
}