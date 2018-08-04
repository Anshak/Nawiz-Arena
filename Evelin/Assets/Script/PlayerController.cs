
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

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

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

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
        float _xMov = CrossPlatformInputManager.GetAxis("Horizontal"); //changed Input to CrossPlatformInputManager
        float _zMov = CrossPlatformInputManager.GetAxis("Vertical"); //changed Input to CrossPlatformInputManager

        Vector3 _movHorizontal = transform.right * _xMov; 
        Vector3 _movVertical = transform.forward * _zMov;

        //final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        //Animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        //apply movement
        motor.Move(_velocity);

        
        //calculate rotation as a 3d vector (turning around)
        float _yRot = CrossPlatformInputManager.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        //apply rotation
        motor.Rotate(_rotation);

        //calculate camera rotation as a 3d vector (look up and down)
        float _xRot = CrossPlatformInputManager.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * lookSensitivity;

        motor.RotateCamera(_cameraRotationX);

    

        //calculate thruster force based on player input
        Vector3 _thrusterForce = Vector3.zero;
        
        if (CrossPlatformInputManager.GetButton("Jump") && thrusterFuelAmount > 0f)//changed Input to CrossPlatformInputManager
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
