using UnityEngine;

public class PlayerVisuals : MonoBehaviour {
    private const string IS_WALKING = "IsWalking";
    private const string LONG_WAIT = "LongWait";

    [SerializeField] private Player player;
    [SerializeField] private GameObject walkDestinationSignPrefab;
    private GameObject walkDestinationSign;


    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetBool(IS_WALKING, false);
        
        player.OnDestinationSet += Player_OnDestinationSet;
        player.OnArriveAtDestination += Player_OnArriveAtDestination;
        player.OnLongIdleWait += Player_OnLongIdleWait;

        InstantiateWalkSign();
    }

    private void InstantiateWalkSign() {
        Quaternion signRotation = Quaternion.Euler(90f, 0f, 0f);
        walkDestinationSign = Instantiate(walkDestinationSignPrefab, Vector3.zero, signRotation);
        walkDestinationSign.transform.localScale = new Vector3(1.8f, 1.8f, 1f);
        walkDestinationSign.name = "Player Walk Sign";
        HideWalkSign();
    }

    private void ShowWalkSign(Vector3 destination) {
        Vector3 signPosition = new Vector3(destination.x, 0.019f, destination.z);
        walkDestinationSign.transform.position = signPosition;
        walkDestinationSign.SetActive(true);
    }

    private void HideWalkSign() {
        walkDestinationSign.SetActive(false);
    }

    private void Player_OnLongIdleWait() {
        LongIdleWait();
    }

    private void Player_OnDestinationSet(Vector3 destination) {
        StartWalking();
        ShowWalkSign(destination);
    }

    private void Player_OnArriveAtDestination() {
        StopWalking();
        HideWalkSign();
    }

    private void OnDestroy() {
        player.OnDestinationSet -= Player_OnDestinationSet;
        player.OnArriveAtDestination -= Player_OnArriveAtDestination;
    }

    private void StartWalking() {
        animator.SetBool(IS_WALKING, true);
    }

    private void StopWalking() {
        animator.SetBool(IS_WALKING, false);
    }

    private void LongIdleWait() {
        animator.SetTrigger(LONG_WAIT);
    }

}
