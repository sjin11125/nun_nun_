using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeStorage : MonoBehaviour
{
    int shapeIndex;
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;

    private Image spriteImage;
    private Image nextSquare;
    public Image exchangeSquare;

    public string shapeColor;
    public string shapeShape;
    public string[] shapeAchieveId;
    public int currentIndexSave;

    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }

    void Start()
    {
        foreach (var shape in shapeList)
        {
            int firstIndex = UnityEngine.Random.Range(0, shapeData.Count);//ù��° ������ �ε���
            currentIndexSave = firstIndex;//nextExchangeItem�� �Ͼ�������Ƿ� ����
            shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);//�ؽ�Ʈ ������ �ε���
            shape.CreateShape(shapeData[firstIndex]);

            GameObject contectShape = GameObject.FindGameObjectWithTag("Shape");
            if (contectShape != null)
            {
                GameObject squareImage = contectShape.transform.GetChild(0).gameObject;
                spriteImage = squareImage.GetComponent<Image>();
                squareImage.transform.GetChild(0).gameObject.GetComponent<Shining>().ShinActive();//��¦�� �ѱ�
            }
            GameObject contectNext = GameObject.FindGameObjectWithTag("NextSquare");
            if (contectNext != null)
            {
                nextSquare = contectNext.GetComponent<Image>();
            }
            spriteImage.sprite = shapeData[firstIndex].sprite;
            shapeColor = shapeData[firstIndex].color;
            shapeShape = shapeData[firstIndex].shape;//ù�Ͽ� ù��° ���� ������ ����ִ�
            shapeAchieveId = shapeData[firstIndex].AchieveId;

            nextSquare.sprite = shapeData[shapeIndex].sprite;
        }
    }

    public Shape GetCurrentSelectedShape()//�׸��彺ũ��Ʈ���� �����
    {
        foreach (var shape in shapeList)
        {
            if (shape.IsOnStartPosition() == false && shape.IsAnyOfShapeSquareActive())
                return shape;//�����̰��ִ� ������ ����
        }
        Debug.LogError("There is no shape selected");
        return null;
    }

    private void RequestNewShapes()//�ι�° �Ϻ��� ��� ���⼭ �����
    {
        foreach (var shape in shapeList)
        {
            shapeColor = shapeData[shapeIndex].color;//start���� ���� ������ ����
            shapeShape = shapeData[shapeIndex].shape;
            shapeAchieveId = shapeData[shapeIndex].AchieveId;

            currentIndexSave = shapeIndex;
            shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);//���������� �ؽ�Ʈ ���������� �������
            shape.RequestNewShape(shapeData[shapeIndex]);

            GameObject contectShape = GameObject.FindGameObjectWithTag("Shape");
            if (contectShape != null)
            {
                GameObject squareImage = contectShape.transform.GetChild(0).gameObject;
                spriteImage = squareImage.GetComponent<Image>();
                squareImage.transform.GetChild(0).gameObject.GetComponent<Shining>().ShinActive();//��¦�� �ѱ�
            }
            GameObject contectNext = GameObject.FindGameObjectWithTag("NextSquare");
            if (contectNext != null)
            {
                nextSquare = contectNext.GetComponent<Image>();
            }
            spriteImage.sprite = nextSquare.sprite;
            nextSquare.sprite = shapeData[shapeIndex].sprite;
        }
    }

    public void nextExchangeItem()//����� ���ĸ� ���� �ڸ���ü
    {
        shapeColor = shapeData[shapeIndex].color;//shapeIndex�� RequestNewShapes���� �̹� ���� ������ ��������� ����
        shapeShape = shapeData[shapeIndex].shape;
        shapeAchieveId = shapeData[shapeIndex].AchieveId;

        exchangeSquare.sprite = spriteImage.sprite;//��������Ʈ ��ü
        spriteImage.sprite = nextSquare.sprite;
        nextSquare.sprite = exchangeSquare.sprite;

        int exchangeIndex = currentIndexSave;//�ε��� ��ü
        currentIndexSave = shapeIndex;
        shapeIndex = exchangeIndex;
    }

    public void ReloadItem()
    {
        foreach (var shape in shapeList)
        {
            shapeColor = shapeData[shapeIndex].color;
            shapeShape = shapeData[shapeIndex].shape;
            shapeAchieveId = shapeData[shapeIndex].AchieveId;

            shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.RequestNewShape(shapeData[shapeIndex]);

            GameObject contectShape = GameObject.FindGameObjectWithTag("Shape");
            if (contectShape != null)
            {
                GameObject squareImage = contectShape.transform.GetChild(0).gameObject;
                spriteImage = squareImage.GetComponent<Image>();
                squareImage.transform.GetChild(0).gameObject.GetComponent<Shining>().ShinActive();//��¦�� �ѱ�
            }
            GameObject contectNext = GameObject.FindGameObjectWithTag("NextSquare");
            if (contectNext != null)
            {
                nextSquare = contectNext.GetComponent<Image>();
            }
            spriteImage.sprite = nextSquare.sprite;
            nextSquare.sprite = shapeData[shapeIndex].sprite;
        }
    }
}
