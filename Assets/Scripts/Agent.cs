using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(PhysicsObject))]
public abstract class Agent : MonoBehaviour
{
    protected PhysicsObject physicsObject;

    [SerializeField]
    float maxSpeed = 5f, maxForce = 5f;

    [SerializeField]
    protected float futureTime = 2f;

    [SerializeField]
    protected float wanderAngle = 0.0f, maxWanderChangePerSecond = 10.0f;

    protected Vector3 totalForces = Vector3.zero;

    private Camera cam;
    private float camHeight, camWidth;

    // bounds
    [SerializeField]
    protected float radius;

    [SerializeField]
    protected float spaceRadius;

    public float Radius { get { return radius; } }

    // Start is called before the first frame update
    protected void Start()
    {
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        physicsObject = GetComponent<PhysicsObject>();
    }

    // Update is called once per frame
    protected void Update()
    {
        CalcSteeringForces();
        
        totalForces = Vector3.ClampMagnitude(totalForces, maxForce); // limit totalForces to maxForce

        physicsObject.ApplyForce(totalForces);
        totalForces = Vector3.zero;

        EdgeCheck();
    }



    public abstract void CalcSteeringForces();

    public abstract void EdgeCheck();

    /// <summary>
    /// Calculates a force for one object to
    /// move towards another
    /// </summary>
    /// <param name="targetPos">target position</param>
    /// <returns>force of seek</returns>
    public Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity = targetPos - transform.position;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        return seekingForce;
    }

    /// <summary>
    /// Calculates a force for one object
    /// to move away from another
    /// </summary>
    /// <param name="targetPos">target position</param>
    /// <returns>force of flee</returns>
    public Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity = transform.position - targetPos;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        return seekingForce;
    }

    /// <summary>
    /// Calculates a force to move
    /// to an Agent's future position.
    /// </summary>
    /// <param name="target">target</param>
    /// <returns>pursuing force</returns>
    public Vector3 Pursue(Agent target)
    {
        return Seek(target.CalcFuturePosition(4f));
    }

    /// <summary>
    /// Calculates a force to move away
    /// from an Agent's future position
    /// </summary>
    /// <param name="target">target</param>
    /// <returns>evading force</returns>
    public Vector3 Evade(Agent target)
    {
        return Flee(target.CalcFuturePosition(4f));
    }

    public Vector3 Wander()
    {
        float maxWanderChange = maxWanderChangePerSecond * Time.deltaTime;
        wanderAngle += Random.Range(-maxWanderChange, maxWanderChange);

        wanderAngle = Mathf.Clamp(wanderAngle, -maxWanderChange, maxWanderChange);

        Vector3 wanderTarget = Quaternion.Euler(0, 0, wanderAngle) * physicsObject.Velocity.normalized + physicsObject.transform.position;

        return Seek(wanderTarget);
    }

    public Vector3 StayInBounds(float time)
    {
        Vector3 futurePos = CalcFuturePosition(time);

        if ( futurePos.x > camWidth  ||
             futurePos.x < -camWidth ||
             futurePos.y > camHeight ||
             futurePos.y < -camHeight )
        {
            return Seek(new Vector3(cam.transform.position.x, cam.transform.position.y, 0f));
        }

        return Vector3.zero;
    }

    /*protected Vector3 Separate<T>(List<T> agents) where T : Agent
    {
        float sqaureSpace = Mathf.Pow(spaceRadius, 2);

        // loop through agents in space
        foreach (T agent in agents)
        {
            float squareDistance = Vector3.SqrMagnitude(agent.transform.position - transform.position);

            if (squareDistance < float.Epsilon)
            {
                continue;
            }

            if (squareDistance < sqaureSpace)
            {
                return Flee(agent.transform.position);
            }
        }

        return Vector3.zero;

        *//*float closestDistance = Mathf.Infinity;
        Agent closestAgent = null;

        // loop through all ships
        foreach (Agent agent in GameManager.Instance.agents)
        {
            float calcedDistance = Vector3.Distance(transform.position, agent.transform.position);
            if (calcedDistance <= Mathf.Epsilon) // bad math?
            {
                continue;
            }

            *//*if (calcedDistance < closestDistance && calcedDistance <= )
            {
            }*//*
        }

        if (closestAgent != null)
        {
            return Flee(closestAgent.transform.position);
        }

        return Vector3.zero;*//*
    }*/

    /// <summary>
    /// Calculates an object's position
    /// in the future.
    /// </summary>
    /// <param name="futureTime">length into the future to calculate</param>
    /// <returns>object's future position</returns>
    public Vector3 CalcFuturePosition(float time)
    {
        return physicsObject.Velocity * time + transform.position;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
