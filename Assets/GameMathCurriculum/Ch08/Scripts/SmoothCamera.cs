using Unity.VisualScripting;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3 (0f, 5f, -8f);
    [SerializeField] private float positionSmoothTime = 0.3f;
    [SerializeField] private float rotationSmoothSpeed = 5f;

    private Vector3 positionVelocity = Vector3.zero;

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.TransformPoint(offset);


        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref positionVelocity,
            positionSmoothTime
        );

        Vector3 look = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(look);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}
