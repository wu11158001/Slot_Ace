using UnityEngine;

public class DataManager : UnitySingleton<DataManager>
{
    public string UserId { get; set; }
    // 頭像URL
    public string UserImgUrl { get; set; }
}
