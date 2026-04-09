using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;

    public void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        float angle = playerInput.Rotate * rotationSpeed * Time.deltaTime;
        playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(0f, angle, 0f));

        Vector3 delta = playerInput.Move * transform.forward * moveSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + delta);
    }
}
