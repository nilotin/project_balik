using UnityEngine;

public class IceSurfaceSlide : MonoBehaviour
{
    public string shipTag = "ship";

    [Header("Slide")]
    public float minSlideSpeed = 12f;      // en az kayma hızı
    public float speedMultiplier = 1.8f;   // ship'in giriş hızını çarpar
    public bool lockY = true;

    private ShipMovement shipMove;
    private Transform shipTf;

    private bool isSliding;
    private Vector3 slideDir;
    private float slideSpeed;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(shipTag))
        {
            return;
        }

        // ShipMovement'ı ship'in kendisinde ya da parent'ta bul (child collider olabilir)
        shipMove = other.GetComponentInParent<ShipMovement>();
        if (shipMove == null)
        {
            shipMove = other.GetComponent<ShipMovement>();
        }

        if (shipMove == null)
        {
            Debug.LogWarning("IceSurfaceSlide: ShipMovement bulunamadı!");
            return;
        }

        shipTf = shipMove.transform;

        // Yön: ship'in forward'ı
        slideDir = shipTf.forward;
        if (lockY)
        {
            slideDir.y = 0f;
            slideDir.Normalize();
        }

        // Hız: girişteki speed'e göre kayma
        slideSpeed = Mathf.Max(minSlideSpeed, shipMove.CurrentSpeed * speedMultiplier);
        GameManager.Instance.IsUntouchable = true;
        // Kontrolü kapat
        shipMove.enabled = false;

        isSliding = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (!isSliding || shipTf == null)
        {
            return;
        }

        // Bazı sahnelerde child collider Stay tetikleyebilir; tag kontrolünü yine tut
        if (!other.CompareTag(shipTag))
        {
            return;
        }

        Vector3 pos = shipTf.position;
        pos += slideDir * slideSpeed * Time.deltaTime;

        if (lockY && shipMove != null)
        {
            pos.y = shipMove.fixedY; // sen zaten fixedY kullanıyorsun
        }

        shipTf.position = pos;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(shipTag))
        {
            return;
        }

        EndSlide();
    }

    private void EndSlide()
    {
        isSliding = false;

        if (shipMove != null)
        {
            shipMove.enabled = true;
        }

        GameManager.Instance.IsUntouchable = false;
        shipMove = null;
        shipTf = null;
        slideDir = Vector3.zero;
        slideSpeed = 0f;
    }
}
