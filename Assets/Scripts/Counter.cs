using UnityEngine;

public interface Counter {
    public Vector3 Position { get; }
    public Vector3 LookToWhenArrive{ get; }

    public void Interact(Player player);
}
