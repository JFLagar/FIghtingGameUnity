using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimationsData", menuName = "Scriptable Objects/CharacterAnimationsData")]
public class CharacterAnimationsData : ScriptableObject
{
    //0 Standing and Walking
    [SerializeField]
    public AnimationClip[] standingClips;
    [SerializeField]
    public AnimationClip[] jumpingClips;
    [SerializeField]
    public AnimationClip crouchingClip;
    [SerializeField]
    public AnimationClip[] blockingClips;
    [SerializeField]
    public AnimationClip[] hitClips;
    [SerializeField]
    public AnimationClip[] stateTransitionClips;
    [SerializeField]
    public AnimationClip[] wakeupClips;
    [SerializeField]
    public AnimationClip[] recoveryClips;
    [SerializeField]
    public AnimationClip[] cancelClips;

    public AnimationClip[] GetStandingClips() {  return standingClips; }
    public AnimationClip[] GetJumpingClips() { return jumpingClips; }
    public AnimationClip GetCrouchingClip() { return crouchingClip; }
    public AnimationClip[] GetBlockingClips() { return blockingClips; }
    public AnimationClip[] GetHitClips() { return hitClips; }
    public AnimationClip[] GetStateTransitionClips() { return stateTransitionClips; }
    public AnimationClip[] GetWakeupClips() { return  wakeupClips; }
    public AnimationClip[] GetRecoveryClips() { return recoveryClips; }
    public AnimationClip[] GetCancelClips() { return cancelClips; }

}
