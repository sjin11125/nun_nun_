using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NuniUpgradeButton : MonoBehaviour
{
    UpgradeScript UpgradeManager;
    Card nuni;
    GameObject gauge;
    public void NuniButtonClick()       //���� ��ư Ŭ���ϸ�
    {
        UpgradeManager.isSelect = true;             //���� ���õ�
        Transform[] UpgradeManagerchild = UpgradeManager.GaugePannel.GetComponentsInChildren<Transform>();
        
        //������ �ʱ�ȭ
        Debug.Log(UpgradeManagerchild.Length);
        for (int i = 2; i <= UpgradeManagerchild.Length; i++)
        {
            Destroy(UpgradeManagerchild[i-1].gameObject);
        }


        UpgradeManager.NuniImage.sprite = nuni.GetChaImange();
        UpgradeManager.NuniText.text = nuni.cardName;
        UpgradeManager.NuniLevel.text = "���� ����: " + nuni.Level;
        UpgradeManager.Level1.text = "���� 1: "+nuni.Effect;
        UpgradeManager.Level2.text = "���� 2: "+nuni.Effect;
        UpgradeManager.Level3.text = "���� 3: "+nuni.Effect;

        UpgradeManager.SelectedNuni = nuni;
        //������ �ݿ�
        Gauge();
    }
    // Start is called before the first frame update
    void Start()
    {
        UpgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeScript>();
        nuni = gameObject.GetComponent<Card>();
    }
    public void Gauge()
    {
        for (int i = 0; i < int.Parse(nuni.Gauge); i++)
        {
            gauge = Instantiate(UpgradeManager.Gauge) as GameObject;
            gauge.transform.SetParent(UpgradeManager.GaugePannel.transform);

            gauge.transform.localPosition = new Vector3(0, -230 + 46 * i, 0);

            if (i < 5)
            {
                gauge.GetComponent<Image>().color = new Color(0.1417319f, 0.3301887f, 0.2576322f);
            }
            else
            {
                gauge.GetComponent<Image>().color = new Color(0.4762816f, 0.7264151f, 0.632615f);
            }
        }
    }
    //���׷��̵� ��ư Ŭ��
    /*public void NuniUpgrade()
    {
        if (UpgradeManager.isSelect==true)      //���ϰ� ���õ� ���¿���
        {
            if (int.Parse(UpgradeManager.SelectedNuni.Gauge)>=10)
            {
                for (int i = 0; i < GameManager.Instance.CharacterList.Count; i++)
                {
                    if (GameManager.Instance.CharacterList[i].cardImage== UpgradeManager.SelectedNuni.cardImage)
                    {
                        
                           int level = int.Parse(GameManager.Instance.CharacterList[i].Level);      //���� +1
                        level += 1;
                        GameManager.Instance.CharacterList[i].Level = level.ToString();

                        int gauge = int.Parse(GameManager.Instance.CharacterList[i].Gauge);      //������ �ʱ�ȭ
                        gauge = 0;
                        GameManager.Instance.CharacterList[i].Gauge = gauge.ToString();
                        nuni = GameManager.Instance.CharacterList[i];
                        Transform[] UpgradeManagerchild = UpgradeManager.GaugePannel.GetComponentsInChildren<Transform>();

                        for (int j = 2; j <= UpgradeManagerchild.Length; j++)
                        {
                            Destroy(UpgradeManagerchild[j - 1].gameObject);
                        }

                        UpgradeManager.NuniLevel.text = "���� ����: " + GameManager.Instance.CharacterList[i].Level; //���� �ݿ�
                        return;
                    }
                }
            }
            
        }
    }*/
    // Update is called once per frame
    void Update()
    {
        
    }
}
