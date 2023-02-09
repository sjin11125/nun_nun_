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
        if (GameManager.Instance.Money.Value >= 2000)
        {
            rand.GetComponent<RandomSelect>().ResultSelect();

            GameManager.Instance.Money.Value -= 2000;       //2000�� ����
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
