using UnityEngine;

[CreateAssetMenu(fileName = "FloorData", menuName = "Map/FloorData", order = 1)]
public class FloorData : ScriptableObject
{
    /// <summary>
    /// 階層番号。
    /// </summary>
    public int FloorNumber;

    /// <summary>
    /// 各階層のMassData。
    /// </summary>
    public MassData[] MassDataList;

    /// <summary>
    /// BGM。
    /// </summary>
    public AudioClip BGM;

    /// <summary>
    /// 環境光の明るさ。
    /// </summary>
    public float DirectionalLightIntensity = 1f;

}
