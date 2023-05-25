// Author: Corie Beale

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Agent
{
    [SerializeField]
    float maxPlanetDistance = 5f;

    [SerializeField]
    float wanderMultiplier = 1f, gravitateMultiplier = 1f;

    private List<Vector3> foundPlanetPositions = new List<Vector3>();

    protected new void Start()
    {
        base.Start();

        radius = 0.58f;
    }

    protected new void Update()
    {
        base.Update();

        transform.position = new Vector3(transform.position.x, transform.position.y, -1.0f); // keep on z = -1

        PlanetCollisionCheck();
    }

    public override void CalcSteeringForces()
    {
        totalForces += Wander() * wanderMultiplier;

        totalForces += GravitateToPlanets() * gravitateMultiplier;
    }

    /// <summary>
    /// Slowly move towards nearby planets.
    /// </summary>
    /// <returns>The resulting force of nearby planets.</returns>
    private Vector3 GravitateToPlanets()
    {
        foundPlanetPositions.Clear();

        Vector3 attractForce = Vector3.zero;
        Vector3 AtoP = Vector3.zero; // asteroid (self) to planet
        float dot;

        // handle all planets
        foreach (Planet planet in GameManager.Instance.planets)
        {
            AtoP = planet.transform.position - transform.position;

            dot = Vector3.Dot(AtoP, physicsObject.Direction);

            if (dot >= 0)
            {
                foundPlanetPositions.Add(planet.transform.position);
            }
        }

        // handle planets in view of asteroid
        foreach (Vector3 pos in foundPlanetPositions)
        {
            // if they are close enough, move towards it
            float xDistance = Mathf.Abs(pos.x - transform.position.x);
            float yDistance = Mathf.Abs(pos.y - transform.position.y);

            if (Mathf.Pow(maxPlanetDistance, 2) > (Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2)))
            {
                attractForce += Seek(pos);
            }
        }

        // resulting force
        return attractForce;
    }

    /// <summary>
    /// Checks for collisions with all planets in space.
    /// If collided, will destroy the asteroid and create
    /// a new one in space to replace it.
    /// </summary>
    private void PlanetCollisionCheck()
    {
        foreach (Planet p in GameManager.Instance.planets)
        {
            float xDistance = Mathf.Abs(p.transform.position.x - transform.position.x);

            float yDistance = Mathf.Abs(p.transform.position.y - transform.position.y);

            if (Mathf.Pow(p.Radius + (Radius / 2), 2) > (Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2)))
            {
                // remove asteroid that collided with planet
                GameManager.Instance.asteroids.Remove(this);
                Destroy(gameObject);
            }
        }


    }

    /// <summary>
    /// Check the boundaries of the
    /// world for collisions
    /// </summary>
    public override void EdgeCheck()
    {
        float x = GameManager.Instance.maxXBound;
        float y = GameManager.Instance.maxYBound;

        if (transform.position.x < (-x - radius) || transform.position.x > (x + radius)
            || transform.position.y < (-y - radius) || transform.position.y > (y + radius))
        {
            // destroy asteroid
            GameManager.Instance.asteroids.Remove(this);
            Destroy(gameObject);
        }

    }

    /*private void OnDrawGizmosSelected()
    {
        //
        //  Draw safe space box
        //
        Vector3 futurePos = CalcFuturePosition(futureTime);

        float dist = Vector3.Distance(transform.position, futurePos) + Radius;

        Vector3 boxSize = new Vector3(Radius * 2f,
            dist
            , Radius * 2f);

        Vector3 boxCenter = Vector3.zero;
        boxCenter.y += dist / 2f;

        Gizmos.color = Color.green;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.matrix = Matrix4x4.identity;

        // Radius
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, Radius);

        //
        //  Draw lines to found obstacles
        //
        Gizmos.color = Color.yellow;

        foreach (Vector3 pos in foundPlanetPositions)
        {
            Gizmos.DrawLine(transform.position, pos);
        }
    }*/
}
