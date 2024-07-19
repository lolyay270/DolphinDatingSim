using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Object Avoidance")]
    [SerializeField] private float avoidDistance;

    //internal variables
    private Rigidbody rb;
    private int biteAreaCount;
    private string status;
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
            }
            else
            {
                foreach (GameObject obj in objectsToAvoid)
                {
                    if (obj.CompareTag("Structure")) status = "wall"; //stopping wall collision is 1st priority
                    else if (obj.CompareTag("Player")) //ANY objects are dolphin
                    {
                        //flee 
                        continue;
                    }
                    //stop flee
                }
            }
        }
        else
        {
            status = "ok";
            //speed decrease
        }

        if (status == "ok") //random movement
        {
            Rotate();
            //Movement();
        }
        else if (status == "wall") //wall/floor too close
        {
            //dont speed up
            //turn slightly away from object
            //CollisionAvoidance();
        }
        else if (status == "dolphin") //dolphin too close
        {
            //speed up
            //turn completely away from object
        }
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

    //method to handle fish being eaten
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bite Area") && status != "eaten")
        {
            biteAreaCount++;
            if (biteAreaCount == 2)
            {
                status = "eaten";
                GameManager.Instance.Points += pointsOnEat;
                Destroy(gameObject);
            }
        }
    }

    //utility method to help handle fish being eaten
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bite Area") && status != "eaten")
        {
            biteAreaCount--;
        }
    }

    private void AddTriggerCollider(GameObject other)
    {
        objectsToAvoid.Add(other);
    }

    private void RemoveTriggerCollider(GameObject other)
    {
        objectsToAvoid.Remove(other);
    }
}
