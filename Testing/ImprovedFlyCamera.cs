using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ImprovedFlyCamera : MonoBehaviour
{
    [SerializeField] Camera cam;

    #region UI    

    [Space]

    [SerializeField]
    [Tooltip("The script is currently active")]
    private bool _active = true;

    [Space]

    [SerializeField]
    [Tooltip("Camera rotation by mouse movement is active")]
    private bool _enableRotation = true;

    [SerializeField]
    [Tooltip("Sensitivity of mouse rotation")]
    private float _mouseSense = 1.8f;

    [Space]

    [SerializeField]
    [Tooltip("Camera zooming in/out by 'Mouse Scroll Wheel' is active")]
    private bool _enableTranslation = true;

    [SerializeField]
    [Tooltip("Velocity of camera zooming in/out")]
    private float _translationSpeed = 24f;

    [Space]

    [SerializeField]
    [Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
    private bool _enableMovement = true;

    [SerializeField]
    [Tooltip("Camera movement speed")]
    private float _movementSpeed = 8f;

    [SerializeField]
    [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
    private float _boostedSpeed = 16f;

    [SerializeField]
    [Tooltip("Boost speed")]
    private KeyCode _boostSpeed = KeyCode.Tab;

    [SerializeField]
    [Tooltip("Move up")]
    private KeyCode _moveUp = KeyCode.Space;

    [SerializeField]
    [Tooltip("Move down")]
    private KeyCode _moveDown = KeyCode.LeftShift;

    [Space]

    [SerializeField]
    [Tooltip("Acceleration at camera movement is active")]
    private bool _enableSpeedAcceleration = true;

    [SerializeField]
    [Tooltip("Rate which is applied during camera movement")]
    private float _speedAccelerationFactor = 1.5f;

    [Space]

    [SerializeField]
    [Tooltip("This keypress will move the camera to initialization position")]
    private KeyCode _initPositonButton = KeyCode.R;

    #endregion UI

    private CursorLockMode _wantedMode;

    private float _currentIncrease = 1;
    private float _currentIncreaseMem = 0;


    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    private void Update()
    {
        if (!_active)

        // Translation
        if (_enableTranslation)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed);
        }

        // Movement
        if (_enableMovement)
        {
            Vector3 deltaPosition = Vector3.zero;
            float currentSpeed = _movementSpeed;

            Vector3 eulerRot = cam.transform.eulerAngles;
            eulerRot.x = 0; eulerRot.z = 0;
            Quaternion heading;
            if (eulerRot.magnitude > 0)
            {
                heading = Quaternion.Euler(eulerRot);
            } 
            else
            {
                heading = Quaternion.Euler(Vector3.zero);
            }

            if (Input.GetKey(_boostSpeed))
                currentSpeed = _boostedSpeed;

            if (Input.GetKey(KeyCode.W))
                deltaPosition += heading * Vector3.forward;

            if (Input.GetKey(KeyCode.S))
                deltaPosition -= heading * Vector3.forward;

            if (Input.GetKey(KeyCode.A))
                deltaPosition -= heading * Vector3.right;

            if (Input.GetKey(KeyCode.D))
                deltaPosition += heading * Vector3.right;

            if (Input.GetKey(_moveUp))
                deltaPosition += heading * Vector3.up;

            if (Input.GetKey(_moveDown))
                deltaPosition -= heading * Vector3.up;

            // Calc acceleration
            CalculateCurrentIncrease(deltaPosition != Vector3.zero);

            transform.position += deltaPosition * currentSpeed * _currentIncrease;
        }

        // Rotation
        if (_enableRotation)
        {
            // Pitch
            cam.transform.rotation *= Quaternion.AngleAxis(
                -Input.GetAxis("Mouse Y") * _mouseSense,
                Vector3.right
            );

            // Paw
            cam.transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense,
                transform.eulerAngles.z
            );
        }
    }
}
