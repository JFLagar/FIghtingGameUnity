using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SettingSelector : Selectable , IValueSelector
{
    [SerializeField]
    private TextMeshProUGUI selectionText;
    private MenuUI parentMenu;
    public int Value { get; private set; }
    private int min = 0;
    private int max = 4;
    public Action<int> _selectorAction;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        parentMenu.SetSelectedSelector(this);
    }

    public void MoveLeft()
    {
        Value--;
        if (Value < min )
            Value = max;
        ProcessSelection();
    }

    public void MoveRight()
    {
        Value++;
        if (Value > max)
            Value = min;
        ProcessSelection();
    }

    private void ProcessSelection()
    {
        if (_selectorAction != null)
            _selectorAction.Invoke(Value);
    }

    public void SetSelectionText(string text)
    {
        selectionText.text = text;
    }

    public void InitializeValues(int initialValue, int minValue, int maxValue, MenuUI menuUI)
    {
        Value = initialValue;
        min = minValue;
        max = maxValue;
        parentMenu = menuUI;
    }

}

public interface IValueSelector
{
    public void MoveLeft();
    public void MoveRight();
}
