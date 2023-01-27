using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public GameObject rand;
    private GameObject settigPanel;

    private void Awake()
    {
        settigPanel = GameObject.FindGameObjectWithTag("SettingPanel");
    }

    public void NuniActive()
    {
        if (int.Parse( GameManager.Instance.PlayerUserInfo.Money)>=2000)
        {
            rand.GetComponent<RandomSelect>().ResultSelect();

            int Money = int.Parse(GameManager.Instance.PlayerUserInfo.Money);
            Money -= 2000;       //2000�� ����
            GameManager.Instance.PlayerUserInfo.Money = Money.ToString();
        }
        GameObject.FindGameObjectWithTag("ShopBtn").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("ShopBtn").transform.GetChild(1).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void NuniAnimationEnd()
    {
        //���� �ִϸ��̼� ����
        settigPanel.GetComponent<AudioController>().Sound[2].Play();
    }

    public void EffectEnd29()
    {
        Destroy(this.gameObject);
    }
}
