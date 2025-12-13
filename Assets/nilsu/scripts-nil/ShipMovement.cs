using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveSpeed = 6f;        // Maksimum hız (GameManager varsa buradan override edilir)
    public float acceleration = 3f;        // Hızlanma
    public float deceleration = 4f;        // Yavaşlama
    public float turnSpeed = 80f;
    public float fixedY = 0f;

    [Header("Sway Settings")]
    public float swayAngle = 2f;
    public float swaySpeed = 1.2f;
    public float idleSwayFactor = 0.3f;

    private float currentSpeed = 0f;

    void Start()
    {
        // GameManager varsa hızını kullan (GameManager'da method adı GetSpeed)
        if (GameManager.Instance != null)
        {
            // speed 0 gelirse gemi hiç gitmesin istemiyorsan: Mathf.Max(1f, ...)
            maxMoveSpeed = GameManager.Instance.GetSpeed();
        }
    }

    void Update()
    {
        // --- INPUT ---
        float moveInput = 0f;
        float turnInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput += 1f;
            if (Keyboard.current.sKey.isPressed) moveInput -= 1f;
            if (Keyboard.current.aKey.isPressed) turnInput -= 1f;
            if (Keyboard.current.dKey.isPressed) turnInput += 1f;
        }
        maxMoveSpeed = GameManager.Instance.GetSpeed();

        // Eğer maxMoveSpeed 0 ise sallantı bölümü (divide) sorun çıkarmasın diye
        float safeMaxSpeed = Mathf.Max(0.001f, Mathf.Abs(maxMoveSpeed));

        // --- ACCELERATION / DECELERATION ---
        float targetSpeed = moveInput * maxMoveSpeed;

        float rate = Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed)
            ? acceleration
            : deceleration;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);

        // --- HAREKET ---
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        // --- DÖNÜŞ (Yaw) ---
        float turn = turnInput * turnSpeed * Time.deltaTime;
        transform.Rotate(0f, turn, 0f);

        // --- Y SABİT ---
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);

        // --- SALLANTI (hıza bağlı) ---
        float speedFactor = Mathf.Abs(currentSpeed) / safeMaxSpeed; // 0–1
        float swayMultiplier = Mathf.Clamp01(speedFactor + idleSwayFactor);
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAngle * swayMultiplier;

        Vector3 rot = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(rot.x, rot.y, sway);
    }
}
