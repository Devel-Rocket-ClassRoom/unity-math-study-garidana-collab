using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInput : MonoBehaviour
{
    private static readonly string MoveAxis = "Vertical";
    private static readonly string RotateAxis = "Horizontal";

    public float Move {get; private set;}
    public float Rotate {get; private set;}

    public void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move = Input.GetAxis(MoveAxis);
        Rotate = Input.GetAxis(RotateAxis);
    }
}
