using UnityEngine;

public class ClearCounter : BaseCounter {
    private KitchenItem placedItem = null;

    public override void Interact(Player player) {
        if (placedItem == null) {
            KitchenItem item = player.GiveItem();
            PositionItemOnCounter(item);
        } else {
            if (player.CanCarry) {
                KitchenItem newItem = Instantiate<KitchenItem>(placedItem);
                Destroy(placedItem.gameObject);
                placedItem = null;
                player.HoldItem(newItem);
            }
        }
    }

    private void PositionItemOnCounter(KitchenItem item) {
        if (item != null) {
            item.transform.parent = this.transform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            item.transform.localScale = Vector3.one * 2;
            this.placedItem = item;
        } else {
            Debug.Log("Item is null");
        }
    }
}
