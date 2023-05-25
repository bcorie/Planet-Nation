using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Agent
{
    [SerializeField]
    float seekMultiplier;

    [SerializeField]
    float maxDistance; // maximum distance needed for planets to seek each other

    // all ships on this planet that are drilling
    public List<DrillShip> busyShips = new List<DrillShip>();

    protected new void Start()
    {
        base.Start();

        radius = 1.69f;
    }

    protected new void Update()
    {
        base.Update();

        transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f); // keep on z = 0
    }

    public override void CalcSteeringForces()
    {
        physicsObject.Velocity = Vector3.zero;
        //totalForces += Seek(ClosestPlanet().transform.position) * seekMultiplier;
    }

    /// <summary>
    /// Check the boundaries of the
    /// world for collisions
    /// </summary>
    public override void EdgeCheck()
    {
        float x = GameManager.Instance.maxXBound;
        float y = GameManager.Instance.maxYBound;

        if (transform.position.x < -x || transform.position.x > x
            || transform.position.y < -y || transform.position.y > y)
        {
            GameManager.Instance.planets.Remove(this);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Find the closest planet to a planet's self.
    /// </summary>
    /// <returns>The closest planet, if within range. If not, returns itself</returns>
    private Planet ClosestPlanet()
    {
        float dist, highDist = float.MinValue;
        Planet closestPlanet;

        foreach (Planet planet in GameManager.Instance.planets)
        {
            if (planet == this) { continue; }

            dist = Vector3.Distance(planet.transform.position, transform.position);
            if (dist <= maxDistance)
            {
                if (dist > highDist)
                {
                    highDist = dist;
                    closestPlanet = planet;
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Determines if the planet is busy.
    /// </summary>
    /// <returns>True if there are busy ships on the planet. False if not.</returns>
    public bool IsBusy()
    {
        if (busyShips.Count == 0) return false;
        return true;
    }
}
