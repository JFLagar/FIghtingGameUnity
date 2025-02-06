using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxResponder
{
    public void BoxCollisionedWith(Collider2D collider);
}
