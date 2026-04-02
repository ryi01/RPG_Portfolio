using System;

[Serializable]
public class StoreJoinRow
{
    public int StoreId;
    public string StoreName;
    public int ItemId;
}

[Serializable]
public class ItemTypeJoinRow
{
    public int ItemId;
    public int ItemType;
}
