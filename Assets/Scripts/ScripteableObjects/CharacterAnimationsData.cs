using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimationsData", menuName = "Scriptable Objects/CharacterAnimationsData")]
public class CharacterAnimationsData : ScriptableObject
{
    //0 Standing and Walking
    public AnimationClip[] standingClips;
    public AnimationClip[] jumpingClips;
    public AnimationClip crouchingClip;
    public AnimationClip[] blockingClips;
    public AnimationClip[] hitClips;

}
