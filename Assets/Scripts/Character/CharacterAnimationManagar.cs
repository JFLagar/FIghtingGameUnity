using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimType
{
    Hit,
    Attack,
    Landing,
    Movement
}
public class CharacterAnimationManagar : MonoBehaviour
{
    public string[] animNames;
    public Animator animator;
    IDictionary<AnimType, string> animations = new Dictionary<AnimType, string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(animations.Count != 0)
        {
            animations.TryGetValue(AnimType.Hit, out animNames[0]);
            animations.TryGetValue(AnimType.Attack, out animNames[2]);
            animations.TryGetValue(AnimType.Landing, out animNames[1]);
            animations.TryGetValue(AnimType.Movement, out animNames[3]);
            PlayAnimation(animNames);

        }
    }
    public void PlayAnimation(string[] m_animations)
    {
        foreach(string animationName in m_animations)
        {
            if (animationName != null)
            {               
                animator.Play(animationName, 0, 0f);
                    ClearAnimations();
            }
        }
    }
    public void ClearAnimations()
    {
        animations.Clear();
    }
    public void AddAnimation(AnimType type, string animName)
    {
        animations.TryAdd(type, animName);
    }
}
