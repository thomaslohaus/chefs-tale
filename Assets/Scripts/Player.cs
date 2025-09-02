using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent (typeof(NavMeshAgent))]
public class Player : MonoBehaviour {
    [SerializeField] private LayerMask floorLayer;
    private NavMeshAgent agent;
    
    private InputActions.PlayerActions playerActions;
    private bool isWalking = false;

    public event Action<Vector3> OnDestinationSet;
    public event Action OnArriveAtDestination;

    private Vector3 lookAtWhenArrive = Vector3.zero;
    [SerializeField] private float rotationSpeed = 200f;

    private Counter currentCounter;

    [SerializeField] private Transform palmLeft, palmRight;
    private KitchenItem itemInLeftHand = null, itemInRightHand = null;

    public bool CanCarry { get { return itemInLeftHand == null || itemInRightHand == null; } }

    private void Awake() {
        InputActions inputActions = new InputActions();
        playerActions = inputActions.Player;

        agent = GetComponent<NavMeshAgent>();
        OnDestinationSet += SetNavAgent;
    }

    private void Update() {
        if (isWalking && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            isWalking = false;
            OnArriveAtDestination?.Invoke();
        }
        if (!isWalking && lookAtWhenArrive != Vector3.zero) {
            float angle = Mathf.Atan2(lookAtWhenArrive.x - transform.position.x, lookAtWhenArrive.z - transform.position.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.up);

            if (Quaternion.Angle(transform.rotation, targetRotation) > 0.2) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            } else {
                lookAtWhenArrive = Vector3.zero;
            }
        }
        if (!isWalking && lookAtWhenArrive == Vector3.zero && currentCounter != null) {
            currentCounter.Interact(this);
            currentCounter = null;
        }
    }

    private void SetDestination(InputAction.CallbackContext obj) {
        Vector2 clickedPosition = playerActions.Move.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(clickedPosition);

        Vector3 walkDestination = Vector3.zero;
        currentCounter = null;

        foreach (RaycastHit hit in Physics.RaycastAll(ray, 100, floorLayer)) {
            if (hit.transform.TryGetComponent<Counter>(out Counter counter)) {
                walkDestination = counter.WalkDestination;
                lookAtWhenArrive = counter.LookAtWhenArrive;
                currentCounter = counter;
                break;
            } else {
                if (hit.point != transform.position) {
                    walkDestination = hit.point;
                    lookAtWhenArrive = Vector3.zero;
                    currentCounter = null;
                }
            }
        }
        if (walkDestination != Vector3.zero) {
            WalkTo(walkDestination);
        }
    }

    public void HoldItem(KitchenItem item) {
        if (item != null) {
            if (itemInLeftHand == null) {
                itemInLeftHand = item;
                item.transform.parent = palmLeft;
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
            } else if (itemInRightHand == null) {
                itemInRightHand = item;
                item.transform.parent = palmRight;
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
            } else {
                Debug.Log("Hands are full. Can't carry any more items.");
            }
        }
    }

    public KitchenItem GiveItem() {
        KitchenItem item = null;
        /*
        if (itemInLeftHand != null) {
            item = Instantiate(itemInLeftHand);
            Destroy(itemInLeftHand.gameObject);
            itemInLeftHand = null;
        } else if (itemInRightHand != null) {
            item = itemInRightHand;
            itemInLeftHand = null;
        } else {
            Debug.Log("Hands are Empty.");
        }
        */
        if (itemInLeftHand != null) {
            item = itemInLeftHand;
            itemInLeftHand = null;
        } else if (itemInRightHand != null) {
            item = itemInRightHand;
            itemInLeftHand = null;
        } else {
            Debug.Log("Hands are Empty.");
        }
        return item;
    }

    private void SetNavAgent(Vector3 destination) {
        agent.SetDestination(destination);
    }

    private void WalkTo(Vector3 destination) {
        isWalking = true;
        OnDestinationSet?.Invoke(destination);
    }

    private void OnEnable() {
        playerActions.Move.performed += SetDestination;
        playerActions.Move.Enable();
    }
    
    private void OnDisable() {
        playerActions.Move.performed -= SetDestination;
        playerActions.Move.Disable();
    }
}
