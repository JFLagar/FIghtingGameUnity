using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.CharacterSpace;
public class VisualShadows : MonoBehaviour
{
    public Player player;
    public Vector3 originalScale;
    public float shadowOffset = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(player.transform.position.x, shadowOffset);
        transform.localScale = new Vector3(originalScale.x / (player.transform.position.y + 1), originalScale.y / (player.transform.position.y + 1), originalScale.z);
    }
}
