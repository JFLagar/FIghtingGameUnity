using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class RebindButton : Selectable, IPointerClickHandler, ISubmitHandler
{
    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
    }

    [SerializeField]
    private RebindActionUI actionUI;
    PlayerController controller;
    public void Initialize(PlayerController playerController)
    {
        controller = playerController;
    }
    public override void OnSelect(BaseEventData eventData)
    {
        controller._clearInput += ClearInput;
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        controller._clearInput -= ClearInput;
        base.OnDeselect(eventData);
    }

    private void ClearInput()
    {
        actionUI.ClearBinding();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Press();
    }
}
