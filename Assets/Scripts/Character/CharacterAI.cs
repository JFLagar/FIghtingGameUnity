
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
    void Start()
    {
        movementTime = StartCoroutine(MovementTimeCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!initiated)
            return;

        CheckGameState();
        if (movementTime != null)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public void Initiate(InputHandler inputHandler)
    {
        handler = inputHandler;
        initiated = true;
        character = handler.character;
    }
    //Try to get closer to the oponent, when close enough do 5L if not 5H
    void CheckGameState()
    {
        //Check Distance Between them
        distance = ((character.xDiff + character.xDiff) / 2) * character.xDiff;
        generatedNumber = Random.Range(1, 11);
        switch(distance)
        {
            //Choices Move, Attack and Do Nothing
            //close by should move more away and less often and attack more often

            case <= 0.5f:
                aIState = AIState.Close;
                ActionSwitch(AIAction.None, AIAction.Attack,AIAction.Move);
                directionBool = false;
                break;
            case >= 1.5f:
                aIState = AIState.Far;
                ActionSwitch(AIAction.None, AIAction.Move,AIAction.Attack);
                directionBool = true;
                break;
            default:
                aIState = AIState.MidRange;
                ActionSwitch(AIAction.None, AIAction.Move,AIAction.Attack);
                directionBool = true;
                break;
        }
      
    }
    void ActionSwitch(AIAction highChance, AIAction mediumChance, AIAction lowChance)
    {
        switch (generatedNumber)
        {
            case <= 1:
                PerfomSimulatedAction(lowChance);
                break;
            case <= 2:
                PerfomSimulatedAction(mediumChance);
                break;
            case <= 5:
                PerfomSimulatedAction(highChance);
                break;
        }
    }
    void PerfomSimulatedAction(AIAction action)
    {
        aIAction = action;
        switch (action)
        {
            case AIAction.None:                
                SimulateNothing();
                break;
            case AIAction.Move:
                SimulateMovementInput();
                break;
            case AIAction.Attack:
                SimulateAttackInput();
                break;
        }
    }
    void SimulateAttackInput()
    {
        dir.y = 0;
        int randomId = 0;
        if (aIState == AIState.Close)
            randomId = Random.Range(0, 4);
        else
            randomId = Random.Range(0, 3);
        switch (randomId)
        {
            case 0:
                handler.LightFunction();
                break;
            case 1:
                handler.HeavyFunction();
                break;
            case 2:
                handler.SpecialFunction();
                break;
            case 4:
                handler.GrabFunction();
                break;
        }
        dir.y = 0;
    }
    void SimulateMovementInput()
    {
        randomTime = Random.Range(15, 101);
        int randomX = 0;

        if (directionBool)
        {
            randomX = Random.Range(-1, 4);
        }
        else
        {
            randomX = Random.Range(-4, 2);
        }
        switch (randomX)
        {
            case < 0:
                x = -1;
                break;
            case > 0:
                x = 1;
                break;
        }
        //check if jump
        int randomY = Random.Range(0, 101);
        int y = 0;
        switch (randomY)
        {
            case > 95:
                y = 1;
                break;
            case < 10:
                y = -1;
                break;
            default:
                y = 0;
                break;
        }
        if (movementTime != null && x == dir.x)
        {
            numberOfFrames += randomTime / 2;
        }
        else
        {
            numberOfFrames = randomTime;
            movementTime = StartCoroutine(MovementTimeCoroutine());
        }
        dir = new Vector2(x, y);
    }
    void SimulateNothing()
    {
        if (movementTime != null)
            numberOfFrames = numberOfFrames / 2;
        else
            dir = Vector2.zero;
    }
    IEnumerator MovementTimeCoroutine()
    {
        int i = 0;
        while (i < numberOfFrames)
        {
            i++;
            yield return null;
        }
        movementTime = null;
    }
    public void AiReset()
    {
        dir = Vector2.zero;
        initiated = false;
        handler = null;
    }
}
