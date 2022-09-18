using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : MonoBehaviour
{
    // 目標：使える技をUIに反映
    // 
    [SerializeField] RectTransform movesParent;
    [SerializeField] SelectableText moveTextPrefab;
    SelectableText[] selectableTexts;

    int selectedIndex;

    public int SelectedIndex { get => selectedIndex; }

    public void Init(List<Move> moves)
    {
        // 自分の子要素で<SelectableText>コンポーネントを持っているものを集める
        selectableTexts = GetComponentsInChildren<SelectableText>();
        SetMovesUISize(moves);
    }

    void SetMovesUISize(List<Move> moves)
    {
        Vector2 uiSize = movesParent.sizeDelta;
        uiSize.y = 50 + 50 * moves.Count;
        movesParent.sizeDelta = uiSize;

        for (int i = 0; i < moves.Count; i++)
        {
            SelectableText moveText = Instantiate(moveTextPrefab, movesParent);
            moveText.SetText(moves[i].Base.Name);
        }
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, selectableTexts.Length - 1);

        for (int i = 0; i < selectableTexts.Length; i++)
        {
            if (selectedIndex == i)
            {
                selectableTexts[i].SetSelectedColor(true);
            }
            else
            {
                selectableTexts[i].SetSelectedColor(false);
            }
        }
    }

    public void Open()
    {
        selectedIndex = 0;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

}
