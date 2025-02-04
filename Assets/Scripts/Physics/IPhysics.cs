using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.CharacterSpace;
public interface IPhysics
{
    void ApplyForce(Vector2 direction, float duration, bool counterForce); //character getting hit or jumping
    void ApplyGravity(); //character once in the air is getting gravity down
}
