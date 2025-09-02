using UnityEngine;

[CreateAssetMenu]
public class IngredientDefinition : ScriptableObject {
    public string ingredientName;
    public Transform prefab;
    public Sprite sprite;
    public Transform icon;
}
