///<summary>
///This class controls the trigger for a large sphere collider around the Fish game object
///</summary>

using UnityEngine;
using UnityEngine.Events;

public class FishAvoidObjects : MonoBehaviour
{
    //create custom event type
    public class FishEvent : UnityEvent<GameObject> { }

    //call the names slightly different
    public FishEvent OnTriggerCollide = new();
    public FishEvent OnTriggerLeave = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Structure"))
        {
            OnTriggerCollide?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Structure"))
        {
            OnTriggerLeave?.Invoke(other.gameObject);
        }
    }
}