using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private List<KitchenItem> itemsInHand;
    private int capacity = 2;

    public bool CanCarry { get { return itemsInHand.Count < capacity; } }

    private void Awake() {
        InputActions inputActions = new InputActions();
        playerActions = inputActions.Player;

        agent = GetComponent<NavMeshAgent>();
        OnDestinationSet += SetNavAgent;

        itemsInHand = new List<KitchenItem>();
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
        if (item != null && itemsInHand.Count < capacity) {
            itemsInHand.Add(item);

            if (itemsInHand.Count == 1)
                item.transform.parent = palmLeft;
            else
                item.transform.parent = palmRight;

            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        } else {
            Debug.Log("Hands are full. Can't carry any more items.");
        }
    }

    public ReadOnlyCollection<KitchenItem> ShowItemsInHand() {
        ReadOnlyCollection<KitchenItem> kitchenItems = new(itemsInHand);
        return kitchenItems;
    }

    public KitchenItem GiveItem(KitchenItem desiredItem) {
        int index = itemsInHand.IndexOf(desiredItem);
        KitchenItem item = null;

        if (index == 0) {
            item = GiveItemInLeftHand();
        } else if (index == 1) {
            item = itemsInHand[1];
            itemsInHand.RemoveAt(1);
        } else {
            Debug.Log("Hands are Empty.");
        }
        
        return item;
    }

    public KitchenItem GiveAnyItem() {
        KitchenItem item = null;
        if (itemsInHand.Count > 0) {
            item = GiveItemInLeftHand();
        } else {
            Debug.Log("Hands are Empty.");
        }
        return item;
    }

    private KitchenItem GiveItemInLeftHand() {
        KitchenItem item = itemsInHand[0];
        itemsInHand.RemoveAt(0);
        if (itemsInHand.Count == 1) {
            Transform itemRemaining = palmRight.GetChild(0);
            itemRemaining.parent = palmLeft;
            itemRemaining.localPosition = Vector3.zero;
            itemRemaining.localRotation = Quaternion.identity;

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
