using System.Collections.Generic;
using UnityEngine;

public class RoundsContainer : MonoBehaviour
{
    [SerializeField]
    private RoundDisplay prefab;
    [SerializeField]
    private List<RoundDisplay> roundList = new List<RoundDisplay>();

    public void CreateRounds(int number)
    {
        for (int i = 0; i < number; i++)
        {
            RoundDisplay round = Instantiate(prefab, this.gameObject.transform);
            roundList.Add(round);
        }
    }

    public void ResetRounds()
    {
        foreach (var round in roundList)
            round.ToggleIcon(false);
    }

    public void UpdateRounds(int wonRounds)
    {
        for (int rounds = 0; rounds < wonRounds; rounds++)
        {
            if (wonRounds > roundList.Count)
            {
                return;
            }
            roundList[rounds].enabled = true;
        }
    }
}
