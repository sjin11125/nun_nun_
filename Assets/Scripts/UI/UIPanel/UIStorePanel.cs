using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStorePanel : UIBase
{
    // Start is called before the first frame update

    //���� ��ư��

    public UIStorePanel(GameObject UIPrefab)
    {
        UIStorePanel r = UIPrefab.GetComponent<UIStorePanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        r.InstantiatePrefab();
    }

   override public void Start()
    {
        base.Start();

        //���� ��ư�� ����
    }

  
}
