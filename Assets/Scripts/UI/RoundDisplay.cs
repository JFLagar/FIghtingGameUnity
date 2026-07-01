using UnityEngine;
using UnityEngine.UI;

public class RoundDisplay : MonoBehaviour
{
    [SerializeField]
    private Image roundIcon;

    public void ToggleIcon(bool toggle)
    {
        roundIcon.enabled = toggle;
    }
}
