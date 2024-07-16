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
        StartCoroutine(Rotate());
    }

    private void Update()
    {
        Movement();
    }

    //method to push the fish forward
    private void Movement() 
    {
        float velocity = Random.Range(minSpeed, maxSpeed);
        rb.AddForce(Vector3.forward * velocity);
    }

    //method to handle swimming with no collision checks
    private IEnumerator Rotate()
    {
        float xAngle = Random.Range(-avgAngle, avgAngle);
        float yAngle = Random.Range(-avgAngle, avgAngle);
        Quaternion newAngle = Quaternion.Euler(xAngle, yAngle, 0);
        newAngle.Normalize();
        print(newAngle.eulerAngles);
        rb.rotation = newAngle * prevAngle;
        prevAngle = newAngle;

        yield return null;
        //yield return new WaitForSeconds(rotationDelayTime);
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
