using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon")]
public class WeaponItem : Item
{
    [SerializeField] private EnumTypes.WP_TYPE wpType;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float damage;
    [SerializeField] private string equipParentTag;
    [SerializeField] private GameObject[] wpPrefabs;

    public EnumTypes.WP_TYPE WpType { get => wpType; set => wpType = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public float Damage { get => damage; set => damage = value; }
    public string EquipParentTag { get => equipParentTag; set => equipParentTag = value; }
    public GameObject[] WpPrefabs { get => wpPrefabs; set => wpPrefabs = value; }
}
