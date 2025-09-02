using UnityEngine;

public class KitchenItemBuilder : MonoBehaviour {

    public static KitchenItemBuilder Instance { get; private set; }

    [SerializeField] private Ingredient ingredientPrefab;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Instance");
        }
        Instance = this;
    }

    public Ingredient InstanciateIngredient(IngredientDefinition definition) {
        Ingredient ingredient = Instantiate<Ingredient>(ingredientPrefab);
        ingredient.name = definition.ingredientName;
        ingredient.Definition = definition;

        Transform ingredientVisuals = Transform.Instantiate(definition.prefab, ingredient.transform, false);
        ingredientVisuals.name = "Visuals";
        ingredientVisuals.localPosition = Vector3.zero;
        ingredientVisuals.localRotation = Quaternion.identity;
        return ingredient;
    }

}
