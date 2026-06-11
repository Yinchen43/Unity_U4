using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FootstepLoop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource footstepSource;

    [Header("Movement Detection")]
    [SerializeField] private float minimumMovementSpeed = 0.2f;

    [Header("Walking")]
    [SerializeField] private float walkingPitch = 1f;
    [SerializeField] private float walkingVolume = 0.55f;

    [Header("Running")]
    [SerializeField] private float runningPitch = 1.25f;
    [SerializeField] private float runningVolume = 0.7f;

    private CharacterController characterController;
    private Vector3 previousPosition;

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController>();

        previousPosition = transform.position;

        if (footstepSource == null)
        {
            Debug.LogError(
                "Footstep AudioSource has not been assigned.",
                this
            );
            return;
        }

        footstepSource.playOnAwake = false;
        footstepSource.loop = true;
        footstepSource.spatialBlend = 0f;
    }

    private void OnEnable()
    {
        previousPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (footstepSource == null)
        {
            return;
        }

        // Measure how far the player actually moved this frame.
        Vector3 movement =
            transform.position - previousPosition;

        previousPosition = transform.position;

        // Ignore jumping and falling.
        movement.y = 0f;

        float horizontalSpeed =
            movement.magnitude /
            Mathf.Max(Time.deltaTime, 0.0001f);

        bool isActuallyMoving =
            horizontalSpeed > minimumMovementSpeed;

        bool shouldPlay =
            isActuallyMoving &&
            characterController.isGrounded;

        bool isRunning =
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed;

        if (shouldPlay)
        {
            footstepSource.pitch =
                isRunning
                    ? runningPitch
                    : walkingPitch;

            footstepSource.volume =
                isRunning
                    ? runningVolume
                    : walkingVolume;

            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else if (footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }

    private void OnDisable()
    {
        if (footstepSource != null)
        {
            footstepSource.Stop();
        }
    }
}