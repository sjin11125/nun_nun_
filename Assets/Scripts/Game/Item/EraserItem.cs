using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EraserItem : MonoBehaviour
{
    private Image squareImage;
    public Image normalImage;
    public GameObject normalObj;
    public GameObject hooverObj;
    bool buttonDown;
    public Text number;

    private GameObject settigPanel;

    public void StartAndReStart()
    {
        normalObj.SetActive(true);
        hooverObj.SetActive(true);
        settigPanel = GameObject.FindGameObjectWithTag("SettingPanel");
    }

    void Update()
    {
        number.text = GridScript.EraserItemTurn.ToString();
        
        if (Input.GetMouseButtonDown(0) && buttonDown == true)// && hooverObj.activeSelf == false) //��Ŭ�Ҷ�&&��ư ����������
        {
            GridScript.EraserItemTurn = 10;
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);//������ ��
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("GridSquare"))//����� ���õƳ�
                {
                    GameObject contectSquare = hit.collider.gameObject.transform.gameObject; //�θ� �޾�
                    contectSquare.GetComponent<GridSquare>().ClearOccupied(); //��ũ��Ʈ�� ���þȵȿɼ����� �ٲ�
                    contectSquare.GetComponent<GridSquare>().Deactivate();
                    squareImage = contectSquare.transform.GetChild(2).gameObject.GetComponent<Image>();
                    squareImage.sprite = normalImage.sprite;//������� �ٲ�
                    buttonDown = false;
                    settigPanel.GetComponent<AudioController>().Sound[0].Play();
                    GameObject.FindGameObjectWithTag("Grid").GetComponent<GridScript>().CheckIfKeepLineIsCompleted();
                }
            }
        }

        if(buttonDown == true)
        {
            hooverObj.SetActive(true);
            number.text = " ";
        }
        else
        {
            if (GridScript.EraserItemTurn == 10)
            {
                hooverObj.SetActive(true);
            }
            else if(GridScript.EraserItemTurn == 0)
            {
                hooverObj.SetActive(false);
            }
        }
    }

    public void NormalBtnOnClick()
    {
        buttonDown = true;      
    }
}
