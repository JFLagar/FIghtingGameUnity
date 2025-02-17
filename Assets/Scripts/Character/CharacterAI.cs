
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.Inputs;
using SkillIssue.CharacterSpace;
using UnityEngine.InputSystem;

public enum AIAction
{
    None,
    Move,
    Attack
}
public enum AIState
{
    Close,
    MidRange,
    Far
}
public class CharacterAI : MonoBehaviour
{
    public AIAction aIAction;
    public AIState aIState;
    InputHandler handler;
    Character character;
    public bool initiated = false;
    public Vector2 dir = Vector2.zero;
    public int attackInput;
    public bool buttonUp;
    public bool directionBool;
    public int x;
    public float distance;
    public int generatedNumber;
    Coroutine movementTime;
    public bool isMoving;
    public int numberOfFrames;
    int randomTime = 30;
    // Start is called before the first frame update

}
