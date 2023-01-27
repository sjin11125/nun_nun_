using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBase : MonoBehaviour
{
    public static CanvasBase _Instance;

    public GameObject TopCanvas;
    public GameObject MiddleCanvas;
    public GameObject BottomCanvas;


    [SerializeField]
    public List<GameObject> UICanvas;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (_Instance == null)
        {
            _Instance = this;
        }
       /* else if (_Instance != this) // �ν��Ͻ��� �����ϴ� ��� ���λ���� �ν��Ͻ��� �����Ѵ�.
        {
            Destroy(gameObject);
        }*/
        //DontDestroyOnLoad(gameObject);  // �Ʒ��� �Լ��� ����Ͽ� ���� ��ȯ�Ǵ��� ����Ǿ��� �ν��Ͻ��� �ı����� �ʴ´�.

    }
    public  GameObject CanvasCheck()
    {
        if (BottomCanvas == null)
        {
            BottomCanvas = Instantiate(UICanvas[0]) as GameObject;
            return BottomCanvas;
        }
        else
        {
            if (!BottomCanvas.activeSelf)           //Bottom Canvas�� Ȱ��ȭ�� �ȵǾ��ֳ�
            {
                BottomCanvas.SetActive(true);
                return BottomCanvas;
            }


            if (MiddleCanvas == null)
            {
                MiddleCanvas = Instantiate(UICanvas[1]) as GameObject;
                return MiddleCanvas;
            }
            else
            {
                if (!MiddleCanvas.activeSelf)
                {
                    MiddleCanvas.SetActive(true);
                    return MiddleCanvas;

                }
                if (TopCanvas = null)
                {
                    TopCanvas = Instantiate(UICanvas[2]) as GameObject;
                    return TopCanvas;
                }
                else
                   if (!TopCanvas.activeSelf)
                {
                    TopCanvas.SetActive(true);
                    return TopCanvas;
                }

            }
        }

        return null;
    }

}
