using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class GridSquare : MonoBehaviour
{
    public Image hooverImage;
    public Image activeImage;
    public Image normalImage;
    public List<Sprite> normalImages;

    [HideInInspector]
    public Image spriteImage;

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }

    public string currentColor;
    public string currentShape;
    public string[] currentAchieveId;
    public bool UseKeepBool;

    private float clickTime;
    private float minClickTime = 0.8f;
    private bool isClick;
    GameObject rainbowObj;
    GameObject ChangeShapeObj;
    GameObject squareImage;

    public bool shinActive;

    public Sprite trashAndKeep;
    public bool IMtrash = false;

    public GameObject KeepShapeObj;
    public bool IMkeep = false;
    Sprite currentSprite;
    bool currentShin;

    private GameObject settigPanel;

    void Awake()
    {
        Selected = false;
        SquareOccupied = false;
        currentColor = null;
        currentShape = null;
        currentAchieveId = new string[2] { null,null};
        shinActive = false;
        UseKeepBool = false;

        GameObject contectShape = GameObject.FindGameObjectWithTag("Shape");
        if (contectShape != null)
        {
            squareImage = contectShape.transform.GetChild(0).gameObject;
            spriteImage = squareImage.GetComponent<Image>();
        }
        GameObject GetRainbow = GameObject.FindGameObjectWithTag("ItemController");//��Ʈ�ѷ� �ټ���° �ڽ���
        if(GetRainbow != null)
        {
            rainbowObj = GetRainbow.transform.GetChild(4).gameObject;//���κ��� ������ ������Ʈ�� �޾�
            ChangeShapeObj = GetRainbow.transform.GetChild(5).gameObject;
        }
        settigPanel = GameObject.FindGameObjectWithTag("SettingPanel");
    }

    
    private void FixedUpdate()
    {
        if (UseKeepBool)
        {
            GameObject.FindGameObjectWithTag("Grid").GetComponent<GridScript>().CheckIfKeepLineIsCompleted();
            UseKeepBool = false;
        }
    }
    
    private void Update()
    {
        Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(wp, Vector2.zero);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (Input.GetMouseButtonDown(0))
        {
            isClick = true;
            if (gameObject.transform.GetChild(2).gameObject.activeSelf)//�������� ���Ŀ� ��Ŭ��
            {
                if (clickTime >= minClickTime)//��Ŭ���̾�����
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            if(GridScript.RainbowItemTurn <= 0 && RainbowItem.rainbowActive)
                            {
                                rainbowObj.GetComponent<RainbowItem>().RainbowItemUse(currentShape);//���κ��� ������ �Լ� ȣ��
                                RainbowItem.squareColorObj = this.gameObject;
                            }
                            else if (GridScript.ChangeShapeItem <= 0 && ChangeShapeItem.changeActive)
                            {
                                ChangeShapeObj.GetComponent<ChangeShapeItem>().RainbowItemUse(currentColor);//�÷��ٲٴ� ������ �Լ� ȣ��
                                ChangeShapeItem.squareObj = this.gameObject;
                            }
                        }
                    }
                }
            }
            clickTime = 0;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isClick = false;               
        }

        if (isClick)
        {
            clickTime += Time.deltaTime;
        }
    }

    public void PlaceShapeOnBoard()//�׸��� ��ũ��Ʈ CheckIfShapeCanBePlaced���� ���
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        settigPanel.GetComponent<AudioController>().Sound[2].Play();
        hooverImage.gameObject.SetActive(false);//���õǰ��ִ��߿��ߴ� ���ѻ�����
        activeImage.gameObject.SetActive(true);//���õ� �� �ѱ�

        if (squareImage.transform.GetChild(0).gameObject.activeSelf && !IMtrash && !IMkeep)//shin�� ����������
        {
            activeImage.transform.GetChild(0).gameObject.SetActive(true);
            shinActive = true;
        }

        Selected = true; //���õ�
        SquareOccupied = true; //�����
        
        if (activeImage.gameObject.activeSelf == true)
        {
            if (IMtrash)
            {
                activeImage.sprite = trashAndKeep;
                if (GridScript.TrashItemTurn < 1)
                {
                    GridScript.TrashItemTurn = 20;
                }
            }
            else if (IMkeep)
            {
                activeImage.sprite = trashAndKeep;
                if (GridScript.KeepItemTurn < 1)
                {
                    GameObject keepInstance = Instantiate(KeepShapeObj, this.transform.parent);
                    keepInstance.transform.localPosition = new Vector3(-377f, -660.5f, 0);
                    keepInstance.GetComponent<CreateKeepShape>().keepColor = currentColor;
                    keepInstance.GetComponent<CreateKeepShape>().keepShape = currentShape;
                    keepInstance.GetComponent<CreateKeepShape>().keepSprite = spriteImage.sprite;
                    keepInstance.GetComponent<Image>().sprite = spriteImage.sprite;
                    keepInstance.GetComponent<CreateKeepShape>().keepShin = currentShin;
                    NonKeep();
                }
            }
            else
            {
                if (UseKeepBool)
                {
                    activeImage.sprite = currentSprite;                   
                }
                else
                {
                    activeImage.sprite = spriteImage.sprite;//������ ��������Ʈ ����
                }
            }
        }
    }

    public void Deactivate()
    {
        activeImage.gameObject.SetActive(false);
        activeImage.sprite = null;//��������� ���� �ȴ����س��� �̰Ż�Ǿ���ɵ�?
        currentColor = null;
        currentShape = null;
        currentAchieveId = new string[2] { null,null};

        activeImage.transform.GetChild(0).gameObject.SetActive(false);
        shinActive = false;
    }

    public void NonKeep()//keep ���� ������ �ֵ鿡 ���
    {
        gameObject.SetActive(false);
    }

    public void ClearOccupied()
    {
        Selected = false;
        SquareOccupied = false;
    }

    public void SetImage(bool setFirstImage)
    {
        normalImage.sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)//�浹ó��
    {
        if (SquareOccupied == false)//������� �ƴϸ�
        {
            Selected = true;//���õȰɷιٲ�
            hooverImage.gameObject.SetActive(true);//���ѻ�
            
            GameObject ShapeStorageObj = GameObject.FindGameObjectWithTag("ShapeStorage");
            if (ShapeStorageObj != null)//���⼭ �׻� ���� shape�� ������ �޴´�
            {                             
                currentColor = ShapeStorageObj.GetComponent<ShapeStorage>().shapeColor;
                currentShape = ShapeStorageObj.GetComponent<ShapeStorage>().shapeShape;
                currentShin = collision.transform.GetChild(0).gameObject.activeSelf;
                currentAchieveId = ShapeStorageObj.GetComponent<ShapeStorage>().shapeAchieveId;
            }
        }
        else if(collision.GetComponent<ShapeSquare>() != null)//�������� �������
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();//������ �������Ʈ
        }
    }

    private void OnTriggerStay2D(Collider2D collision)//�浹��
    {
        Selected = true;//���õȰɷιٲ�

        if (SquareOccupied == false)//������� �ƴϸ�
        {            
            hooverImage.gameObject.SetActive(true);
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }       
    }

    private void OnTriggerExit2D(Collider2D collision)//�浹���
    {
        if (SquareOccupied == false)//������� �ƴϸ�
        {
            Selected = false;//���þȵȰɷ���
            hooverImage.gameObject.SetActive(false);//���ѻ� ��
            currentColor = null;
            currentShape = null;
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnSetOccupied();//�������Ʈ��
        }
    }

    public void UseSquareKeep(string color, string shape, Sprite sprite)//ŵ �����հ� ������ ������ �Լ�
    {
        UseKeepBool = true;
        currentColor = color;//ŵ ���� �� Ŀ��Ʈ �÷��� ŵ �÷��� ���
        currentShape = shape;
        currentSprite = sprite;
        ActivateSquare();
    }
}
