using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CuttingRecipe : ScriptableObject {

    [Tooltip("Ingredient to be cut")]
    public IngredientDefinition input;
    [Tooltip("All ingredient cutting steps, intermediate and final")]
    public List<IngredientDefinition> outputs;
    [Tooltip("If the ingredient used in recipies should look different than the final cutting step. Leave blank if not needed")]
    public IngredientDefinition uitlityOutput;
    [Tooltip("Time, in s, to cut through all steps.")]
    public float timeToFinalOutput;
    [Tooltip("Time, in s, which the ingredient rottens if left half cut.")]
    public float timeToRotten;
}
