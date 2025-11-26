using UnityEngine;

[CreateAssetMenu(fileName = "GeneralCombatValues", menuName = "Scriptable Objects/GeneralCombatValues")]
public class GeneralCombatValues : ScriptableObject
{
    [SerializeField]
    private int dashDuration = 10;
    [SerializeField]
    private int airDashAnimationDuration = 22;
    [SerializeField]
    private int halfMeter = 50;
    [SerializeField]
    private int hitstopBase = 8;
    [SerializeField]
    private int hitVerticalBase = 10;
    [SerializeField]
    private int hitstunBase = 10;
    [SerializeField]
    private int hitMovementDuration = 3;
    [SerializeField]
    private float backWalkReduction = 1.5f;
    [SerializeField]
    private float pushMultiplier = 3;
    [SerializeField]
    private float jumpMultiplier = 2;
    [SerializeField]
    private float dashMultiplier = 1.5f;
    [SerializeField]
    private Projectile baseProjectile;

    public int GetDashDuration() {  return dashDuration; }
    public int GetAirDashAnimationDuration() {  return airDashAnimationDuration; }
    public int GetHalfMeter() { return halfMeter; }
    public int GetHitstopBase() { return hitstopBase; }
    public int GetHitstunBase() { return hitstunBase; }
    public int GetHitVerticalBase() {  return hitVerticalBase; }
    public int GetHitMovementDuration() { return hitMovementDuration; }
    public float GetBackWalkReduction() {  return backWalkReduction; }
    public float GetPushMultiplier() {  return pushMultiplier; }
    public float GetJumpMultiplier() {  return jumpMultiplier; }
    public float GetDashMultiplier() { return dashMultiplier; }

    public Projectile GetProjectile() { return baseProjectile; }
}
