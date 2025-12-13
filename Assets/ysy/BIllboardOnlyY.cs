using UnityEngine;

public class BillboardYOnly : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.y = 0f;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
