using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// States of the drill ship
/// </summary>
public enum ShipState
{
    Depart, // 0 (default)
    Drill,  // 1
    Return  // 2
}

/// <summary>
/// Extracts materials from planets and brings them back to the bus.
/// </summary>
public class DrillShip : Agent
{
    // images for drill state
    [SerializeField]
    Sprite drillEmpty, drillOne, drillTwo, drillFull, flyFull;


    [SerializeField]
    float avoidMultiplier = 1f, seekMultplier = 1f, separateMultiplier = 1f;

    [SerializeField]
    float asteroidAvoidDistance, shipSeparateDistance, planetSeekDistance;

    [SerializeField]
    float separateSpace; // buffer for ships to keep, personal space

    private List<Asteroid> foundAsteroids = new List<Asteroid>();
    private List<DrillShip> foundShips = new List<DrillShip>();

    public ShipState currentState;

    private Planet targetPlanet;

    private Vector3 landPos = Vector3.zero;
    private Vector3 returnPos = Vector3.zero;

    // drill timer
    [SerializeField]
    float drillTime;
    private float drillTimeProgress = 0f;


    // called in base.Update()
    public override void CalcSteeringForces()
    {

        switch (currentState)
        {
            case ShipState.Depart:
                // look for planet
                SeekClosestPlanet();

                // avoid asteroids
                totalForces += AvoidAsteroids() * avoidMultiplier;

                // separate from other ships
                totalForces += AvoidShips() * separateMultiplier;

                break;

            case ShipState.Drill:
                // no collision handling or avoiding
                // no forces to calculate
                break;

            case ShipState.Return:
                // natural wander
                //totalForces += Wander();

                // seek edge of world
                totalForces += Seek(returnPos * 2);

                // avoid asteroids
                totalForces += AvoidAsteroids() * avoidMultiplier;
                
                // separate from other ships
                totalForces += AvoidShips() * separateMultiplier;
                break;

            default:
                break;
        }

    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        transform.position = new Vector3(transform.position.x, transform.position.y, -2.0f); // keep on z = -2

        if (currentState == ShipState.Depart)
        {
            if (PlanetLandCheck()) currentState = ShipState.Drill;

            // can collide with asteroids
            AsteroidCollisionCheck();
        }

        if (currentState == ShipState.Drill)
        {
            // invulnerable to asteroids

            // keep ship in place
            transform.position = landPos;

            // increment time
            drillTimeProgress += Time.deltaTime;

            // handle event
            Drill();

            // check for time up
            if (drillTimeProgress > drillTime)
            {
                currentState = ShipState.Return;

                targetPlanet.busyShips.Remove(this);
                targetPlanet = null;

                GetComponent<SpriteRenderer>().sprite = flyFull;
                returnPos = GameManager.Instance.GetRandomEdgePoint();
            }

        }

        if (currentState == ShipState.Return)
        {
            // can collide with asteroids
            AsteroidCollisionCheck();
        }
    }

    /// <summary>
    /// Avoid nearby asteroids in view of the ship.
    /// </summary>
    /// <returns>Resulting vector to avoid all applicable asteroids.</returns>
    private Vector3 AvoidAsteroids()
    {
        foundAsteroids.Clear();

        Vector3 avoidForce = Vector3.zero;

        Vector3 toAsteroid = transform.position; // Agent to Asteroid
        float forwardDot;

        // handle obstacles
        foreach (Asteroid a in GameManager.Instance.asteroids)
        {
            toAsteroid = a.transform.position - transform.position;

            // first vec is what you want to measure
            // second vec is normalized of what you want to measure
            forwardDot = Vector3.Dot(toAsteroid, physicsObject.Direction);

            if (forwardDot >= 0)
            {
                foundAsteroids.Add(a);
            }
        }

        // handle obstacles in view
        foreach (Asteroid a in foundAsteroids)
        {
            float x = Mathf.Pow(a.transform.position.x, 2);
            float y = Mathf.Pow(a.transform.position.y, 2);

            // obstacle and agent are too close
            if ((x + y) < Mathf.Pow(asteroidAvoidDistance, 2))
            {
                // avoid obstacle
                if (toAsteroid != transform.position) avoidForce += Evade(a);
            }
        }

        // resulting force
        return avoidForce;
    }

