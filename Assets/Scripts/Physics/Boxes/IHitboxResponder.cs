using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxResponder
{
    public void CollisionedWith(Collider2D collider);
}
