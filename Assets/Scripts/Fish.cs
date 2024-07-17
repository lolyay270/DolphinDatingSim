using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    [SerializeField] private int pointsOnEat;

    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float avgAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] private float rotationDelayTime;

    [Header("Collision Avoidance")]
    [SerializeField] private float wallFloorDistance;
    [SerializeField] private int layerToAvoid = 3;

    private Rigidbody rb;
    private bool eaten;
    private Quaternion prevAngle;
    private RaycastHit closestStructure;
    private LayerMask layerMask;

    //---------------------- METHODS ----------------------\\

    private void Awake()
    {
        layerMask = 1 << layerToAvoid;
        rb = GetComponent<Rigidbody>();
        prevAngle = rb.rotation;
    }

    private void Update()
    {
        Rotate();
        Movement();
    }

    //method to handle swimming with no collision checks
    private void Rotate()
    {
        //make new horizontal angle, no constraints
        float horRotate = Random.Range(-avgAngle, avgAngle) + rb.rotation.eulerAngles.y;

        //make new vertical angle, constrained by maxAngle
        float currentVert = rb.rotation.eulerAngles.x;
        currentVert = (currentVert > 180 ? currentVert - 360 : currentVert); //fix 340 into -20
        float vertRotate = Random.Range(-avgAngle, avgAngle) + currentVert; 
        vertRotate = Mathf.Clamp(vertRotate, -maxAngle, maxAngle); //clamp to max angles

        //rotate fish with new angles
        rb.MoveRotation(Quaternion.Euler(vertRotate, horRotate, 0));
    }

    //method to push the fish forward
    private void Movement() 
    {
        float velocity = Random.Range(minSpeed, maxSpeed);
        rb.AddRelativeForce(Vector3.forward * velocity);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bite Area") && !eaten)
        {
            eaten = true;
            GameManager.Instance.Points += pointsOnEat;
            Destroy(gameObject);
        }

        if (other.CompareTag("Structure"))
        {
            //CollisionAvoidance();
            print("collision avoidance");
        }
    }

    private void CollisionAvoidance()
    {
        RaycastHit hit;

        // Cast a sphere wrapping character controller 10 meters forward to see if it is about to hit anything.
        if (Physics.SphereCast(transform.position, transform.lossyScale.y / 2, transform.forward, out hit, wallFloorDistance, layerMask))
        {
            closestStructure = hit;
        }
        else
        {
            closestStructure = new();
        }

        if (closestStructure.point != Vector3.zero) //if too close to a wall
        {
            float deltaX = transform.position.x - closestStructure.point.x;
            float angleToStructure = Mathf.Asin(deltaX / closestStructure.distance);
            print(angleToStructure);
        }
    }
}
