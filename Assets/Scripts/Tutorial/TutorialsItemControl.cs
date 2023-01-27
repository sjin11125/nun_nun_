using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialsItemControl : MonoBehaviour
{
    public enum ItemType
    {
        touch
    }

    [SerializeField] [Header("�����ϱ� ������ ����")] ItemType itemType;
    [SerializeField] [Header("����� �Է� ������ ����ð�")] float timeToInput;
    [SerializeField] [Header("����� �Է� ���� ǥ���� ���ӿ�����Ʈ")] GameObject gameObjectToShow;

    bool isReadyToInput = false;
    public bool goNext;
    public bool isGame;

    private void OnEnable()
    {
        Invoke("ShowGameObject", timeToInput);
    }

    void Update()
    {
        // �Է´�� ���°� �Ǹ� ��ġ�� �Է� �޴´�.
        if (isReadyToInput)
        {
            if (itemType == ItemType.touch)
            {
                // �Է��� �ϸ� ��� ����
                if (Input.GetMouseButtonDown(0) && goNext)
                {
                    Run();                  
                }
            }
        }
    }

    virtual protected void Run()
    {
        if (gameObjectToShow == null)
            return;

        // ǥ�� item ��Ȱ��ȭ �ϰ�
        gameObjectToShow.SetActive(false);

        // ���� ������ Ȱ��ȭ
        if (isGame)
        {
            GameTutorialsManager parentTutorialsManager = transform.parent.GetComponent<GameTutorialsManager>();
            if (parentTutorialsManager != null)
            {
              //  if (!parentTutorialsManager.isItem)
               // {
                    parentTutorialsManager.ActiveNextItem();
               // }
            }
        }
        else
        {
            TutorialsManager parentTutorialsManager = transform.parent.GetComponent<TutorialsManager>();
            if (parentTutorialsManager != null)
            {
                parentTutorialsManager.ActiveNextItem();
            }
        }
    }

    void ShowGameObject()
    {
        isReadyToInput = true;

        if (gameObjectToShow == null)
            return;

        gameObjectToShow.SetActive(true);
    }

    public void HiRandomOnclick()
    {
        LoadingSceneController.Instance.LoadScene(SceneName.Shop);
        goNext = true;
    }

    public void StoreCloseOnclick()
    {
        goNext = true;
    }

    public void GameItem()
    {
        goNext = true;
    }
}
