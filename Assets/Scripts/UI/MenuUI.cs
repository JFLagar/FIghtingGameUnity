using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem.Samples.RebindUI;

public class MenuUI : MonoBehaviour , IMenuUI
{
    [SerializeField]
    private RectTransform[] uiElements;
    [SerializeField]
    private Selectable[] selectables;

    public virtual void OpenUIElement(int id)
    {
        foreach (RectTransform transform in uiElements)
        {
            transform.gameObject.SetActive(false);
        }
        AudioManager.instance.PlaySoundEffect(1);
        uiElements[id].gameObject.SetActive(true);
        if (selectables[id] != null)
        InputManager.Instance.GetMainPlayerController().SelectUIElement(selectables[id]);
    }

    public virtual void CloseUIElements()
    {
        foreach (RectTransform transform in uiElements)
        {
            transform.gameObject.SetActive(false);
        }
    }
}

public interface IMenuUI
{
    public void OpenUIElement(int id)
    {

    }
}
