using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BodycamVignetteHDRP : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Volume volume;
    public FirstPersonController playerController;

    [Header("Vignette Settings")]
    public float baseIntensity = 0.8f;
    public float maxIntensity = 1.0f;
    private Vignette vignette;

    [Header("Vignette Position Settings")]
    public float vignetteShiftAmount = 0.15f;
    public float vignetteReturnSpeed = 3f;

    [Header("Movement Sway Settings")]
    public float swayAmount = 0.15f;
    public float swaySpeed = 5f;

    [Header("Rotation Lag Settings")]
    public float rotationLagAmount = 0.3f;
    public float rotationLagSpeed = 2f;

    private Rigidbody rb;
    private float targetIntensity;
    private float currentIntensity;
    private float lastYaw;
    private Vector2 vignetteCenter = new Vector2(0.5f, 0.5f);
    private Vector2 targetVignetteCenter = new Vector2(0.5f, 0.5f);
    private float swayTimer = 0f;

    void Start()
    {
        if (volume == null)
            volume = GetComponent<Volume>();

        if (volume == null || volume.profile == null)
        {
            Debug.LogError("❌ Volume or Volume Profile not found!");
            return;
        }

        if (volume.profile.TryGet<Vignette>(out vignette) == false)
        {
            Debug.LogError("❌ Vignette override not found!");
            return;
        }

        vignette.intensity.overrideState = true;
        vignette.center.overrideState = true;
        vignette.roundness.overrideState = true;  // Changed from roundedCorners
        vignette.smoothness.overrideState = true;

        // Set up circular camera look
        vignette.intensity.value = baseIntensity;
        vignette.center.value = new Vector2(0.5f, 0.5f);
        vignette.roundness.value = 1.0f;  // Maximum roundness
        vignette.smoothness.value = 0.15f;     // Sharp edges

        Debug.Log("✓ Bodycam vignette initialized!");

        if (player != null)
        {
            rb = player.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("❌ Rigidbody not found on player!");
                return;
            }
        }
        else
        {
            Debug.LogError("❌ Player not assigned!");
            return;
        }

        if (playerController == null)
        {
            playerController = player.GetComponent<FirstPersonController>();
            if (playerController == null)
            {
                Debug.LogError("❌ FirstPersonController not found!");
                return;
            }
        }

        lastYaw = transform.eulerAngles.y;
        currentIntensity = baseIntensity;
        vignetteCenter = new Vector2(0.5f, 0.5f);
        targetVignetteCenter = new Vector2(0.5f, 0.5f);
    }

    void Update()
    {
        if (vignette == null || rb == null || playerController == null)
            return;

        targetIntensity = baseIntensity;
        targetVignetteCenter = new Vector2(0.5f, 0.5f);

        HandleMovementSway();
        HandleRotationLag();
        ApplyVignette();
        ApplyVignettePosition();
    }

    void HandleMovementSway()
    {
        if (playerController.IsWalking)
        {
            swayTimer += Time.deltaTime * swaySpeed;
            Vector3 velocity = rb.linearVelocity;
            float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;

            float sway = Mathf.Sin(swayTimer) * swayAmount * speed;
            targetIntensity += sway;
        }
        else
        {
            swayTimer = 0f;
        }
    }

    void HandleRotationLag()
    {
        float currentYaw = transform.eulerAngles.y;
        float yawDelta = Mathf.DeltaAngle(lastYaw, currentYaw);

        // Slight intensity change on rotation
        float rotationMagnitude = Mathf.Abs(yawDelta);
        float rotationEffect = rotationMagnitude * rotationLagAmount;
        targetIntensity += rotationEffect;

        // Shift vignette center based on rotation
        targetVignetteCenter.x = 0.5f - (yawDelta * vignetteShiftAmount);

        lastYaw = currentYaw;
    }

    void ApplyVignette()
    {
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * rotationLagSpeed);
        float finalValue = Mathf.Clamp(currentIntensity, baseIntensity, maxIntensity);

        vignette.intensity.value = finalValue;
        vignette.intensity.overrideState = true;
    }

    void ApplyVignettePosition()
    {
        vignetteCenter = Vector2.Lerp(vignetteCenter, targetVignetteCenter, Time.deltaTime * vignetteReturnSpeed);
        vignetteCenter.x = Mathf.Clamp(vignetteCenter.x, 0f, 1f);
        vignetteCenter.y = Mathf.Clamp(vignetteCenter.y, 0f, 1f);

        vignette.center.value = vignetteCenter;
        vignette.center.overrideState = true;
    }
}