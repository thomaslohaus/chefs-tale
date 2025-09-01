using UnityEngine;

public class ClearCounter : MonoBehaviour, Counter {
    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private Transform prefab;


    public Vector3 Position { get { return new Vector3(transform.position.x + positionOffset.x, 0f, transform.position.z + positionOffset.y); } }
    public Vector3 LookToWhenArrive { get { return new Vector3(transform.position.x, 0f, transform.position.z); } }

    public void Interact(Player player) {
        Debug.Log("Interacted!");
        Transform foodInstance = Instantiate(prefab, transform.position, Quaternion.identity);
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Position, .1f);
    }
}
