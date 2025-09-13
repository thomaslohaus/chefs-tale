using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class CuttingCounter : BaseCounter {
    private Color BAR_COLOR_RUNNING = new Color(1f, 183 / 255f, 0f, 1f);
    private Color BAR_COLOR_FINISHED = new Color(0f, 116 / 255f, 0f, 1f);

    [SerializeField] private Vector3 ingredientPosition = new Vector3(0f, 0.15f, -0.04542181f);
    public Vector3 IngredientPosition { get { return ingredientPosition; } }
    
    [SerializeField] private GameObject choppingBoardPrefab;
    [SerializeField] private GameObject knifeNormalPrefab;
    [SerializeField] private GameObject knifeButcherPrefab;
    private GameObject choppingBoard, knife, butcherKnife;

    private Ingredient currentIngredient = null;
    private CuttingRecipe currentCuttingRecipe = null;
    
    private float cutProgress = 0f;
    private int cutStep = 0;
    private Timer timer;

    [SerializeField] private List<CuttingRecipe> recipes;

    private ProgressBar progressBar;

    private void Awake() {
        InstatiateAllCounterObjects();
        progressBar = GetComponentInChildren<ProgressBar>();
    }

    private void Start() {
        ResetCuttingProgress();
        CreateTimer();
    }

    public override void Interact(Player player) {
        if (currentIngredient == null) {
            // Get ingretent from player and place it on the counter
            if (TryGetIngredientFromPlayer(player)) {
                StartCutting();
            }
        } else {
            if (cutProgress == 1) {
                // Get ingretent from counter and give it to player
                if (player.CanCarry) {
                    GiveIngredientToPlayer(player);
                    ResetCuttingProgress();
                }
            } else {
                //Interrupt cutting            
            }
        }
    }

    private bool TryGetIngredientFromPlayer(Player player) {
        ReadOnlyCollection<KitchenItem> itemsInHand = player.ShowItemsInHand();

        foreach(KitchenItem item in itemsInHand) {
            Ingredient ingredient = (Ingredient)item;
            if (ingredient != null) {
                if (TrySetCurrentRecipe(ingredient.Definition)) {
                    PositionIngredientOnCounter((Ingredient)player.GiveItem(ingredient));
                    return true;
                }
            }
        }
        return false;
    }

    private void GiveIngredientToPlayer(Player player) {
        if (currentCuttingRecipe.uitlityOutput != null)
            ReplaceCurrentIngredientWithUtility();
        player.HoldItem(currentIngredient);
        currentIngredient = null;
    }

    private void ResetCuttingProgress() {
        progressBar.SetColor(BAR_COLOR_RUNNING);
        progressBar.ResetAndHide();
        cutProgress = 0f;
        cutStep = 0;
    }

    private bool TrySetCurrentRecipe(IngredientDefinition definition) {
        try {
            currentCuttingRecipe = recipes.Single(recipe => recipe.input == definition);
        } catch (InvalidOperationException) {
            return false;
        }
        return true;
    }

    private void StartCutting() {
        cutStep = 0;
        float timeToCut = currentCuttingRecipe.timeToFinalOutput;
        StartTimer(timeToCut);
    }

    private void CreateTimer() {
        if (timer == null) {
            timer = new Timer();
            timer.OnTick += CheckCuttingProgress;
            timer.OnFinish += FinishCutting;
        }
    }

    private void StartTimer(float time) {
        if (!timer.IsRunning) {
            timer.Start(time);
        }
    }

    private void FinishCutting() {
        Debug.Log($"Timer Finished");
        ReplaceCurrentIngredientWithNextStepAndPositionIt();
        cutProgress = 1f;
        progressBar.SetColor(BAR_COLOR_FINISHED);
        progressBar.UpdateProgress(cutProgress);
    }

    private void CheckCuttingProgress(float time) {
        cutProgress = time / currentCuttingRecipe.timeToFinalOutput;
        progressBar.UpdateProgress(cutProgress);

        Debug.Log($"Cut Progress: {cutProgress}, cut step: {cutStep}");

        if (cutProgress >= ((float)(cutStep + 1) / currentCuttingRecipe.outputs.Count)) {
            // next step
            ReplaceCurrentIngredientWithNextStepAndPositionIt();
        }
    }

    private void ReplaceCurrentIngredientWithNextStepAndPositionIt() {
        Ingredient newIngredient = ReplaceCurrentIngredient(currentCuttingRecipe.outputs[cutStep++]);
        PositionIngredientOnCounter(newIngredient);
    }

    private void ReplaceCurrentIngredientWithUtility() {
        Ingredient newIngredient = ReplaceCurrentIngredient(currentCuttingRecipe.uitlityOutput);
    }

    private Ingredient ReplaceCurrentIngredient(IngredientDefinition ingredientDefinition) {
        Ingredient oldIngredient = (Ingredient)currentIngredient;
        oldIngredient.enabled = false;
        Ingredient newIngredient = KitchenItemBuilder.Instance.InstanciateIngredient(ingredientDefinition);
        Destroy(oldIngredient.gameObject);
        this.currentIngredient = newIngredient;
        return newIngredient;
    }


    private void PositionIngredientOnCounter(Ingredient ingredient) {
        ingredient.transform.parent = this.transform;
        ingredient.transform.localPosition = ingredientPosition;
        ingredient.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        ingredient.transform.localScale = Vector3.one * 2;
        this.currentIngredient = ingredient;
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
        Gizmos.DrawMesh(choppingBoardPrefab.GetComponent<MeshFilter>().sharedMesh, transform.position + new Vector3(0f, 0, -0.15f), Quaternion.Euler(0, 180, 0));
    }
}
