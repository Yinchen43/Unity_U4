using UnityEngine;

public class FoyerLiftTrigger : MonoBehaviour
{
    [SerializeField]
    private FoyerSequenceManager sequenceManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (sequenceManager != null)
        {
            sequenceManager.PlayerEnteredLift();
        }
    }
}