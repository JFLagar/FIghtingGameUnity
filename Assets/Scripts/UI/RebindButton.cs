using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    public void Initialize(PlayerController playerController, int id, int deviceId)
    {
        controller = playerController;
        InputActionReference actionRef = InputActionReference.Create(playerController.GetPlayerInput().actions.FindActionMap("Controls").actions[id]);
        // Find here the action reference
        actionUI.actionReference = actionRef;
        
        actionUI.bindingId = actionRef.action.bindings[deviceId].id.ToString();
    }
    public override void OnSelect(BaseEventData eventData)
    {
        SubscribeToEvent(controller);
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        UnsubscribeToEvent(controller);
        base.OnDeselect(eventData);
    }

    public void SubscribeToEvent(PlayerController playerController)
    {
        playerController._clearInput += ClearInput;
    }

    public void UnsubscribeToEvent(PlayerController playerController)
    {
        playerController._clearInput -= ClearInput;
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
