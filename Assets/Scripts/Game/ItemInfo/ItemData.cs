using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
[System.Serializable]
public class ItemData : ScriptableObject
{
    // Start is called before the first frame update
    public int ItemNumber;
    public Button ItemBuntton;
    public Image ItemImage;
    public string ItemInfo;
}
