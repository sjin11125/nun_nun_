using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuniInfoClose : MonoBehaviour
{
    private GameObject settigPanel;
    public GameObject nuniInfo;

    private void Start()
    {
        settigPanel = GameObject.FindGameObjectWithTag("SettingPanel");
    }

    public void CloseOnClick()    //���� ���� �޾Ƽ� ���������� �÷��� �ƴϸ� ��
    {
        if (GameManager.mainMusicOn)
        {
            settigPanel.GetComponent<AudioController>().Sound[0].Play();
            Destroy(nuniInfo);//���������г� ����
        }
    }
}
