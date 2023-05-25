/*using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TagState
{
    NotIt,      // 0 (default)
    Counting,   // 1
    It          // 2
}


public class TagPlayer : Agent
{

    [SerializeField]
    float boundsScalar = 1f, wanderScalar = 1f, separateScalar = 1f, fleeScalar = 1f;

    public TagState currentState;

    [SerializeField]
    int countToValue = 5;
    float countTimer = 10;

    public override void CalcSteeringForces()
    {

        switch (currentState)
        {
            case TagState.NotIt: // wander

                totalForces += Flee(AgentManager.Instance.ItPLayerPosition()) * fleeScalar;

                #region Wander
                //totalForces += Separate() * separateScalar; // fix math

                totalForces += Wander(futureTime) * wanderScalar;

                totalForces += StayInBounds(futureTime) * boundsScalar;
                #endregion Wander

                break;

            case TagState.Counting: // don't move

                totalForces = Vector3.zero;
                countTimer += Time.deltaTime;

                if (countTimer >= countToValue)
                {
                    countTimer = 0;
                    currentState++; // become it
                }
                break;

            case TagState.It:

                totalForces += Seek(AgentManager.Instance.ClosestNotItPlayer());
                
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, GetComponent<PhysicsObject>().Velocity);

        Gizmos.color = Color.magenta;
        //Gizmos.DrawRay(transform.position, wanderForce);

        Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(transform.position, boundsForce);

    }
}
*/