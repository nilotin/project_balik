using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Gemi
    public Vector3 offset;          // Kamera - gemi arası mesafe
    public float smoothSpeed = 5f;  // Takip yumuşaklığı

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}