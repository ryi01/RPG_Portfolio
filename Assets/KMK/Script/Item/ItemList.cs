using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "Item/ItemList")]
public class ItemList : ScriptableObject
{
    [SerializeField] private List<Item> list;
    public List<Item> List { get => list; set => list = value; }

}
