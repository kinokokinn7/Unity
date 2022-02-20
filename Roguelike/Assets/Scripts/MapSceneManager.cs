using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapSceneManager : MonoBehaviour
{
    public GameObject GameOver;

    [TextArea(3, 15)]
    public string mapData =
        "000P\n" +
        "0+++\n" +
        "0000\n" +
        "+++0\n" +
        "G000\n";

    void Awake()
    {
        GameOver.SetActive(false);

        var map = GetComponent<Map>();
        var lines = mapData.Split('\n').ToList();
        map.BuildMap(lines);
    }
}
