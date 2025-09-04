using System;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter {
    [SerializeField] private Vector3 ingredientPosition = new Vector3(0f, 0.15f, -0.04542181f);
    public Vector3 IngredientPosition { get { return ingredientPosition; } }
    
    [SerializeField] private GameObject choppingBoardPrefab;
    [SerializeField] private GameObject knifeNormalPrefab;
    [SerializeField] private GameObject knifeButcherPrefab;
    private GameObject choppingBoard, knife, butcherKnife;

    private KitchenItem placedItem = null;

    private CuttingRecipe currentCuttingRecipe = null;
    private float cutProgress = 0f;
    private int cutStep = 0;

    [SerializeField] private List<CuttingRecipe> recipes;

    private void Awake() {
        InstatiateAllCounterObjects();
    }

    public override void Interact(Player player) {
        if (placedItem == null) {
            Ingredient ingredient = (Ingredient)player.GiveItem();
            if (ingredient != null) {
                PositionIngredient(ingredient);

                SetCurrentRecipe(ingredient.Definition);
            } else {
                Debug.Log("Item is null");
            }
        } else {
            if (cutProgress == 1) {
                if (player.CanCarry) {
                    KitchenItem newItem = Instantiate<KitchenItem>(placedItem);
                    Destroy(placedItem.gameObject);
                    placedItem = null;
                    player.HoldItem(newItem);
                }
            } else {
                cutProgress += 1f / recipes[0].outputs.Count;
                Ingredient currentIngredient = (Ingredient)placedItem;
                currentIngredient.enabled = false;
                Ingredient newIngredient = KitchenItemBuilder.Instance.InstanciateIngredient(currentCuttingRecipe.outputs[cutStep++]);
                PositionIngredient(newIngredient);
                Destroy(currentIngredient.gameObject);
            }
        }
    }

    private void SetCurrentRecipe(IngredientDefinition definition) {
        currentCuttingRecipe = recipes[0];
    }

    private void PositionIngredient(Ingredient ingredient) {
        ingredient.transform.parent = this.transform;
        ingredient.transform.localPosition = ingredientPosition;
        ingredient.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        ingredient.transform.localScale = Vector3.one;
        this.placedItem = ingredient;
    }

    private void InstatiateAllCounterObjects() {
        choppingBoard = InstantiateCounterObject("Chopping Board", 
            choppingBoardPrefab,
            new Vector3(0f, 0.15f, 0),
            Quaternion.Euler(-90, 0, 0));

        knife = InstantiateCounterObject("Knife",
            knifeNormalPrefab,
            new Vector3(-0.3f, -0.27f, -0.01f),
            Quaternion.Euler(0, 90, -180));

        butcherKnife = InstantiateCounterObject("Butcher Knife", 
            knifeButcherPrefab,
            new Vector3(0.1f, -0.36f, -0.01f),
            Quaternion.Euler(0, 90, -180));
    }

    private GameObject InstantiateCounterObject(string name, GameObject prefab, Vector3 localPosition, Quaternion localRotation) {
        GameObject counterObject = Instantiate(prefab);
        counterObject.name = name;
        counterObject.transform.parent = transform;
        counterObject.transform.localPosition = localPosition;
        counterObject.transform.localRotation = localRotation;

        return counterObject;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(WalkDestination, .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(LookAtWhenArrive, .05f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(IngredientPosition, .05f);
    }
}
