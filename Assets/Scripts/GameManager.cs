// Author: Corie Beale

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    #region Game Space
    [SerializeField]
    private Camera cam;

    [SerializeField]
    float cameraMovementMultiplier;

    public float camHeight, camWidth;

    public Vector3 mousePos; // TODO: make private after debugging

    [SerializeField]
    public float maxXBound = 20.0f;

    [SerializeField]
    public float maxYBound = 20.0f;
    #endregion

    #region Agents
    // Asteroids
    [SerializeField]
    private int maxAsteroidCount = 10;
    [SerializeField]
    public Agent asteroidPrefab;

    public List<Agent> asteroids = new List<Agent>();

    // Planets
    [SerializeField]
    private int maxPlanetCount = 10;
    [SerializeField]
    private int initialPlanetSpawnCount = 2;
    [SerializeField]
    public Agent planetPrefab;

    public List<Agent> planets = new List<Agent>();

    // Ships
    [SerializeField]
    private int maxShipCount = 1;
    [SerializeField]
    public Agent shipPrefab;

    public List<Agent> ships = new List<Agent>();
    #endregion

    #region Text

    [SerializeField]
    Text counterText;

    [SerializeField]
    Button resetButton;

    [SerializeField]
    Text planetText;

    public int shipReturnCount = 0;
    private bool resetPressed = false;

    #endregion

    // user input
    [SerializeField]
    EventSystem eventSystem;

    [SerializeField]
    private float inputCooldown = 1.0f; // 1 sec
    private float inputCooldownProgress = 0.0f;
    private bool cooldownActive = false;

    [SerializeField]
    bool useCamMove = true;

    // Start is called before the first frame update
    void Start()
    {
        // camera
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = cam.aspect * camHeight;

        // spawn initial asteroids
        for (int i = 0; i < maxAsteroidCount; i++)
        {
            float randomX = Random.Range(-maxXBound, maxXBound);
            float randomY = Random.Range(-maxYBound, maxYBound);

            asteroids.Add(Instantiate(asteroidPrefab, new Vector3(randomX, randomY, -1.0f), Quaternion.identity));
        }

        // spawn initial planets
        for (int i = 0; i < initialPlanetSpawnCount; i++)
        {
            float randomX = Random.Range(-maxXBound, maxXBound);
            float randomY = Random.Range(-maxYBound, maxYBound);

            planets.Add(Instantiate(planetPrefab, new Vector3(randomX, randomY, 0.0f), Quaternion.identity));
        }

        // spawn initial ships
        for (int i = 0; i < maxShipCount; i++)
        {
            float randomX = Random.Range(-maxXBound, maxXBound);
            float randomY = Random.Range(-maxYBound, maxYBound);

            ships.Add(Instantiate(shipPrefab, new Vector3(randomX, randomY, -2.0f), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse information
        mousePos = Mouse.current.position.ReadValue();
        mousePos = cam.ScreenToWorldPoint(mousePos);
        mousePos.z = transform.position.z; // match with canvas for ui

        // repopulate any destroyed agents
        SpawnAsteroids(); // asteroids
        SpawnDrillShips(); // ships

        // camera movement
        if (useCamMove)
        {
            // check camera and mouse bounds for any actions to be done
            MouseBoundsCheck();
            CameraBoundsCheck();
        }

        // handle cooldown if active
        if (cooldownActive)
        {
            inputCooldownProgress += Time.deltaTime;

            if (inputCooldownProgress >= inputCooldown)
            {
                cooldownActive = false;
                inputCooldownProgress = 0.0f;
            }
        }

        // update texts
        counterText.text = "Successful drill trips: " + shipReturnCount;
        if (planets.Count == maxPlanetCount) // max
        {
            planetText.text = "Planets maxed! Planet count: " + planets.Count;
        }
        else
        {
            planetText.text = "Planet count: " + planets.Count;
        }
    }

    /// <summary>
    /// Ensures the camera does not move too far from
    /// the game space.
    /// </summary>
    private void CameraBoundsCheck()
    {
        // right
        if (cam.transform.position.x + camWidth > maxXBound)
        {
            cam.transform.position = new Vector3(maxXBound - camWidth, cam.transform.position.y, cam.transform.position.z);
        }
        // left
        if (cam.transform.position.x - camWidth < -maxXBound)
        {
            cam.transform.position = new Vector3(-maxXBound + camWidth, cam.transform.position.y, cam.transform.position.z);
        }
        // top
        if (cam.transform.position.y + camHeight > maxYBound)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, maxYBound - camHeight, cam.transform.position.z);
        }
        // bottom
        if (cam.transform.position.y - camHeight < -maxYBound)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, -maxYBound + camHeight, cam.transform.position.z);
        }

    }

    /// <summary>
    /// Checks if the mouse is on the edge of the screen
    /// and moves the camera in the direction of the edge
    /// of the screen the mouse is on.
    /// </summary>
    private void MouseBoundsCheck()
    {
        float deadZoneHalfY = camHeight * 0.8f;
        float deadZoneHalfX = camWidth * 0.8f;

        // right
        if (mousePos.x >= cam.transform.position.x + deadZoneHalfX)
        {
            cam.transform.position += new Vector3(cameraMovementMultiplier, 0);
        }
        // left
        if (mousePos.x <= cam.transform.position.x - deadZoneHalfX)
        {
            cam.transform.position += new Vector3(-cameraMovementMultiplier, 0);
        }
        // top
        if (mousePos.y >= cam.transform.position.y + deadZoneHalfY)
        {
            cam.transform.position += new Vector3(0, cameraMovementMultiplier);
        }
        // bottom
        if (mousePos.y <= cam.transform.position.y - deadZoneHalfY)
        {
            cam.transform.position += new Vector3(0, -cameraMovementMultiplier);
        }
    }

    public void ResetCounter()
    {
        shipReturnCount = 0;
        resetPressed = true;

        Debug.Log("Reset pressed");
    }

    #region Spawning
    private void SpawnAsteroids()
    {
        // as many needed to meet max
        for (int i = asteroids.Count; i < maxAsteroidCount; i++)
        {
            asteroids.Add(Instantiate(asteroidPrefab, GetRandomEdgePoint(), Quaternion.identity));
        }
    }

    private void SpawnDrillShips()
    {
        // as many needed to meet max
        for (int i = ships.Count; i < maxShipCount; i++)
        {
            ships.Add(Instantiate(shipPrefab, GetRandomEdgePoint(), Quaternion.identity));
        }
    }
    private Planet MouseIsOverPlanet()
    {
        foreach (Planet planet in planets)
        {
            Vector3 mousePosPlanet = new Vector3(mousePos.x, mousePos.y, 0.0f);

            if (Vector3.Distance(planet.transform.position, mousePosPlanet) <= planet.Radius)
            {
                return planet;
            }
        }

        return null;

    }

    /// <summary>
    /// Checks for planet placement. Will either place or destroy a planet.
    /// </summary>
    /// <param name="context">input</param>
    public void OnMouseDown()
    {
        // planet interaction where there is no active cooldown
        // and the mouse is not over and UI elements
        if (!cooldownActive && !eventSystem.IsPointerOverGameObject())
        {
            Planet planet = MouseIsOverPlanet();

            Vector3 mousePosPlanet = new Vector3(mousePos.x, mousePos.y, 0.0f);

            // planet to remove
            if (MouseIsOverPlanet() != null && !planet.IsBusy())
            {
                planets.Remove(planet);
                Destroy(planet.gameObject);

                Debug.Log("Planet destroyed");
            }

            // limit planet spawning
            else if (planets.Count < maxPlanetCount)
            {
                // spawn planet
                planets.Add(Instantiate(planetPrefab, mousePosPlanet, Quaternion.identity));

                Debug.Log("Planet spawned");
            }

            cooldownActive = true;
        }

        resetPressed = false;
    }

    /// <summary>
    /// Finds a random edge to spawn an object
    /// on the border of the world
    /// </summary>
    /// <returns>The point on the edge of the world.</returns>
    public Vector3 GetRandomEdgePoint()
    {
        // find an edge to get a point from
        int side = Random.Range(1, 4);

        float x, y;

        switch (side)
        {
            // top
            case 1:
                x = Random.Range(-maxXBound, maxXBound);
                y = maxYBound;
                break;

            // bottom
            case 2:
                x = Random.Range(-maxXBound, maxXBound);
                y = -maxYBound;
                break;

            // left
            case 3:
                x = -maxXBound;
                y = Random.Range(-maxYBound, maxYBound);
                break;

            // right
            case 4:
                x = maxXBound;
                y = Random.Range(-maxYBound, maxYBound);
                break;
                
            default:
                return Vector3.zero;
        }

        return new Vector3(x, y, 0f);
    }
    #endregion
}
