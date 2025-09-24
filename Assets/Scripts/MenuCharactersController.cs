using System.Collections.Generic;
using UnityEngine;

public class MenuCharactersController : MonoBehaviour {

    [SerializeField] private List<GameObject> characters;
    [SerializeField] private List<AnimationClip> animations;
    [SerializeField] private List<Vector3> positions;

    private List<AnimationClip> usedAnimations;
    private List<Vector3> usedPositions;

    private void Start() {
        usedAnimations = new List<AnimationClip>(animations);
        usedPositions = new List<Vector3>(positions);

        foreach (GameObject character in characters) {
            PositionAndAnimateCharacter(character);
        }
    }


    private void Awake() {
        
    }

    private void PositionAndAnimateCharacter(GameObject character) {
        int positionIndex = Random.Range(0, usedPositions.Count);
        Vector3 position = usedPositions[positionIndex];
        usedPositions.RemoveAt(positionIndex);

        int animationIndex = Random.Range(0, usedAnimations.Count);
        AnimationClip animation = usedAnimations[animationIndex];
        usedAnimations.RemoveAt(animationIndex);

        character.transform.position = position;
        Animator animator = character.GetComponent<Animator>();
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
        overrideController["Dancing 1"] = animation;
    }
}
