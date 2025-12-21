using UnityEngine;

public class PickupTracker : MonoBehaviour
{
    private HealthSpawner _spawner;

    public void Init(HealthSpawner spawner)
    {
        _spawner = spawner;
    }

    private void OnDestroy()
    {
        if(_spawner != null)
            _spawner.NotifyPickupDestroyed();
    }
}
