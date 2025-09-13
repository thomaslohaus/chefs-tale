using UnityEngine;

public class PlayerVisuals : MonoBehaviour {
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player player;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetBool(IS_WALKING, false);

        player.OnDestinationSet += Player_OnDestinationSet;
        player.OnArriveAtDestination += Player_OnArriveAtDestination;
    }

    private void Player_OnDestinationSet(Vector3 obj) {
        animator.SetBool(IS_WALKING, true);
    }

    private void Player_OnArriveAtDestination() {
        animator.SetBool(IS_WALKING, false);
    }

    private void OnDestroy() {
        player.OnDestinationSet -= Player_OnDestinationSet;
        player.OnArriveAtDestination -= Player_OnArriveAtDestination;
    }
}
