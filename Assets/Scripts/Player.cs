using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;

[RequireComponent (typeof(NavMeshAgent))]
public class Player : MonoBehaviour {
    [SerializeField] private LayerMask floorLayer;
    private NavMeshAgent agent;
    
    private InputActions.PlayerActions playerActions;
    private bool isWalking = false;

    public event Action<Vector3> OnDestinationSet;
    public event Action OnArriveAtDestination;

    private Vector3 lookToWhenArrive = Vector3.zero;
    [SerializeField] private float rotationSpeed = 100f;

    private Counter currentCounter;

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
        if (!isWalking && lookToWhenArrive != Vector3.zero) {
            float angle = Mathf.Atan2(lookToWhenArrive.x - transform.position.x, lookToWhenArrive.z - transform.position.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.up);

            if (Quaternion.Angle(transform.rotation, targetRotation) > 0.2) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            } else {
                lookToWhenArrive = Vector3.zero;
            }
        }
        if (!isWalking && lookToWhenArrive == Vector3.zero && currentCounter != null) {
            currentCounter.Interact(this);
            currentCounter = null;
        }
    }

    private void SetDestination(InputAction.CallbackContext obj) {
        Vector2 clickedPosition = playerActions.Move.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(clickedPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, floorLayer)) {
            if (hit.transform.TryGetComponent<ClearCounter>(out ClearCounter clearCounter)) {
                WalkTo(clearCounter.Position);
                lookToWhenArrive = clearCounter.LookToWhenArrive;
                currentCounter = clearCounter;
            } else {
                if (hit.point != transform.position) {
                    WalkTo(hit.point);
                    currentCounter = null;
                }
            }
        }
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
