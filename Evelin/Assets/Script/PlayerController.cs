
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 1f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;

    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    [HideInInspector]
    public Vector2 RunAxis;
    [HideInInspector]
    public bool JumpAxis;
    [HideInInspector]
    public Vector2 LookAxis;

    //public FixedJoystick MoveJoystick;
    // public FixedButton JumpButton;
    // public FixedTouchField TouchField;
    public GameObject joy;
    public GameObject jump;
    public GameObject touch;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring Settings:")]
  
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        SetJointSettings(jointSpring);
    }

    private void Update()
    {

        if (PauseMenu.IsOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);
                return;
        }

        //var MoveJoystick = GameObject.FindWithTag("MoveJoystick");

        // Getting the Joystick scene gameObject
        joy = GameObject.Find("Fixed Joystick");
        // Getting the Script component to reference the function
        FixedJoystick MoveJoystick = joy.GetComponent<FixedJoystick>();

        //same than the Joystick
        jump = GameObject.Find("FixedButton 1");
        FixedButton JumpButton = jump.GetComponent<FixedButton>();

        //same than the Joystick
        touch = GameObject.Find("LookAxis Panel");
        FixedTouchField TouchField = touch.GetComponent<FixedTouchField>();

        RunAxis = MoveJoystick.inputVector;
        JumpAxis = JumpButton.Pressed;
        LookAxis = TouchField.TouchDist;

        // if (Cursor.lockState != CursorLockMode.Locked)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //}

        // setting target position for spring makes it cool when flying over objects
        RaycastHit _hit;
        if (Physics.Raycast (transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);

        } else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        //Calculate movement velocity
        float _xMov = RunAxis.x; // Changed  Input.GetAxis("Horizontal") to RunAxis.x 
        float _zMov = RunAxis.y; // Same
    

        Vector3 _movHorizontal = transform.right * _xMov; 
        Vector3 _movVertical = transform.forward * _zMov;

        //final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        //Animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        //apply movement
        motor.Move(_velocity);


        //calculate rotation as a 3d vector (turning around)
        float _yRot = LookAxis.x; // Changed Input.GetAxisRaw("Mouse X") to LookAxis.x

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        //apply rotation
        motor.Rotate(_rotation);

        //calculate camera rotation as a 3d vector (look up and down)
        float _xRot = LookAxis.y; // Changed Input.GetAxisRaw("Mouse Y") to LookAxis.y

        float _cameraRotationX = _xRot * lookSensitivity;

        motor.RotateCamera(_cameraRotationX);

    

        //calculate thruster force based on player input
        Vector3 _thrusterForce = Vector3.zero;
        
        if (JumpAxis && thrusterFuelAmount > 0f)//changed Input.GetButton("Jump")  to JumpAxis
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount > 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);

            }

        } else
        {

            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;

            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //apply the thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings (float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}
