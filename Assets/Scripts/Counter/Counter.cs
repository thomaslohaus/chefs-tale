using UnityEngine;

public interface Counter {
    public Vector3 WalkDestination { get; }
    public Vector3 LookAtWhenArrive{ get; }

    public void Interact(Player player);
}
