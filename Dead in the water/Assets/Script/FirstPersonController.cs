// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1
// "Converted to new Input System" || version 1.0.2
// "Added Vault System, Fixed crouch walkSpeed drift, Fixed sprint cooldown variable conflict, Improved ground check" || version 1.0.3

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    public bool IsWalking { get; private set; }
    private float baseWalkSpeed; // FIX: Store original walk speed to prevent float drift

    // Public properties for external access
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsSprinting { get { return isSprinting; } }
    public bool IsCrouched { get { return isCrouched; } }
    public float SprintRemaining { get { return sprintRemaining; } }
    public bool IsSprintCooldown { get { return isSprintCooldown; } }

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    public float sprintRemaining { get; private set; }
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;
    private float sprintCooldownTimer; // FIX: Separate timer so we don't overwrite the inspector value

    #endregion

    #region Jump

    public bool enableJump = true;
    public float jumpPower = 5f;

    // Internal Variables
    private bool isGrounded; // FIX: Made private — use IsGrounded property externally

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    #region Vault Variables

    public bool enableVault = true;
    public float maxVaultHeight = 1.1f;
    public float vaultDuration = 0.4f;
    public float vaultDetectionRange = 1.0f;
    public LayerMask vaultableLayer;
    // In the Vault Variables region, change this line:
    [Tooltip("Height from player feet where the forward ray is cast (0-1 percentage of player height)")]
    public float vaultRayOriginHeight = 0.5f; // 50% = chest height

    [Tooltip("Height above obstacle to cast downward ray from")]
    public float vaultTopCheckHeight = 1.5f;
    [Tooltip("How far past the obstacle to check for a landing spot")]
    public float vaultLandingCheckDepth = 1.0f;

    // Internal Variables
    private bool isVaulting = false;
    public bool IsVaulting { get { return isVaulting; } }

    #endregion

    #region Ground Check Variables

    [Header("Ground Check")]
    public float groundCheckRadius = 0.3f;
    public float groundCheckExtraDistance = 0.1f; // how far below the collider bottom to check
    public LayerMask groundLayer = ~0; // Everything by default

    // Internal
    private CapsuleCollider playerCollider;

    #endregion

    // New Input System
    public PlayerInput playerInput { get; private set; }
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    // Public methods for external input access
    public Vector2 GetMoveInput() => moveAction.ReadValue<Vector2>();
    public Vector2 GetLookInput() => lookAction.ReadValue<Vector2>();
    public bool GetSprintInput() => sprintAction.IsPressed();
    public bool GetJumpInput() => jumpAction.WasPressedThisFrame();
    public bool GetCrouchInput() => crouchAction.IsPressed();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        crosshairObject = GetComponentInChildren<Image>();

        // Initialize new Input System
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            lookAction = playerInput.actions["Look"];
            moveAction = playerInput.actions["Move"];
            zoomAction = playerInput.actions["Zoom"];
            sprintAction = playerInput.actions["Sprint"];
            jumpAction = playerInput.actions["Jump"];
            crouchAction = playerInput.actions["Crouch"];
        }

        // Set internal variables
        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;
        baseWalkSpeed = walkSpeed; // FIX: Store original walk speed

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth = screenWidth * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector2(sprintBarWidth, sprintBarHeight);
            sprintBar.rectTransform.sizeDelta = new Vector2(sprintBarWidth - 2, sprintBarHeight - 2);

            if (hideBarWhenFull)
            {
                sprintBarCG.alpha = 0;
            }
        }
        else
        {
            sprintBarBG.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }

        #endregion
    }

    float camRotation;

    private void Update()
    {
        // Block all input during vault
        if (isVaulting) return;

        #region Camera

        // Control camera movement
        if (cameraCanMove)
        {
            Vector2 lookInput = lookAction.ReadValue<Vector2>();

            yaw = transform.localEulerAngles.y + lookInput.x * mouseSensitivity / 100;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity / 100 * lookInput.y;
            }
            else
            {
                pitch += mouseSensitivity / 100 * lookInput.y;
            }

            // Clamp pitch between lookAngle
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        #region Camera Zoom

        if (enableZoom)
        {
            bool zoomInput = zoomAction.IsPressed();

            // Behavior for toggle zoom
            if (zoomAction.WasPressedThisFrame() && !holdToZoom && !isSprinting)
            {
                isZoomed = !isZoomed;
            }

            // Behavior for hold to zoom
            if (holdToZoom && !isSprinting)
            {
                isZoomed = zoomInput;
            }

            // Lerps camera.fieldOfView to allow for a smooth transition
            if (isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if (!isZoomed && !isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }

        #endregion
        #endregion

        #region Sprint

        if (enableSprint)
        {
            bool sprintInput = sprintAction.IsPressed();

            if (isSprinting)
            {
                isZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                // Drain sprint remaining while sprinting
                if (!unlimitedSprint)
                {
                    sprintRemaining -= 1 * Time.deltaTime;
                    if (sprintRemaining <= 0)
                    {
                        isSprinting = false;
                        isSprintCooldown = true;
                    }
                }
            }
            else
            {
                // Regain sprint while not sprinting
                sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            }

            // Handles sprint cooldown
            // FIX: Uses separate timer variable so inspector value isn't overwritten
            if (isSprintCooldown)
            {
                sprintCooldownTimer -= 1 * Time.deltaTime;
                if (sprintCooldownTimer <= 0)
                {
                    isSprintCooldown = false;
                }
            }
            else
            {
                sprintCooldownTimer = sprintCooldown;
            }

            // Handles sprintBar
            if (useSprintBar && !unlimitedSprint)
            {
                float sprintRemainingPercent = sprintRemaining / sprintDuration;
                sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }
        }

        #endregion

        #region Jump & Vault

        if (enableJump && jumpAction.WasPressedThisFrame() && isGrounded)
        {
            // Try vault FIRST — if a vaultable obstacle is found, vault instead of jumping
            if (enableVault && TryVault())
            {
                // Vaulting! Skip the jump.
            }
            else
            {
                Jump();
            }
        }

        #endregion

        #region Crouch

        if (enableCrouch)
        {
            bool crouchInput = crouchAction.IsPressed();

            if (crouchAction.WasPressedThisFrame() && !holdToCrouch)
            {
                Crouch();
            }

            if (crouchAction.WasPressedThisFrame() && holdToCrouch)
            {
                isCrouched = false;
                Crouch();
            }
            else if (crouchAction.WasReleasedThisFrame() && holdToCrouch)
            {
                isCrouched = true;
                Crouch();
            }
        }

        #endregion

        CheckGround();

        if (enableHeadBob)
        {
            HeadBob();
        }
    }

    void FixedUpdate()
    {
        #region Movement

        // Block movement during vault
        if (isVaulting) return;

        if (playerCanMove)
        {
            // Calculate how fast we should be moving
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y);

            // Checks if player is walking and isGrounded
            // Will allow head bob
            if ((targetVelocity.x != 0 || targetVelocity.z != 0) && isGrounded)
            {
                IsWalking = true;
            }
            else
            {
                IsWalking = false;
            }

            bool sprintInput = sprintAction.IsPressed();

            // All movement calculations while sprint is active
            if (enableSprint && sprintInput && sprintRemaining > 0f && !isSprintCooldown)
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.linearVelocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                // Player is only moving when velocity change != 0
                // Makes sure fov change only happens during movement
                if (velocityChange.x != 0 || velocityChange.z != 0)
                {
                    isSprinting = true;

                    if (isCrouched)
                    {
                        Crouch();
                    }

                    if (hideBarWhenFull && !unlimitedSprint)
                    {
                        sprintBarCG.alpha += 5 * Time.deltaTime;
                    }
                }

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            // All movement calculations while walking
            else
            {
                isSprinting = false;

                if (hideBarWhenFull && sprintRemaining == sprintDuration)
                {
                    sprintBarCG.alpha -= 3 * Time.deltaTime;
                }

                targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.linearVelocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        #endregion
    }

    // FIX: Improved ground check using SphereCast — more reliable on edges and slopes
    // FIX: Ground check now starts from the actual bottom of the CapsuleCollider
    private void CheckGround()
    {
        // Calculate the true bottom of the capsule collider in world space
        // collider.center is in local space, so we transform it
        Vector3 colliderCenter = transform.TransformPoint(playerCollider.center);
        float colliderHalfHeight = (playerCollider.height * transform.localScale.y) / 2f;
        float scaledRadius = playerCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.z);

        // SphereCast origin = bottom of the capsule sphere (not the very bottom point, but the center of the bottom sphere)
        Vector3 origin = colliderCenter - Vector3.up * (colliderHalfHeight - scaledRadius);

        // Cast downward from the bottom sphere of the capsule
        float castDistance = scaledRadius + groundCheckExtraDistance;

        if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, castDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }

        // When crouched and using toggle system, will uncrouch for a jump
        if (isCrouched && !holdToCrouch)
        {
            Crouch();
        }
    }

    // FIX: Crouch no longer multiplies/divides walkSpeed directly — uses baseWalkSpeed instead
    public void Crouch()
    {
        // Stands player up to full height
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed = baseWalkSpeed; // FIX: Restore exact original speed

            isCrouched = false;
        }
        // Crouches player down to set height
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed = baseWalkSpeed * speedReduction; // FIX: Always calculate from base

            isCrouched = true;
        }
    }

    private void HeadBob()
    {
        if (IsWalking)
        {
            // Calculates HeadBob speed during sprint
            if (isSprinting)
            {
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            // Calculates HeadBob speed during crouched movement
            else if (isCrouched)
            {
                timer += Time.deltaTime * (bobSpeed * speedReduction);
            }
            // Calculates HeadBob speed during walking
            else
            {
                timer += Time.deltaTime * bobSpeed;
            }
            // Applies HeadBob movement
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when player stops moving
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }

    #region Vault System

    /// <summary>
    /// Attempts to vault over an obstacle in front of the player.
    /// Returns true if a vault was started, false if no valid vault target was found.
    /// </summary>
    private bool TryVault()
    {
        if (isVaulting) return false;
        if (isCrouched) return false;

        // === STEP 1: Forward ray from chest height — is there an obstacle? ===
        // Use the collider to find proper chest height instead of a fixed offset
        float playerHeight = playerCollider.height * transform.localScale.y;
        float feetY = transform.position.y - (playerHeight / 2f) + playerCollider.center.y * transform.localScale.y;
        float rayHeight = feetY + (playerHeight * vaultRayOriginHeight); // vaultRayOriginHeight is now a 0-1 percentage of player height

        Vector3 rayOrigin = new Vector3(transform.position.x, rayHeight, transform.position.z);
        Vector3 forward = transform.forward;

        Debug.DrawRay(rayOrigin, forward * vaultDetectionRange, Color.yellow, 2f);

        if (!Physics.Raycast(rayOrigin, forward, out RaycastHit forwardHit, vaultDetectionRange, vaultableLayer))
        {
            Debug.Log("[Vault] STEP 1 FAILED — No obstacle detected. Check: Is the obstacle on the 'Vaultable' layer? Is Detection Range long enough?");
            return false;
        }
        Debug.Log($"[Vault] STEP 1 PASSED — Hit: {forwardHit.collider.name} at distance {forwardHit.distance:F2}");

        // === STEP 2: Find the top of the obstacle ===
        Vector3 topRayOrigin = forwardHit.point + forward * 0.1f + Vector3.up * vaultTopCheckHeight;

        Debug.DrawRay(topRayOrigin, Vector3.down * (vaultTopCheckHeight + 1f), Color.cyan, 2f);

        if (!Physics.Raycast(topRayOrigin, Vector3.down, out RaycastHit topHit, vaultTopCheckHeight + 1f, vaultableLayer))
        {
            Debug.Log("[Vault] STEP 2 FAILED — Can't find top of obstacle. Try increasing Top Check Height.");
            return false;
        }
        Debug.Log($"[Vault] STEP 2 PASSED — Obstacle top at Y: {topHit.point.y:F2}");

        // === STEP 3: Validate vault height ===
        float obstacleTopY = topHit.point.y;
        float vaultHeight = obstacleTopY - feetY;

        if (vaultHeight <= 0.1f)
        {
            Debug.Log($"[Vault] STEP 3 FAILED — Obstacle too short ({vaultHeight:F2}m). Step over it instead.");
            return false;
        }
        if (vaultHeight > maxVaultHeight)
        {
            Debug.Log($"[Vault] STEP 3 FAILED — Obstacle too tall ({vaultHeight:F2}m). Max is {maxVaultHeight}m.");
            return false;
        }
        Debug.Log($"[Vault] STEP 3 PASSED — Vault height: {vaultHeight:F2}m (max: {maxVaultHeight}m)");

        // === STEP 4: Check if there's room to land on the other side ===
        Vector3 landingCheckOrigin = topHit.point + forward * vaultLandingCheckDepth + Vector3.up * 2f;

        Debug.DrawRay(landingCheckOrigin, Vector3.down * 5f, Color.magenta, 2f);

        if (!Physics.Raycast(landingCheckOrigin, Vector3.down, out RaycastHit landingHit, 5f))
        {
            Debug.Log("[Vault] STEP 4 FAILED — No ground on the other side to land on.");
            return false;
        }
        Debug.Log($"[Vault] STEP 4 PASSED — Landing at Y: {landingHit.point.y:F2}");

        // === STEP 5: Calculate landing position ===
        // FIX: Offset by half player height so feet land on the ground, not the player center
        float halfHeight = playerHeight / 2f;
        Vector3 landingPosition = landingHit.point + Vector3.up * (halfHeight + 0.05f);

        // === STEP 6: Check for walls/obstacles blocking the vault path ===
        // FIX: Prevent vaulting through walls at corners
        Vector3 peakPosition = new Vector3(
            (transform.position.x + landingPosition.x) / 2f,
            obstacleTopY + halfHeight + 0.2f, // peak must clear obstacle + player height
            (transform.position.z + landingPosition.z) / 2f
        );

        // Check from start to peak
        Vector3 startToPeak = peakPosition - transform.position;
        if (Physics.Raycast(transform.position, startToPeak.normalized, startToPeak.magnitude, groundLayer & ~vaultableLayer))
        {
            Debug.Log("[Vault] STEP 6 FAILED — Wall blocking vault path (start to peak).");
            return false;
        }

        // Check from peak to landing
        Vector3 peakToLanding = landingPosition - peakPosition;
        if (Physics.Raycast(peakPosition, peakToLanding.normalized, peakToLanding.magnitude, groundLayer & ~vaultableLayer))
        {
            Debug.Log("[Vault] STEP 6 FAILED — Wall blocking vault path (peak to landing).");
            return false;
        }

        // Also check with a SphereCast for the player's width
        float playerRadius = playerCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.z);
        if (Physics.SphereCast(peakPosition, playerRadius, peakToLanding.normalized, out _, peakToLanding.magnitude, groundLayer & ~vaultableLayer))
        {
            Debug.Log("[Vault] STEP 6 FAILED — Not enough clearance on vault path.");
            return false;
        }

        Debug.Log($"[Vault] STEP 6 PASSED — Vault path is clear.");

        // === STEP 7: Start the vault! ===
        Debug.Log("[Vault] ALL CHECKS PASSED — Vaulting!");
        StartCoroutine(VaultCoroutine(peakPosition, landingPosition));
        return true;
    }

    private IEnumerator VaultCoroutine(Vector3 peakPosition, Vector3 landingPosition)
    {
        isVaulting = true;

        // Disable physics during vault
        rb.isKinematic = true;

        Vector3 startPosition = transform.position;

        float halfDuration = vaultDuration / 2f;

        // --- Phase 1: Move UP to peak ---
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
            transform.position = Vector3.Lerp(startPosition, peakPosition, t);
            yield return null;
        }
        transform.position = peakPosition;

        // --- Phase 2: Move DOWN to landing ---
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
            transform.position = Vector3.Lerp(peakPosition, landingPosition, t);
            yield return null;
        }
        transform.position = landingPosition;

        // Re-enable physics
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        isVaulting = false;
    }

    #endregion

    #region Debug Gizmos

    private void OnDrawGizmosSelected()
    {
        // === Ground Check Gizmo ===
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            Vector3 colliderCenter = transform.TransformPoint(col.center);
            float colliderHalfHeight = (col.height * transform.localScale.y) / 2f;
            float scaledRadius = col.radius * Mathf.Max(transform.localScale.x, transform.localScale.z);
            Vector3 origin = colliderCenter - Vector3.up * (colliderHalfHeight - scaledRadius);

            // SphereCast end position (solid green)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(origin + Vector3.down * (scaledRadius + groundCheckExtraDistance), groundCheckRadius);

            // SphereCast start position (faded green) — inside the character
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawSphere(origin, groundCheckRadius);

            // Line connecting both for clarity
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + Vector3.down * (scaledRadius + groundCheckExtraDistance));
        }

        // === Vault Gizmos ===
        if (enableVault && col != null)
        {
            float playerHeight = col.height * transform.localScale.y;
            float feetY = transform.position.y - (playerHeight / 2f) + col.center.y * transform.localScale.y;
            float rayHeight = feetY + (playerHeight * vaultRayOriginHeight);

            Vector3 vaultRayOriginPos = new Vector3(transform.position.x, rayHeight, transform.position.z);

            // Forward detection ray (yellow) — this is what looks for obstacles
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(vaultRayOriginPos, vaultRayOriginPos + transform.forward * vaultDetectionRange);
            Gizmos.DrawSphere(vaultRayOriginPos, 0.05f); // small sphere at ray origin

            // Max vault height line (red)
            Gizmos.color = Color.red;
            Vector3 maxHeightPos = new Vector3(transform.position.x, feetY + maxVaultHeight, transform.position.z);
            Gizmos.DrawWireCube(maxHeightPos + transform.forward * 0.5f, new Vector3(0.5f, 0.05f, 0.5f));

            // Feet level (blue) — reference line
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                new Vector3(transform.position.x - 0.3f, feetY, transform.position.z),
                new Vector3(transform.position.x + 0.3f, feetY, transform.position.z)
            );
        }
    }

    #endregion
}

// Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
public class FirstPersonControllerEditor : Editor
{
    FirstPersonController fpc;
    SerializedObject SerFPC;

    private void OnEnable()
    {
        fpc = (FirstPersonController)target;
        SerFPC = new SerializedObject(fpc);
    }

    public override void OnInspectorGUI()
    {
        SerFPC.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Jess Case", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.3", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #region Camera Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.playerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera attached to the controller."), fpc.playerCamera, typeof(Camera), true);
        fpc.fov = EditorGUILayout.Slider(new GUIContent("Field of View", "The camera's view angle. Changes the player camera directly."), fpc.fov, fpc.zoomFOV, 179f);
        fpc.cameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), fpc.cameraCanMove);

        GUI.enabled = fpc.cameraCanMove;
        fpc.invertCamera = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), fpc.invertCamera);
        fpc.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Look Sensitivity", "Determines how sensitive the mouse movement is."), fpc.mouseSensitivity, .1f, 10f);
        fpc.maxLookAngle = EditorGUILayout.Slider(new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."), fpc.maxLookAngle, 40, 90);
        GUI.enabled = true;

        fpc.lockCursor = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), fpc.lockCursor);

        fpc.crosshair = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on, and sets it to the center of the screen."), fpc.crosshair);

        // Only displays crosshair options if crosshair is enabled
        if (fpc.crosshair)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Image", "Sprite to use as the crosshair."));
            fpc.crosshairImage = (Sprite)EditorGUILayout.ObjectField(fpc.crosshairImage, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.crosshairColor = EditorGUILayout.ColorField(new GUIContent("Crosshair Color", "Determines the color of the crosshair."), fpc.crosshairColor);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        #region Camera Zoom Setup

        GUILayout.Label("Zoom", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.enableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), fpc.enableZoom);

        GUI.enabled = fpc.enableZoom;
        fpc.holdToZoom = EditorGUILayout.ToggleLeft(new GUIContent("Hold to Zoom", "Requires the player to hold the zoom key instead of pressing to zoom and unzoom."), fpc.holdToZoom);
        fpc.zoomFOV = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), fpc.zoomFOV, .1f, fpc.fov);
        fpc.zoomStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while zooming in."), fpc.zoomStepTime, .1f, 10f);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Movement Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), fpc.playerCanMove);

        GUI.enabled = fpc.playerCanMove;
        fpc.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), fpc.walkSpeed, .1f, fpc.sprintSpeed);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #region Sprint

        GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.enableSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), fpc.enableSprint);

        GUI.enabled = fpc.enableSprint;
        fpc.unlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), fpc.unlimitedSprint);
        fpc.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), fpc.sprintSpeed, fpc.walkSpeed, 20f);

        fpc.sprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), fpc.sprintDuration, 1f, 20f);
        fpc.sprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), fpc.sprintCooldown, .1f, fpc.sprintDuration);

        fpc.sprintFOV = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the field of view the camera changes to while sprinting."), fpc.sprintFOV, fpc.fov, 179f);
        fpc.sprintFOVStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while sprinting."), fpc.sprintFOVStepTime, .1f, 20f);

        fpc.useSprintBar = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Determines if the default sprint bar will appear on screen."), fpc.useSprintBar);

        // Only displays sprint bar options if sprint bar is enabled
        if (fpc.useSprintBar)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            fpc.hideBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the sprint bar when sprint duration is full, and fades the bar in when sprinting. Disabling this will leave the bar on screen at all times when the sprint bar is enabled."), fpc.hideBarWhenFull);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
            fpc.sprintBarBG = (Image)EditorGUILayout.ObjectField(fpc.sprintBarBG, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as sprint bar foreground."));
            fpc.sprintBar = (Image)EditorGUILayout.ObjectField(fpc.sprintBar, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.sprintBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Width", "Determines the width of the sprint bar."), fpc.sprintBarWidthPercent, .1f, .5f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.sprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the sprint bar."), fpc.sprintBarHeightPercent, .001f, .025f);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Jump

        GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.enableJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), fpc.enableJump);

        GUI.enabled = fpc.enableJump;
        fpc.jumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player will jump."), fpc.jumpPower, .1f, 20f);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Crouch

        GUILayout.Label("Crouch", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.enableCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Crouch", "Determines if the player is allowed to crouch."), fpc.enableCrouch);

        GUI.enabled = fpc.enableCrouch;
        fpc.holdToCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Hold To Crouch", "Requires the player to hold the crouch key instead of pressing to crouch and uncrouch."), fpc.holdToCrouch);
        fpc.crouchHeight = EditorGUILayout.Slider(new GUIContent("Crouch Height", "Determines the y scale of the player object when crouched."), fpc.crouchHeight, .1f, 1);
        fpc.speedReduction = EditorGUILayout.Slider(new GUIContent("Speed Reduction", "Determines the percent 'Walk Speed' is reduced by. 1 being no reduction, and .5 being half."), fpc.speedReduction, .1f, 1);
        GUI.enabled = true;

        #endregion

        #region Vault

        EditorGUILayout.Space();

        GUILayout.Label("Vault", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        fpc.enableVault = EditorGUILayout.ToggleLeft(new GUIContent("Enable Vault", "Determines if the player can vault over obstacles."), fpc.enableVault);

        GUI.enabled = fpc.enableVault;
        fpc.maxVaultHeight = EditorGUILayout.Slider(new GUIContent("Max Vault Height", "Maximum height of obstacle the player can vault over."), fpc.maxVaultHeight, 0.3f, 2f);
        fpc.vaultDuration = EditorGUILayout.Slider(new GUIContent("Vault Duration", "How long the vault takes in seconds. Lower = snappier."), fpc.vaultDuration, 0.1f, 1f);
        fpc.vaultDetectionRange = EditorGUILayout.Slider(new GUIContent("Detection Range", "How far in front the player checks for vaultable obstacles."), fpc.vaultDetectionRange, 0.5f, 3f);
        fpc.vaultRayOriginHeight = EditorGUILayout.Slider(new GUIContent("Ray Height (%)", "Where the detection ray starts as a percentage of player height. 0.5 = chest height."), fpc.vaultRayOriginHeight, 0.1f, 0.9f);
        fpc.vaultTopCheckHeight = EditorGUILayout.Slider(new GUIContent("Top Check Height", "Height above hit point to cast downward ray for finding obstacle top."), fpc.vaultTopCheckHeight, 1f, 3f);
        fpc.vaultLandingCheckDepth = EditorGUILayout.Slider(new GUIContent("Landing Check Depth", "How far past the obstacle to check for valid landing ground."), fpc.vaultLandingCheckDepth, 0.5f, 3f);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Vaultable Layer", "Layer mask for objects that can be vaulted over."));
        fpc.vaultableLayer = EditorGUILayout.MaskField(UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(fpc.vaultableLayer), UnityEditorInternal.InternalEditorUtility.layers);
        fpc.vaultableLayer = UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(fpc.vaultableLayer);
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;

        #endregion

        #endregion

        #region Head Bob

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.enableHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Enable Head Bob", "Determines if the camera will bob while the player is walking."), fpc.enableHeadBob);

        GUI.enabled = fpc.enableHeadBob;
        fpc.joint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Joint", "Joint object position is moved while head bob is active."), fpc.joint, typeof(Transform), true);
        fpc.bobSpeed = EditorGUILayout.Slider(new GUIContent("Speed", "Determines how often a bob rotation is completed."), fpc.bobSpeed, 1, 20);
        fpc.bobAmount = EditorGUILayout.Vector3Field(new GUIContent("Bob Amount", "Determines the amount the joint moves in both directions on every axes."), fpc.bobAmount);
        GUI.enabled = true;

        #endregion

        #region Ground Check

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Ground Check", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.groundCheckRadius = EditorGUILayout.Slider(new GUIContent("Check Radius", "Radius of the SphereCast used for ground detection."), fpc.groundCheckRadius, 0.1f, 0.5f);
        fpc.groundCheckExtraDistance = EditorGUILayout.Slider(new GUIContent("Extra Distance", "How far below the collider bottom to check for ground."), fpc.groundCheckExtraDistance, 0.05f, 0.5f);
        fpc.groundLayer = EditorGUILayout.MaskField(new GUIContent("Ground Layer", "Which layers count as ground."), UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(fpc.groundLayer), UnityEditorInternal.InternalEditorUtility.layers);
        fpc.groundLayer = UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(fpc.groundLayer);

        #endregion

        // Sets any changes from the prefab
        if (GUI.changed)
        {
            EditorUtility.SetDirty(fpc);
            Undo.RecordObject(fpc, "FPC Change");
            SerFPC.ApplyModifiedProperties();
        }
    }
}

#endif