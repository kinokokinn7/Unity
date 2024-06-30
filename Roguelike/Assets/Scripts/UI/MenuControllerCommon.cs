using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControllerCommon : MonoBehaviour, IMenuController
{
    public void ExecuteSelection()
    {
    }

    public void ShowMenu()
    {
        // 効果音を鳴らす
        SoundEffectManager.Instance.PlayOpenWindowSound();
    }

    public void HideMenu()
    {
    }

    public void MoveSelectionDown()
    {
    }

    public void MoveSelectionUp()
    {
    }

}
