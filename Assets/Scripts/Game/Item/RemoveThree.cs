using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveThree : MonoBehaviour
{
    GameObject[] myChlid;
    private Image squareImage;
    public Image normalImage;
    bool useRemove;
    bool centerhave;
    public Text number;

    private GameObject settigPanel;

    public void StartAndReStart()
    {
        myChlid = new GameObject[3];
        for (int i = 0; i < myChlid.Length; i++)
        {
            myChlid[i] = gameObject.transform.GetChild(i).gameObject;
        }
        settigPanel = GameObject.FindGameObjectWithTag("SettingPanel");
        myChlid[0].SetActive(false);
        myChlid[1].SetActive(true);
        myChlid[2].SetActive(true);
        myChlid[1].GetComponent<BoxCollider2D>().enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);//������ ��
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("GridSquare") && useRemove == true)//ä�����ִ¾ְ� ������
                {
                    if(hit.collider.gameObject.transform.GetChild(2).gameObject.activeSelf == true)//�����Ѿְ� ������ �����־�� ����Ұ���
                    {
                        FindUpDown(hit.collider.gameObject);
                    }
                    if(centerhave == true)
                    {
                        GridScript.ThreeVerticalItem = 30;
                        myChlid[0].SetActive(false);
                        myChlid[1].SetActive(true);
                        myChlid[2].SetActive(true);
                        myChlid[1].GetComponent<BoxCollider2D>().enabled = false;
                        useRemove = false;
                        settigPanel.GetComponent<AudioController>().Sound[0].Play();
                    }
                }
            }
        }//�ڽ� 2�� Ʈ��� 1�� �ڽ��ݶ��̴� ����

        number.text = GridScript.ThreeVerticalItem.ToString();//�׻� ���ڸ� �޴µ�
        if (GridScript.ThreeVerticalItem <= 0)
        {
            myChlid[2].SetActive(false);//0���ϸ� ��밡��������
            myChlid[1].GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    void FindUpDown(GameObject center)
    {
        GameObject[] tempObj = new GameObject[25];
        for (int i = 0; i < 25; i++)
        {
           // tempObj[i] = center.GetComponentInParent<GridScript>().transform.GetChild(i).gameObject;//�׸��� ��ũ��Ʈ �ڽĵ� ��� ����
            tempObj[i] = GameObject.FindGameObjectWithTag("Grid").transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < tempObj.Length; i++)
        {
            if (tempObj[i] == center)//�°� ���� ���õ� �ֶ� ������
            {
                clearSquare(tempObj[i]);

                int up = i - 5;
                int down = i + 5;
                if (up > -1 && tempObj[up].transform.GetChild(2).gameObject.activeSelf == true)
                {
                    clearSquare(tempObj[up]);
                }
                if(down <25 && tempObj[down].transform.GetChild(2).gameObject.activeSelf == true)
                {
                    clearSquare(tempObj[down]);
                }
                centerhave = true;
            }
        }
      
    }

    void clearSquare(GameObject square)//���������� ����
    {
        if(square.transform.GetChild(2).gameObject.activeSelf == true)
        {
            square.GetComponent<GridSquare>().ClearOccupied();
            square.GetComponent<GridSquare>().Deactivate();
            squareImage = square.transform.GetChild(2).gameObject.GetComponent<Image>();
            squareImage.sprite = normalImage.sprite;
        }
    }

    public void OnClickChild1()
    {
        myChlid[0].SetActive(true);
        myChlid[1].SetActive(false);
        useRemove = true;
        centerhave = false;
    }
}
