using UnityEngine;

public class StreetZoneTrigger : MonoBehaviour
{
    [SerializeField]
    private StreetZoneType zoneType;

    [SerializeField]
    private StreetSequenceManager sequenceManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (sequenceManager != null)
        {
            sequenceManager.EnterZone(zoneType);
        }
    }
}