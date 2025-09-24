using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    [SerializeField] private LookAtOption option;

    private void LateUpdate() {
        switch(option) {
            case LookAtOption.LookAt:
                transform.LookAt(Camera.main.transform);
                break;

            case LookAtOption.LookAtInverterd:
                Vector3 directionFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + directionFromCamera);
                break;

            case LookAtOption.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;

            case LookAtOption.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }

    private enum LookAtOption {
        LookAt,
        LookAtInverterd,
        CameraForward,
        CameraForwardInverted
    }
}
