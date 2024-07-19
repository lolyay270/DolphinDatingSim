using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    [SerializeField] private int pointsOnEat;

    [Header("Water Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float avgAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] private float rotationDelayTime;
    [SerializeField] private float massInWater;
    [SerializeField] private float dragInWater;

    [Header("Air Movement")]
    [SerializeField] private float massOutWater;
    [SerializeField] private float dragOutWater;

    [Header("Object Avoidance")]
    [SerializeField] private float avoidDistance;
    [SerializeField] private float fleeSpeedMultiplier;
    [SerializeField] private float wallLeaveAngle;



    //internal variables
    private Rigidbody rb;
    private int biteAreaCount;
    private string status;

    private bool flee = false;
    private FishAvoidObjects largeTrigger;
    private List<GameObject> objectsToAvoid = new();


    //---------------------- METHODS ----------------------\\

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // object avoidance setup
        status = "ok";
        GetComponentInChildren<SphereCollider>().radius = avoidDistance / 2; //radius is half diameter
        largeTrigger = GetComponentInChildren<FishAvoidObjects>();
        largeTrigger.OnTriggerCollide.AddListener(AddTriggerCollider);
        largeTrigger.OnTriggerLeave.AddListener(RemoveTriggerCollider);

    }

    private void Update()
    {
        //check what surrounds the fish every frame
        if (objectsToAvoid.Count > 0)
        {
            //only focus on dolphin avoidance if there is no walls around
            if (objectsToAvoid.Count == 1 && objectsToAvoid[0].CompareTag("Player"))
            {
                status = "dolphin";
                flee = true;
            }
            else
            {
                foreach (GameObject obj in objectsToAvoid)
                {
                    if (obj.CompareTag("Structure")) status = "wall"; //stopping wall collision is 1st priority
                    else if (obj.CompareTag("Player") || obj.CompareTag("Bite Area")) //ANY objects are dolphin
                    {
                        flee = true;
                        continue;
                    }
                    flee = false; //no objects are dolphin
                }
            }
        }
        else //objectsToAvoid.Count == 0
        {
            status = "ok";
            flee = false;
        }

        Movement(); //always move the fish forward
        RandomRotate();

        switch (status)
        {
            case "ok":
                break;
            case "wall":
                AvoidWallCollision();
                break;
            case "dolphin":
                //turn completely away from object
                break;
        }
    }

    //method to handle swimming with no collision checks
    private void RandomRotate()
    {
        //make new horizontal angle, no constraints
        float horRotate = Random.Range(-avgAngle, avgAngle) + rb.rotation.eulerAngles.y;

        //make new vertical angle, constrained by maxAngle
        float currentVert = FixRotationToNeg(rb.rotation.eulerAngles.x);
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

    //method to handle fish being eaten and air physics
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bite Area"))
        {
            biteAreaCount++; //need counter since largeTrigger hits Bite Area too
            if (biteAreaCount == 2)
            {
                status = "eaten";
                GameManager.Instance.Points += pointsOnEat;
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Water"))
        {
            rb.drag = dragInWater;
            rb.mass = massInWater;
            rb.useGravity = false;
        }
    }

    //utility method to handle air physics and help handle fish being eaten
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bite Area"))
        {
            biteAreaCount--;
        }

        if (other.CompareTag("Water")) //going into air
        {
            //rotate fish downwards
            float fishX = FixRotationToNeg(rb.rotation.eulerAngles.x);
            if (fishX < maxAngle && fishX > -maxAngle) //stop over rotation
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(avgAngle, 0, 0)); //tilt fish down each frame
            }

            rb.drag = dragOutWater;
            rb.mass = massOutWater;
            rb.useGravity = true;
        }
    }

    private void AvoidWallCollision()
    {
        GameObject avoid = objectsToAvoid[0]; //1 object in list, must be a wall 
        if (objectsToAvoid.Count > 1) //if more than 1, find closest wall
        {
            foreach (GameObject obj in objectsToAvoid)
            {
                if (!avoid.CompareTag("Structure")) //chance dolphin could be first in list and closer than walls
                {
                    avoid = obj;
                }
                else if (obj != avoid)
                {
                    //if next obj is closer to fish than last obj
                    if (Vector3.Distance(transform.position, obj.transform.position) < Vector3.Distance(transform.position, avoid.transform.position))
                    {
                        avoid = obj;
                    }
                }
            }
        }

        //rotate away from object
        if (avoid.transform.rotation.eulerAngles.x == 270) //obj is wall
        {
            float wallY = FixRotationToNeg(avoid.transform.rotation.eulerAngles.y);
            float fishY = FixRotationToNeg(rb.rotation.eulerAngles.y);
            if (fishY >= wallY && fishY < ((wallY + 90 + wallLeaveAngle) % 360)) //wall front left of fish
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, avgAngle, 0)); //increase fish y
            }
            if (fishY > wallY - 90 - wallLeaveAngle && fishY <= wallY) //wall front right of fish
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -avgAngle, 0)); //decrease fish y
            }
        }
        else //obj is floor
        {
            float fishX = FixRotationToNeg(rb.rotation.eulerAngles.x);

            if (fishX < maxAngle && fishX > -maxAngle) //stop over rotation
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(-avgAngle, 0, 0)); //tilt fish upwards each frame
            }
        }
    }

    //utility method to keep a list of objects in largeTrigger
    private void AddTriggerCollider(GameObject other)
    {
        objectsToAvoid.Add(other);
    }

    //utility method to keep a list of objects in large trigger
    private void RemoveTriggerCollider(GameObject other)
    {
        objectsToAvoid.Remove(other);
    }

    //utility method to change angles higher than 180
    private float FixRotationToNeg(float angle)
    {
        //fixes 270 to be -90 etc
        return (angle > 180 ? angle - 360 : angle);
    }
}