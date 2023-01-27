using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveThreeHo : MonoBehaviour
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
                if (hit.collider.gameObject.CompareTag("GridSquare") && useRemove == true)//����� ���õƳ�
                {
                    if (hit.collider.gameObject.transform.GetChild(2).gameObject.activeSelf == true)//�����Ѿְ� ������ �����־�� ����Ұ���
                    {
                        FindLeftRight(hit.collider.gameObject);
                    }
                    if (centerhave == true)
                    {
                        GridScript.ThreeHorizontalItem = 30;
                        myChlid[0].SetActive(false);
                        myChlid[1].SetActive(true);
                        myChlid[2].SetActive(true);
                        myChlid[1].GetComponent<BoxCollider2D>().enabled = false;
                        useRemove = false;
                        settigPanel.GetComponent<AudioController>().Sound[0].Play();
                    }
                }
            }
        }

        number.text = GridScript.ThreeHorizontalItem.ToString();//�׻� ���ڸ� �޴µ�
        if (GridScript.ThreeHorizontalItem <= 0)
        {
            myChlid[2].SetActive(false);//0���ϸ� ��밡��������
            myChlid[1].GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    void FindLeftRight(GameObject center)
    {
        GameObject[] tempObj = new GameObject[25];
        for (int i = 0; i < 25; i++)
        {           
            tempObj[i] = GameObject.FindGameObjectWithTag("Grid").transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < tempObj.Length; i++)
        {
            if (tempObj[i] == center)//�°� ���� ���õ� �ֶ� ������
            {
                clearSquare(tempObj[i]);

                int left = i - 1;
                int right = i + 1;
                if (left > -1)
                {
                    if (left / 5 == i / 5 && tempObj[left].transform.GetChild(2).gameObject.activeSelf == true)
                    {//���� �ٿ��ִ��� ���
                        clearSquare(tempObj[left]);
                    }
                }
                if (right < 26)
                {
                    if (right / 5 == i / 5 && tempObj[right].transform.GetChild(2).gameObject.activeSelf == true)
                    {
                        clearSquare(tempObj[right]);
                    }
                }            
                centerhave = true;
            }
        }
    }

    void clearSquare(GameObject square)//���������� ����
    {
        if (square.transform.GetChild(2).gameObject.activeSelf == true)
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
