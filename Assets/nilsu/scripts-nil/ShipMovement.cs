using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ShipMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveSpeed = 6f;
    public float acceleration = 3f;
    public float deceleration = 4f;
    public float fixedY = 0f;

    [Header("Turning")]
    public float maxTurnSpeed = 80f;

    [Header("Turning Inertia")]
    public float turnAcceleration = 120f;
    public float turnDeceleration = 160f;
    public float minTurnFactor = 0.3f;

    [Header("Sway Settings")]
    public float swayAngle = 2f;
    public float swaySpeed = 1.2f;
    public float idleSwayFactor = 0.3f;

    private float currentSpeed = 0f;
    private float currentTurnSpeed = 0f; // 🔥 kavisli dönüşün anahtarı

    void Start()
    {
        if (GameManager.Instance != null)
            maxMoveSpeed = GameManager.Instance.GetSpeed();
    }

    void Update()
    {
        // ================= INPUT =================
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
        float safeMaxSpeed = Mathf.Max(0.001f, maxMoveSpeed);

        // ================= SPEED INERTIA =================
        float targetSpeed = moveInput * maxMoveSpeed;
        float speedRate = Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed)
            ? acceleration
            : deceleration;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            speedRate * Time.deltaTime
        );

        // ================= MOVE =================
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        // ================= TURN INERTIA (KAVİS) =================
        float speedFactor = Mathf.Abs(currentSpeed) / safeMaxSpeed;
        float turnFactor = Mathf.Lerp(minTurnFactor, 1f, speedFactor);

        float targetTurnSpeed = turnInput * maxTurnSpeed * turnFactor;

        float turnRate = Mathf.Abs(targetTurnSpeed) > Mathf.Abs(currentTurnSpeed)
            ? turnAcceleration
            : turnDeceleration;

        currentTurnSpeed = Mathf.MoveTowards(
            currentTurnSpeed,
            targetTurnSpeed,
            turnRate * Time.deltaTime
        );

        transform.Rotate(0f, currentTurnSpeed * Time.deltaTime, 0f);

        // ================= FIX Y =================
        transform.position = new Vector3(
            transform.position.x,
            fixedY,
            transform.position.z
        );

        // ================= SWAY =================
        float swayMultiplier = Mathf.Clamp01(speedFactor + idleSwayFactor);
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAngle * swayMultiplier;

        Vector3 rot = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(rot.x, rot.y, sway);
    }
    public float CurrentSpeed
    {
        get { return currentSpeed; }
    }
    private bool isKnockbacked;

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        StartCoroutine(KnockbackRoutine(direction, force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force, float duration)
    {
        isKnockbacked = true;

        // ShipMovement input ve kontrolü kilitle
        enabled = false;

        float elapsed = 0f;

        direction.y = 0f;
        direction.Normalize();

        while (elapsed < duration)
        {
            transform.position += direction * force * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        enabled = true;
        isKnockbacked = false;
    }


}
