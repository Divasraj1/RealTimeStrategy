using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitProjectile :NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float destroyAfterSeconds = 3f;
    [SerializeField] private float LaunchForce = 10f;
    [SerializeField] private int damageToDeal = 20;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * LaunchForce;
    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if(networkIdentity.connectionToClient == connectionToClient) { return; }
        }
        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }
        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
