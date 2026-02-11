using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class BodycamVignetteHDRP : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Assign your player object here
    public Volume volume;             // Volume with Vignette override

    [Header("Vignette Settings")]
    public float baseIntensity = 0.2f;     // Minimum intensity
    public float maxIntensity = 0.6f;      // Max intensity
    private Vignette vignette;

    [Header("Movement Sway Settings")]
    public float swayAmount = 0.1f;
    public float swaySpeed = 5f;

    [Header("Rotation Lag Settings")]
    public float rotationLagAmount = 0.2f;
    public float rotationLagSpeed = 5f;

    private CharacterController characterController;
    private float targetIntensity;
    private float currentIntensity;
    private Vector2 lastCameraEuler;

    void Start()
    {
        if (volume.profile.TryGet<Vignette>(out vignette) == false)
        {
            Debug.LogError("Vignette override not found in HDRP volume!");
        }

        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
        }
        else
        {
            Debug.LogError("Player reference not assigned!");
        }

        lastCameraEuler = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
    }

    void Update()
    {
        if (vignette == null || characterController == null) return;

        HandleMovementSway();
        HandleRotationLag();
        ApplyVignette();
    }

    void HandleMovementSway()
    {
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        float speed = horizontalVelocity.magnitude;

        targetIntensity = Mathf.Sin(Time.time * swaySpeed) * swayAmount * speed;
    }

    void HandleRotationLag()
    {
        Vector2 currentEuler = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);
        Vector2 deltaEuler = new Vector2(
            Mathf.DeltaAngle(lastCameraEuler.x, currentEuler.x),
            Mathf.DeltaAngle(lastCameraEuler.y, currentEuler.y)
        );

        targetIntensity += deltaEuler.magnitude * rotationLagAmount;
        lastCameraEuler = currentEuler;
    }

    void ApplyVignette()
    {
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * rotationLagSpeed);

        // HDRP Vignette intensity is a ClampedFloatParameter
        vignette.intensity.value = Mathf.Clamp(baseIntensity + currentIntensity, baseIntensity, maxIntensity);
    }
}
