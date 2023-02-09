using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialsManager : MonoBehaviour
{
    [SerializeField] 
    [Header("Tutorials items")] 
    TutorialsItemControl[] items;
    public GameObject bunsu;
    public static bool IsGoNext = false;
    
    void Start()
    {
        if (items==null)
            return;

        if (items.Length.Equals( 0))
            return;

        foreach (var item in items)
        {
            item.gameObject.SetActive(false);
        }
        ActiveNextItem();
    }

    private void Update()
    {
        if (GameManager.Instance.PlayerUserInfo.Tuto < 4)
        {
            if (bunsu != null)
            {
                bunsu.gameObject.SetActive(false);
            }
        }
        else
        {
            if (bunsu != null)
            {
                bunsu.gameObject.SetActive(true);
            }
        }
    }

    public void ActiveNextItem()
    {
        if (items.Length .Equals( GameManager.Instance.PlayerUserInfo.Tuto))
        {
            RandomSelect.isTuto = 1;
            PlayerPrefs.SetInt("TutorialsDone", GameManager.Instance.PlayerUserInfo.Tuto);
        }
        else
        {
            if (GameManager.Instance.PlayerUserInfo.Tuto - 1 > -1 && GameManager.Instance.PlayerUserInfo.Tuto - 1 < items.Length)
            {
                items[GameManager.Instance.PlayerUserInfo.Tuto - 1].gameObject.SetActive(false);// �� ������ ��Ȱ��ȭ
            }
            if (GameManager.Instance.PlayerUserInfo.Tuto > -1 && GameManager.Instance.PlayerUserInfo.Tuto < items.Length)
            {
                items[GameManager.Instance.PlayerUserInfo.Tuto].gameObject.SetActive(true);// ������ Ȱ��ȭ

                if (GameManager.Instance.PlayerUserInfo.Tuto.Equals(1))
                {
                    bunsu = GameObject.FindWithTag("bunsu"); //ii1y1
                    if (bunsu != null)
                    {
                        bunsu.SetActive(false);
                    }
                }
                if (GameManager.Instance.PlayerUserInfo.Tuto.Equals(10))
                {
                    
                    if (GameManager.Instance.Money.Value < 100 
                        || GameManager.Instance.ShinMoney.Value < 1)
                    {
                        GameManager.Instance.Money.Value += 100;

                        GameManager.Instance.ShinMoney.Value += 1;

                    }
                }
            }
            PlayerPrefs.SetInt("TutorialsDone", GameManager.Instance.PlayerUserInfo.Tuto);
            GameManager.Instance.PlayerUserInfo.Tuto++;
        }
    }
}
