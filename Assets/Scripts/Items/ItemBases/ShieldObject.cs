using Items;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shield Object", menuName = "Inventory System/Items/Shield")]
public class ShieldObject : ItemObject
{
    public void Awake()
    {
        type = ItemType.Shield;
    }
}
