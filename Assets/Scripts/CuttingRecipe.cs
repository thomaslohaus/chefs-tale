using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CuttingRecipe : ScriptableObject {

    public IngredientDefinition input;
    public List<IngredientDefinition> outputs;
    public float timeToFinalOutput;
}
