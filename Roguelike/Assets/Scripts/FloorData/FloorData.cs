using UnityEngine;

[CreateAssetMenu(fileName = "FloorData", menuName = "Map/FloorData", order = 1)]
public class FloorData : ScriptableObject
{
    /// <summary>
    /// 階層番号。
    /// </summary>
    public int FloorNumber;

    /// <summary>
    /// 各階層のMassData
    /// </summary>
    public MassData[] MassDataList;

}
