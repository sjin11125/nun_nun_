using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.AddressableAssets;

public class InventoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject inventory_prefab;     //�κ��丮 Ȱ��ȭ�� ��ư ������
    public GameObject inventory_nuni_prefab;     //�κ��丮 ��ư ������
    public Transform Content;
    public GameObject NuniParent;                //���� ������Ʈ �θ�

    public Button InvenBuildingBtn;
    public Button InvenStrBtn;
    public Button InvenNuniBtn;
    public Button InvenCloseBtn;

    GameObject ActiveBuildingPrefab;
    InventoryButton ActiveButton;
    void Start()
    {
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
        }).AddTo(this);


    }

    public void Inventory_Exit()
    {
        Transform[] Content_Child = Content.GetComponentsInChildren<Transform>();
        for (int i = 1; i < Content_Child.Length; i++)
        {
            Destroy(Content_Child[i].gameObject);
        }
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

                if (item.Value.Id != "ii1y1" )         //�м��� �ƴϰ� ��ġ���� �ƴ϶��
                {

                    Debug.Log(item.Value.Id);
                    GameObject inven = Instantiate(inventory_prefab, Content) as GameObject;         //�κ� ��ư ������ ����


                    InventoryButton inventoryBtn = inven.GetComponent<InventoryButton>();
                Addressables.LoadAssetAsync<Sprite>(item.Value.Building_Image).Completed += (image) =>
                {            //��巹����� �̹��� �ҷ��� �ֱ�
                    inventoryBtn.SetButtonImage(image.Result);

                };
                //inventoryBtn.SetButtonImage(GameManager.GetDogamChaImage(item.Value.Building_Image));   //��ư �̹��� ����
                if (LoadManager.Instance.MyBuildings[item.Value.Id].isLock=="T")
                {

                    inventoryBtn.SetNoImage(true);                  //Xǥ�� �Ȼ����
                }
                else
                {
                    inventoryBtn.SetNoImage(false);                  //Xǥ�� �����
                }

                inventoryBtn.SetBuildingInfo(LoadManager.Instance.MyBuildings[item.Value.Id]);

                Building building = item.Value;
                    inventoryBtn.SetBuildingInfo(building);                           //�ش� �ǹ� ���� ���
                    inventoryBtn.temp_building = item.Value;

                    Button Button = inven.GetComponent<Button>();



                    Button.OnClickAsObservable().Subscribe(_ =>                     //�κ��丮 �ǹ� Ŭ�� ����
                    {
                        inventoryBtn.SetBuildingInfo(LoadManager.Instance.MyBuildings[inventoryBtn.temp_building.Id]);
                        if (inventoryBtn.temp_building.isLock == "T")         //�ش� �ǹ��� ��ġ�Ǿ�����
                    {

                            inventoryBtn.temp_building.Type = BuildType.Load;
                            GridBuildingSystem.OnEditMode.OnNext(inventoryBtn.temp_building);  //�Ǽ���� ON (Ÿ�� �ʱ�ȭ)
                        LoadManager.Instance.RemoveBuilding(inventoryBtn.temp_building.Id); //�ش� ������ ����

                        inventoryBtn.temp_building.isLock = "F";                            //��ġ�ȵ� ���·� �ٲٱ�

                        inventoryBtn.SetNoImage(false);

                            inventoryBtn.temp_building.BuildingPosition.x = 0;                            //��ġ �ʱ�ȭ
                        inventoryBtn.temp_building.BuildingPosition.y = 0;
                            inventoryBtn.temp_building.Placed = false;
                            // LoadManager.Instance.buildingsave.BuildingReq(BuildingDef.updateValue, inventoryBtn.temp_building);     //������ ����
                            FirebaseLogin.Instance.AddBuilding(inventoryBtn.temp_building.BuildingToJson());            //������ ����
                        }
                        else                               //�ش� �ǹ��� ��ġ�ȵǾ�������
                    {
                            Building ActiveBuilding = new Building();

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
                                GridBuildingSystem.OnEditMode.OnNext(ActiveButton.temp_building);  //�Ǽ���� ON (Ÿ�� �ʱ�ȭ)
                                if (LoadManager.Instance.MyBuildingsPrefab.ContainsKey(ActiveButton.temp_building.Id))
                                        LoadManager.Instance.RemoveBuilding(ActiveButton.temp_building.Id); //�ش� ������ ����
                            }
                            }
                            try
                            {
                                LoadManager.Instance.InstantiateBuilding(inventoryBtn.temp_building,()=> { 
                                
                               
                                ActiveBuilding =LoadManager.Instance.MyBuildingsPrefab[inventoryBtn.temp_building.Id].GetComponent<Building>();


                                ActiveBuildingPrefab = ActiveBuilding.gameObject;

                                ActiveButton = inventoryBtn;
                                ActiveButton.temp_building.area = LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id].area;

                                Debug.Log(ActiveBuildingPrefab);
                                ActiveBuilding.Type = BuildType.Move;

                                GridBuildingSystem.OnEditMode.OnNext(ActiveBuilding);  //�Ǽ���� ON
                            inventoryBtn.SetNoImage(true);
                                });
                                /* ActiveButton.temp_building.BuildEditBtn[1].btn.OnClickAsObservable().Subscribe(_=>
                                 {
                                     ActiveButton.temp_building.isLock = "T";
                                 }).AddTo(this);*/
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError(e.Message);
                                throw;
                            }

                        }



                    }).AddTo(this);


                }
            
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

           // ButtonImage.sprite = GameManager.GetCharacterImage(item.Value.cardImage);

            InventoryButton inventoryBtn = inven.GetComponent<InventoryButton>();
            inventoryBtn.this_nuni = item.Value;

            Button Button = inven.GetComponent<Button>();
            Button.OnClickAsObservable().Subscribe(_=> {
                if (inventoryBtn.this_nuni.isLock=="T")         //�ش� ���ϰ� ������
                {
                    LoadManager.Instance.RemoveNuni(inventoryBtn.this_nuni.Id);//�ش� ���� ������Ʈ ����

                    inventoryBtn.this_nuni.isLock = "F";
                    GameManager.Instance.CharacterList[inventoryBtn.this_nuni.Id].isLock = "F";

                    Cardsave nuni = new Cardsave(GameManager.Instance.PlayerUserInfo.Uid, inventoryBtn.this_nuni.cardImage, inventoryBtn.this_nuni.isLock, inventoryBtn.this_nuni.Id);
                   

                    FirebaseLogin.Instance.SetNuni(nuni);//������ ����
                    inventoryBtn.SetNoImage(false); //��ư�� xǥ�� ��
                }
                else                                            //�ش� ���ϰ� ������
                {
                    GameObject nunObject = Instantiate(GameManager.CharacterPrefab[inventoryBtn.this_nuni.cardImage], NuniParent.transform);//�ش� ���� ������Ʈ ����
                    LoadManager.Instance.MyNuniPrefab.Add(inventoryBtn.this_nuni.Id, nunObject);
                    inventoryBtn.this_nuni.isLock = "T";
                    GameManager.Instance.CharacterList[inventoryBtn.this_nuni.Id].isLock = "T";

                    Cardsave nuni = new Cardsave(GameManager.Instance.PlayerUserInfo.Uid, inventoryBtn.this_nuni.cardImage, inventoryBtn.this_nuni.isLock, inventoryBtn.this_nuni.Id);
                    FirebaseLogin.Instance.SetNuni(nuni);//������ ����
                    inventoryBtn.SetNoImage(true); //��ư�� xǥ�� ����
                }
            }).AddTo(this);
        }
    }
}
