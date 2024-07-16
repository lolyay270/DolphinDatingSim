using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Rotate();
    }

    private void Movement() 
    {
        //random forward velocity
    }

    private void Rotate()
    {
        //random rotation within X 
        //does NOT swim into walls/floor
    }

    private void OnTriggerEnter(Collider other)
    {
        //if enters dolphin head area and dolphin bites
          // destroy this fish
    }
}
