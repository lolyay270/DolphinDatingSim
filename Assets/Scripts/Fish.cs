using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    [SerializeField] private int pointsOnEat;

    private Rigidbody rb;
    private bool eaten;

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
        if (other.CompareTag("Bite Area") && !eaten)
        {
            eaten = true;
            GameManager.Instance.Points += pointsOnEat;
            Destroy(gameObject);
        }
    }
}
