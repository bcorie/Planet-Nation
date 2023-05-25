using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : Agent
{

    /// <summary>
    /// Check the boundaries of the
    /// world for collisions
    /// </summary>
    public override void EdgeCheck()
    {
    }
    public override void CalcSteeringForces()
    {
        totalForces += Wander();

        totalForces += StayInBounds(futureTime);
    }
}
