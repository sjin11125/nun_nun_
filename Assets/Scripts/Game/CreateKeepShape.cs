using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateKeepShape : MonoBehaviour, IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IDropHandler //, IDragHandler
{
    private Vector3 _startPosition;
    private Canvas canvas;
    public bool drop = false;
    private RectTransform rectTransform;

    public string keepColor;
    public string keepShape;
    public Sprite keepSprite;
    public bool keepShin;
    public GameObject hitKeepObj;

    private void Awake()
    {       
        _startPosition = new Vector3(-377f, -660.5f, 0);
        gameObject.tag = "KeepShape";
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
    }

    private void Start()
    {
        this.transform.GetChild(0).gameObject.SetActive(keepShin);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "GridSquare")
        {
            if (collision.gameObject.GetComponent<GridSquare>().SquareOccupied == false)//���������
            {
                hitKeepObj = collision.gameObject;            
                drop = true;
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
        drop = false;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        gameObject.transform.localPosition = _startPosition;

        if (drop == true)//��������
        {
            GridScript.KeepItemTurn = 30;
            hitKeepObj.GetComponent<GridSquare>().UseSquareKeep(keepColor, keepShape, keepSprite);//��� ������Ʈ�� �Ҵ�
            Destroy(this.gameObject);//�� �������� �����ȴ�
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnDrop(PointerEventData eventData)
    {

        
    }
}
