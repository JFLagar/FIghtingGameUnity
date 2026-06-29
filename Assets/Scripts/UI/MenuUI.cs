using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem.Samples.RebindUI;
using Unity.VisualScripting;

public class MenuUI : MonoBehaviour , IMenuUI
{
    [SerializeField]
    private RectTransform[] uiElements;
    [SerializeField]
    private Selectable[] selectables;

    private Selectable previousSelected;

    public virtual void OpenUIElement(int id)
    {
        foreach (RectTransform transform in uiElements)
        {
            transform.gameObject.SetActive(false);
        }
        previousSelected = null;
        AudioManager.instance.PlaySoundEffect(1);
        uiElements[id].gameObject.SetActive(true);
        if (id <= selectables.Length - 1 && selectables[id] != null)
        InputManager.Instance.GetMainPlayerController().SelectUIElement(selectables[id]);
    }

    public virtual void CloseUIElements(Selectable selectable)
    {
        foreach (RectTransform transform in uiElements)
        {
            transform.gameObject.SetActive(false);
        }
        selectable.Select();
    }
}

public interface IMenuUI
{
    public void OpenUIElement(int id)
    {

    }
}