    /// <summary>
    /// Separates itself from other ships to avoid
    /// being on top of each other.
    /// </summary>
    /// <returns>Resulting vector of all nearby ships to avoid them.</returns>
    private Vector3 AvoidShips()
    {
        foundShips.Clear();

        Vector3 avoidForce = Vector3.zero;


        // handle obstacles
        foreach (DrillShip ship in GameManager.Instance.ships)
        {
            // for ships that are not itself
            if (ship != this)
            {
                float x = Mathf.Pow(ship.transform.position.x, 2);
                float y = Mathf.Pow(ship.transform.position.y, 2);

                // obstacle and agent are too close
                if ((x + y) < Mathf.Pow(shipSeparateDistance, 2))
                {
                    // avoid obstacle
                    avoidForce += Evade(ship);
                }
            }
        }

        // resulting force
        return avoidForce;
    }

    /// <summary>
    /// Handles force calculation to find a
    /// planet nearby, or move towards the closest one.
    /// </summary>
    private void SeekClosestPlanet()
    {
        float componentDist, highDist = float.MinValue;
        Planet closestPlanet = null;

        foreach (Planet planet in GameManager.Instance.planets)
        {
            // check if within threshold (planetSeekDistance)
            float xDistance = Mathf.Abs(planet.transform.position.x - transform.position.x);
            float yDistance = Mathf.Abs(planet.transform.position.y - transform.position.y);
            componentDist = Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2);

            if (componentDist <= Mathf.Pow(planetSeekDistance, 2))
            {
                // new closest distance
                if (componentDist > highDist)
                {
                    highDist = componentDist;
                    closestPlanet = planet;
                }
            }
        }

        // no planets nearby, look for one in another space
        if (closestPlanet == null)
        {
            totalForces += Wander();
            return;
        }

        targetPlanet = closestPlanet;
        totalForces += Seek(targetPlanet.transform.position) * seekMultplier;
    }

    private bool PlanetLandCheck()
    {
        foreach (Planet p in GameManager.Instance.planets)
        {
            float xDistance = Mathf.Abs(p.transform.position.x - transform.position.x);
            float yDistance = Mathf.Abs(p.transform.position.y - transform.position.y);

            // collision check
            if (Mathf.Pow(p.Radius + (Radius / 2), 2) > (Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2)))
            {
                // stop ship movement
                physicsObject.Velocity = Vector3.zero;
                landPos = transform.position;
                landPos.z = -2.0f; // appear above planet

                Vector3 dir = p.transform.position - transform.position;

                // change direction to be opposite of planet, facing out
                transform.rotation = Quaternion.LookRotation(Vector3.back, dir);
                transform.Rotate(new Vector3(0f, 0f, 180f));

                p.busyShips.Add(this);

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Handles sprite updates to fill the ship.
    /// </summary>
    private void Drill()
    {
        // land buffer
        if (drillTimeProgress <= drillTime * 0.3)
        {
            GetComponent<SpriteRenderer>().sprite = drillEmpty;
        }
        // first third
        else if (drillTimeProgress <= drillTime * 0.6)
        {
            GetComponent<SpriteRenderer>().sprite = drillOne;
        }
        // second third
        else if (drillTimeProgress <= drillTime * 0.9)
        {
            GetComponent<SpriteRenderer>().sprite = drillTwo;
        }
        // full
        else
        {
            GetComponent<SpriteRenderer>().sprite = drillFull;
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
            // add to counter if full ship
            if (GetComponent<SpriteRenderer>().sprite == flyFull)
            {
                GameManager.Instance.shipReturnCount++;
            }

            // destroy ship
            GameManager.Instance.ships.Remove(this);
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Checks if this ship collides with an asteroid.
    /// Destroys the ship if there is a collision.
    /// </summary>
    private void AsteroidCollisionCheck()
    {
        foreach(Asteroid a in GameManager.Instance.asteroids)
        {
            float xDistance = Mathf.Abs(a.transform.position.x - transform.position.x);
            float yDistance = Mathf.Abs(a.transform.position.y - transform.position.y);

            if (Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2) < Mathf.Pow(Radius, 2))
            {
                GameManager.Instance.ships.Remove(this);
                Destroy(gameObject);
            }
        }
    }

    private new void OnDrawGizmosSelected()
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

        Gizmos.DrawLine(transform.position, targetPlanet.transform.position);

        /*foreach (Asteroid pos in foundAsteroids)
        {
            Gizmos.DrawLine(transform.position, pos.transform.position);
        }*/
    }
}
