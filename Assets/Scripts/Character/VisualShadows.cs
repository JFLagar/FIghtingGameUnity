using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.CharacterSpace;
public class VisualShadows : MonoBehaviour
{
    public Character character;
    public Vector3 originalScale;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(character.transform.position.x, -0.5f);
        transform.localScale = new Vector3(originalScale.x / (character.transform.position.y + 1), originalScale.y / (character.transform.position.y + 1), originalScale.z);
    }
}
