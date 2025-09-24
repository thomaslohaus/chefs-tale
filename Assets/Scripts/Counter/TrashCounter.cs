using UnityEngine;

public class TrashCounter : BaseCounter {

    public override void Interact(Player player) {
        KitchenItem ki = player.GiveAnyItem();
        if (ki != null)
            Destroy(ki.gameObject);
    }
}
