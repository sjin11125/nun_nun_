using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class DontDestroy : MonoBehaviour
{
    public static bool isStart = false;
    static DontDestroy _Instance;
    static Transform[] trans;

   public static TileBase[] Area;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            isStart = true;
        }
        else if (_Instance != this) // �ν��Ͻ��� �����ϴ� ��� ���λ���� �ν��Ͻ��� �����Ѵ�.
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);  // �Ʒ��� �Լ��� ����Ͽ� ���� ��ȯ�Ǵ��� ����Ǿ��� �ν��Ͻ��� �ı����� �ʴ´�.

    }
    // Update is called once per frame
    void Update()
    {
        trans = gameObject.GetComponentsInChildren<Transform>();

        if (trans!=null)
        {
            
        }
        
        if (SceneManager.GetActiveScene().name != "Main")
        {
            if (transform.childCount != 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        
        else if (SceneManager.GetActiveScene().name == "Main")
        {

            if (transform.childCount != 0) 
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            
        }
    }
    
}
