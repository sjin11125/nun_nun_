using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject eraserItem, nextSquareItem, nextResetItem, NextExchangeItem, RainbowItem, ChangeShapeItem, ThreeVerticalItem, ThreeHorizontalTtem;
    public int keepItemIndex, trashCanItemIndex;

    public bool[] mainItemBool = new bool[10];
    public GameObject keepshdow;
    public GameObject trashshdow;

    public GameObject GameoverObj;

    public static bool reStart;

    public StartManager startManager;

    private void Awake()
    {
        ItemActive();
        reStart = false;
    }

    public void ItemActive()//������ ����â�� ���� ����
    {      
        
        for (int i = 0; i < mainItemBool.Length; i++)//������ ������ ����
        {
            mainItemBool[i] = startManager.itemList[i];
        }
        SetItem();//������ ����
    }

    void SetItem()
    {
        if (mainItemBool[0] == true)
        {
            GridScript.EraserItemTurn = 10;
            eraserItem.SetActive(true);
            eraserItem.GetComponent<EraserItem>().StartAndReStart();
        }
        else
        {
            eraserItem.SetActive(false);
        }

        if (mainItemBool[1] == true)
        {
            GridScript.KeepItemTurn = 30;
            keepItemIndex = 25;
            keepshdow.SetActive(true);
        }
        else
        {
            keepItemIndex = 30;
        }

        if (mainItemBool[2] == true)
        {
            GridScript.TrashItemTurn = 20;
            trashCanItemIndex = 29;
            trashshdow.SetActive(true);
        }
        else
        {
            trashCanItemIndex = 30;
        }

        if (mainItemBool[3] == true)
        {
            nextSquareItem.SetActive(false);
        }
        else
        {
            nextSquareItem.SetActive(true);
        }

        if (mainItemBool[4] == true)
        {
            GridScript.ReloadItemTurn = 15;
            nextResetItem.SetActive(true);
            nextResetItem.GetComponent<ReloadItem>().StartAndReStart();
        }
        else
        {
            nextResetItem.SetActive(false);
        }

        if (mainItemBool[5] == true)
        {
            GridScript.NextExchangeItemTurn = 15;
            NextExchangeItem.SetActive(true);
            NextExchangeItem.GetComponent<NextExchangeItem>().StartAndReStart();
        }
        else
        {
            NextExchangeItem.SetActive(false);
        }

        if (mainItemBool[6] == true)
        {
            GridScript.RainbowItemTurn = 40;
            RainbowItem.SetActive(true);
            RainbowItem.GetComponent<RainbowItem>().StartAndReStart();
        }
        else
        {
            RainbowItem.SetActive(false);
        }

        if (mainItemBool[7] == true)
        {
            GridScript.ChangeShapeItem = 40;
            ChangeShapeItem.SetActive(true);
            ChangeShapeItem.GetComponent<ChangeShapeItem>().StartAndReStart();
        }
        else
        {
            ChangeShapeItem.SetActive(false);
        }

        if (mainItemBool[8] == true)
        {
            GridScript.ThreeVerticalItem = 30;
            ThreeVerticalItem.SetActive(true);
            ThreeVerticalItem.GetComponent<RemoveThree>().StartAndReStart();
        }
        else
        {
            ThreeVerticalItem.SetActive(false);
        }

        if (mainItemBool[9] == true)
        {
            GridScript.ThreeHorizontalItem = 30;
            ThreeHorizontalTtem.SetActive(true);
            ThreeHorizontalTtem.GetComponent<RemoveThreeHo>().StartAndReStart();
        }
        else
        {
            ThreeHorizontalTtem.SetActive(false);
        }
    }

    public void GameoverReStart()//���ӿ����� �ִ� ����ŸƮ��ư
    {
        GameoverObj.gameObject.SetActive(false);//���ӿ��� �ǳڲ���
        startManager.transform.parent.gameObject.SetActive(true);//������ �ǳ� �ѱ�

        startManager.CharacterOpen();//ĳ���� ������ ���ӸŴ����� ����
        for (int i = 0; i < mainItemBool.Length; i++)//�ʱ�ȭ
        {
            mainItemBool[i] = false;
        }
        reStart = true;
       GameManager.Instance.GameSave();
    }
}
