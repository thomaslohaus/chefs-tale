using UnityEngine;

public class IngredientCounter : BaseCounter {
    [SerializeField] private IngredientDefinition ingredientDefinition;

    public override void Interact(Player player) {
        Debug.Log("Interacted with Ingredient Counter!");
        if (ingredientDefinition != null) {
            Ingredient ingredient = KitchenItemBuilder.Instance.InstanciateIngredient(ingredientDefinition);
            player.HoldItem(ingredient);
        }
    }
}
