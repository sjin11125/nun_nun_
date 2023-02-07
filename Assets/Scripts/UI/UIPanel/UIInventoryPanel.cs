using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.AddressableAssets;

public class UIInventoryPanel : UIBase
{    // Start is called before the first frame update
    // Start is called before the first frame update
    public GameObject inventory_prefab;     //�κ��丮 Ȱ��ȭ�� ��ư ������
    public GameObject inventory_nuni_prefab;     //�κ��丮 ��ư ������
    public Transform Content;
    public GameObject NuniParent;                //���� ������Ʈ �θ�

    public Button InvenBuildingBtn;
    public Button InvenStrBtn;
    public Button InvenNuniBtn;
    public Button InvenCloseBtn;

    GameObject ActiveBuildingPrefab=null;
    InventoryButton ActiveButton=null;
    public UIInventoryPanel(GameObject UIPrefab)
    {
        UIInventoryPanel r = UIPrefab.GetComponent<UIInventoryPanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        r.InstantiatePrefab();
    }

    // Start is called before the first frame update
   public override void Start()
    {
        base.Start();
        if (LoadManager.Instance == null)
            return;
        NuniParent = GameObject.Find("nunis");


        InvenBuildingBtn.OnClickAsObservable().Subscribe(_ =>
        {
            Inventory_Building_Open(false);
        }).AddTo(this);

        InvenStrBtn.OnClickAsObservable().Subscribe(_ =>
        {
            Inventory_Building_Open(true);
        }).AddTo(this);


        InvenNuniBtn.OnClickAsObservable().Subscribe(_ =>
        {
            Inventory_Nuni_Open();
        }).AddTo(this);

        InvenCloseBtn.OnClickAsObservable().Subscribe(_ =>
        {
            // Inventory_Nuni_Open();
            if (ActiveButton != null)            //������ ��ġ���� �ǹ��� �־��ٸ�
            {
                ActiveButton.SetBuildingInfo(LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id]);
                ActiveButton.temp_building.area = LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id].area;
                //  Debug.LogError(ActiveButton.temp_building);
                if (ActiveButton.temp_building.isLock == "F")
                {


                    Destroy(ActiveBuildingPrefab);
                    ActiveButton.temp_building.Type = BuildType.Load;
                    ActiveButton.SetNoImage(false);                  //Xǥ�� �����
                    GridBuildingSystem.OnEditModeOff.OnNext(ActiveButton.temp_building);  //�Ǽ���� ON (Ÿ�� �ʱ�ȭ)
                    if (LoadManager.Instance.MyBuildingsPrefab.ContainsKey(ActiveButton.temp_building.Id))
                        LoadManager.Instance.RemoveBuilding(ActiveButton.temp_building.Id); //�ش� ������ ����
                }
            }
            Inventory_Exit();

            if (GameManager.Instance.isTuto)
            {
                TutorialsManager.IsGoNext = true;
            }

            this.gameObject.transform.parent.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }).AddTo(this);

    }
    public void Inventory_Building_Open(bool isStr)            //�ǹ� �κ� ��ư ������ ��
    {
        Inventory_Exit();           //���� �ִ� ��� �� �����
        foreach (var item in LoadManager.Instance.MyBuildings)
        {
            if (isStr)          //��ġ�� �κ��ΰ�
            {
                if (!item.Value.isStr)          //�ش� �ǹ��� ��ġ���� �ƴ϶�� �׳� �ѱ��
                {
                    continue;
                }
            }
            else                        //�ǹ� �κ��ΰ�
            {
                if (item.Value.isStr)          //�ش� �ǹ��� ��ġ���� �ƴ϶�� �׳� �ѱ��
                {
                    continue;
                }
            }

            if (item.Value.Id != "ii1y1")         //�м��� �ƴϰ� ��ġ���� �ƴ϶��
            {

                Debug.Log(item.Value.Id);
                GameObject inven = Instantiate(inventory_prefab, Content) as GameObject;         //�κ� ��ư ������ ����


                InventoryButton inventoryBtn = inven.GetComponent<InventoryButton>();

                Addressables.LoadAssetAsync<Sprite>(item.Value.Building_Image).Completed += (image) =>
                {            //��巹����� �̹��� �ҷ��� �ֱ�
                    //ButtonImage.sprite = image.Result;
                    inventoryBtn.SetButtonImage(image.Result);   //��ư �̹��� ����
                };
               
                if (LoadManager.Instance.MyBuildings[item.Value.Id].isLock == "T")
                {

                    inventoryBtn.SetNoImage(true);                  //Xǥ�� �Ȼ����
                }
                else
                {
                    inventoryBtn.SetNoImage(false);                  //Xǥ�� �����
                }

                

                Building building = item.Value;
                inventoryBtn.SetBuildingInfo(building);                           //�ش� �ǹ� ���� ���
                //�κ��丮 ��ư�� �ǹ� ���� �־��ֱ�(ex. �ǹ� ���� ��);

                inventoryBtn.temp_building =LoadManager.Instance.MyBuildings[item.Value.Id];

      

                inventoryBtn.button.OnClickAsObservable().Subscribe(_ =>                     //�κ��丮 �ǹ� Ŭ�� ����
                {
                    inventoryBtn.SetBuildingInfo(LoadManager.Instance.MyBuildings[inventoryBtn.temp_building.Id]);
                    if (inventoryBtn.temp_building.isLock == "T")         //�ش� �ǹ��� ��ġ�Ǿ�����
                    {

                        inventoryBtn.temp_building.Type = BuildType.Load;       //BuildType�� Load�� 
                        GridBuildingSystem.OnEditMode.OnNext(inventoryBtn.temp_building);  //�Ǽ���� ON (Ÿ�� �ʱ�ȭ)

                        Destroy(GameManager.Instance.BuildingPrefabData[inventoryBtn.temp_building.Id]); //�ش� �ǹ� ������Ʈ ����

                        inventoryBtn.temp_building.isLock = "F";                            //��ġ�ȵ� ���·� �ٲٱ�

                        inventoryBtn.SetNoImage(false);                             //��ư�� X �̹��� �ֱ�

                        inventoryBtn.temp_building.BuildingPosition.x = 0;                            //��ġ �ʱ�ȭ
                        inventoryBtn.temp_building.BuildingPosition.y = 0;
                        inventoryBtn.temp_building.Placed = false;

                        FirebaseScript.Instance.AddBuilding(inventoryBtn.temp_building.BuildingToJson());            //������ ������ ����
                    }
                    else                               //�ش� �ǹ��� ��ġ�ȵǾ�������
                    {
                        inventoryBtn.temp_building.Type= BuildType.Load;

                        if (ActiveButton != null)           //������ �κ��丮 Ŭ���� ���� �ֳ�?
                        {
                            if (ActiveButton.temp_building.isLock == "F")       //������ ��ġ���� �ǹ��� �־��ٸ�
                            {
                                Destroy(ActiveBuildingPrefab);                  //�ش� �ǹ��� ������Ʈ�� ����

                                ActiveButton.temp_building.Type = BuildType.Load;

                                ActiveButton.SetNoImage(false);                  //Xǥ�� �����

                                GridBuildingSystem.OnEditMode.OnNext(LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id]);  //�Ǽ���� ON (Ÿ�� �ʱ�ȭ)

                            }
                        }

                        LoadManager.Instance.InstantiateBuilding(inventoryBtn.temp_building,out ActiveBuildingPrefab, () =>      //�ǹ� ������ Instantiate�ϰ� �ݹ� ����
                        {
                            
                            GridBuildingSystem.OnEditMode.OnNext(LoadManager.Instance.MyBuildings[inventoryBtn.temp_building.Id]);  //�Ǽ���� ON
                                inventoryBtn.SetNoImage(true);
                                ActiveButton = inventoryBtn;
                                ActiveButton.temp_building.area = LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id].area;

                                ActiveButton.temp_building.Type = BuildType.Move;
                          
                        });



                    }



                }).AddTo(this);


            }

        }
    }
    public void Inventory_Exit()
    {
        Transform[] Content_Child = Content.GetComponentsInChildren<Transform>();
        for (int i = 1; i < Content_Child.Length; i++)
        {
            Destroy(Content_Child[i].gameObject);
        }
    }
    public void Inventory_Nuni_Open()            //���� �κ� ��ư ������ ��
    {
        Inventory_Exit();

        foreach (var item in GameManager.Instance.CharacterList)
        {
            GameObject inven = Instantiate(inventory_nuni_prefab, Content) as GameObject;         //�κ� ��ư ������ ����

            //inven.name = GameManager.Instance.CharacterList[i].cardImage;
            inven.tag = "Inven_Nuni";            //�κ� ��ư �±� ����
            inven.name = item.Value.Id;
            Image ButtonImage = inven.GetComponent<Image>();


            Addressables.LoadAssetAsync<Sprite>(item.Value.cardImage).Completed += (image) =>
            {            //��巹����� �̹��� �ҷ��� �ֱ�
                ButtonImage.sprite = image.Result;

            };

            //ButtonImage.sprite = GameManager.GetCharacterImage(item.Value.cardImage);

            InventoryButton inventoryBtn = inven.GetComponent<InventoryButton>();
            inventoryBtn.this_nuni = item.Value;

            Button Button = inven.GetComponent<Button>();
            Button.OnClickAsObservable().Subscribe(_ => {
                if (inventoryBtn.this_nuni.isLock == "T")         //�ش� ���ϰ� ������
                {
                    LoadManager.Instance.RemoveNuni(inventoryBtn.this_nuni.Id);//�ش� ���� ������Ʈ ����

                    inventoryBtn.this_nuni.isLock = "F";
                    GameManager.Instance.CharacterList[inventoryBtn.this_nuni.Id].isLock = "F";

                    Cardsave nuni = new Cardsave(GameManager.Instance.PlayerUserInfo.Uid, inventoryBtn.this_nuni.cardImage, inventoryBtn.this_nuni.isLock, inventoryBtn.this_nuni.Id);


                    FirebaseScript.Instance.SetNuni(nuni);//������ ����
                    inventoryBtn.SetNoImage(false); //��ư�� xǥ�� ��
                }
                else                                            //�ش� ���ϰ� ������
                {
                    GameObject nunObject = Instantiate(GameManager.CharacterPrefab[inventoryBtn.this_nuni.cardImage], NuniParent.transform);//�ش� ���� ������Ʈ ����
                    LoadManager.Instance.MyNuniPrefab.Add(inventoryBtn.this_nuni.Id, nunObject);
                    inventoryBtn.this_nuni.isLock = "T";
                    GameManager.Instance.CharacterList[inventoryBtn.this_nuni.Id].isLock = "T";

                    Cardsave nuni = new Cardsave(GameManager.Instance.PlayerUserInfo.Uid, inventoryBtn.this_nuni.cardImage, inventoryBtn.this_nuni.isLock, inventoryBtn.this_nuni.Id);
                    FirebaseScript.Instance.SetNuni(nuni);//������ ����
                    inventoryBtn.SetNoImage(true); //��ư�� xǥ�� ����
                }
            }).AddTo(this);
        }
    }
}
