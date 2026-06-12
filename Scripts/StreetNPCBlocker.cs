using UnityEngine;

public class StreetNPCBlocker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform stopPoint;
    [SerializeField] private Rigidbody npcRigidbody;

    [Header("NPC Forms")]
    [SerializeField] private GameObject manModel;
    [SerializeField] private GameObject bikeModel;
    [SerializeField] private GameObject carModel;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 0.75f;
    [SerializeField] private float sidewaysSpeed = 10f;
    [SerializeField] private float stoppingDistance = 0.2f;

    private Vector3 forwardDirection;
    private Vector3 sidewaysDirection;
    private bool reachedEnd;

    private void Awake()
    {
        if (npcRigidbody == null)
        {
            npcRigidbody = GetComponent<Rigidbody>();
        }

        ShowMan();
    }

    private void Start()
    {
        if (stopPoint == null)
        {
            Debug.LogError(
                "NPC Stop Point is not assigned.",
                this
            );

            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogError(
                "Player is not assigned to StreetNPCBlocker.",
                this
            );

            enabled = false;
            return;
        }

        if (npcRigidbody == null)
        {
            Debug.LogError(
                "NPC Rigidbody is missing.",
                this
            );

            enabled = false;
            return;
        }

        forwardDirection =
            stopPoint.position - transform.position;

        forwardDirection.y = 0f;
        forwardDirection.Normalize();

        sidewaysDirection =
            Vector3.Cross(
                Vector3.up,
                forwardDirection
            ).normalized;

        if (forwardDirection != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(
                    forwardDirection,
                    Vector3.up
                );
        }
    }

    private void FixedUpdate()
    {
        if (reachedEnd)
        {
            return;
        }

        Vector3 currentPosition =
            npcRigidbody.position;

        Vector3 toStopPoint =
            stopPoint.position - currentPosition;

        toStopPoint.y = 0f;

        float distanceToEnd =
            toStopPoint.magnitude;

        if (toStopPoint.x <= stoppingDistance)
        {
            reachedEnd = true;

            Debug.Log(
                "NPC reached its stop point."
            );

            return;
        }

        Vector3 forwardMovement =
            forwardDirection *
            forwardSpeed *
            Time.fixedDeltaTime;

        Vector3 toPlayer =
            player.position - currentPosition;

        float sidewaysDistance =
            Vector3.Dot(
                toPlayer,
                sidewaysDirection
            );

        float sidewaysStep =
            Mathf.Clamp(
                sidewaysDistance,
                -sidewaysSpeed * Time.fixedDeltaTime,
                sidewaysSpeed * Time.fixedDeltaTime
            );

        Vector3 sidewaysMovement =
            sidewaysDirection * sidewaysStep;

        Vector3 nextPosition =
            currentPosition +
            forwardMovement +
            sidewaysMovement;

        npcRigidbody.MovePosition(nextPosition);
    }

    public void SetLaneForm(StreetLaneType lane)
    {
        switch (lane)
        {
            case StreetLaneType.Sidewalk:
                ShowMan();
                break;

            case StreetLaneType.BikeLane:
                ShowBike();
                break;

            case StreetLaneType.Road:
                ShowCar();
                break;
        }
    }

    private void ShowMan()
    {
        SetModels(true, false, false);
    }

    private void ShowBike()
    {
        SetModels(false, true, false);
    }

    private void ShowCar()
    {
        SetModels(false, false, true);
    }

    private void SetModels(
        bool showMan,
        bool showBike,
        bool showCar)
    {
        if (manModel != null)
        {
            manModel.SetActive(showMan);
        }

        if (bikeModel != null)
        {
            bikeModel.SetActive(showBike);
        }

        if (carModel != null)
        {
            carModel.SetActive(showCar);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (stopPoint == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            transform.position,
            stopPoint.position
        );

        Gizmos.DrawSphere(
            stopPoint.position,
            0.25f
        );
    }
}