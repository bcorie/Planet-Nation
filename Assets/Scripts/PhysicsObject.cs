using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    private Vector3 direction = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    public Vector3 Direction { get { return direction; } set { direction = value; } }
    
    private Vector3 acceleration = Vector3.zero;

    [SerializeField]
    float mass = 1f, frictionCoefficient;

    [SerializeField]
    bool useFriction, useGravity;

    private Camera cam;
    private float camHeight, camWidth;

    [SerializeField]
    Vector3 gravity;

    

    // Start is called before the first frame update
    void Start()
    {
        // camera
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = cam.aspect * camHeight;
        
        velocity = Random.insideUnitCircle.normalized;
        
        if (useFriction)
        {
            ApplyFriction(frictionCoefficient);
        }

        // gravity
        if (useGravity)
        {
            gravity = Vector3.down;
            ApplyForce(gravity * mass); // mass calc cancels in funct
        }
    }

    // Update is called once per frame
    void Update()
    {
        velocity += acceleration * Time.deltaTime;
        direction = velocity.normalized;

        transform.position += velocity * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(Vector3.back, direction);

        // reset acceleration for Newton's first law
        acceleration = Vector3.zero;
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    public void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction *= coeff;
        ApplyForce(friction);
    }

}
