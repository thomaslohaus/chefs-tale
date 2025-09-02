using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, Counter {
    [SerializeField] private Vector2 walkDestinationOffset;
    [SerializeField] private Vector3 lookAtOffset;

    public Vector3 WalkDestination {
        get { return new Vector3(transform.position.x + walkDestinationOffset.x, 0f, transform.position.z + walkDestinationOffset.y); }
    }
    public Vector3 LookAtWhenArrive {
        get { return new Vector3(transform.position.x + lookAtOffset.x, 0f + lookAtOffset.y, transform.position.z + lookAtOffset.z); }
    }

    public virtual void Interact(Player player) {
        Debug.Log("Interacted with Base Counter!");
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(WalkDestination, .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(LookAtWhenArrive, .05f);
    }
}
