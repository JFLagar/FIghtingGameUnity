using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillIssue
{
    namespace Inputs { }
}
public interface ICommandInput
{
    public void InputPressed() { }
    public void InputReleased() { }
    public void InputHold(float time) { }
}
