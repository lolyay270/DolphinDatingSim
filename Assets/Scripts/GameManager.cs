using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Game Statistics
    private int points;
    public int Points 
    {
        get { return points; } set { points = value; print(points); }
    }

    //singleton
    public static GameManager Instance;


    void Awake()
    {
        //only one singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this);
        }

        //hide cursor
        Cursor.lockState = CursorLockMode.Locked;
    }
}
