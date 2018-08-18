
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))]
// trying to remove joint effect
//[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(WeaponManager))]

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
    private GameObject joy;
    private GameObject jump;
    private GameObject touch;
    private GameObject weapon2buttonObject;
    private GameObject weapon1buttonObject;


    public bool enter = true;

    public WeaponShoot m4Weapon;

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
    // trying to remove joint effect
    //private ConfigurableJoint joint;
    private Animator animator;
    private WeaponManager weaponManager;

    public Sprite m4Sprite; // <- This is the new sprite

    private bool hasWeapon2 = false;
    

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        // trying to remove joint effect
        //joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        weaponManager = GetComponent<WeaponManager>();
        
                
        // What the hell is this???
        //SetJointSettings(jointSpring);
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
        joy = GameObject.Find("Fixed Joystick");
        // Getting the Joystick scene gameObject
        FixedJoystick MoveJoystick = joy.GetComponent<FixedJoystick>();
        
        
        // Getting the Script component to reference the function

        //jump = GameObject.Find("FixedButton 1");


        //same than the Joystick
        //FixedButton JumpButton = jump.GetComponent<FixedButton>();

        touch = GameObject.Find("LookAxis Panel");
        //same than the Joystick
        FixedTouchField TouchField = touch.GetComponent<FixedTouchField>();




        RunAxis = MoveJoystick.inputVector;
        //JumpAxis = JumpButton.Pressed;
        LookAxis = TouchField.TouchDist;

        // if (Cursor.lockState != CursorLockMode.Locked)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //}

        // setting target position for spring makes it cool when flying over objects
        /*
        RaycastHit _hit;
        if (Physics.Raycast (transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);

        } else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
        */
        //Calculate movement velocity
        float _xMov = RunAxis.x; // Changed  Input.GetAxis("Horizontal") to RunAxis.x 
        float _zMov = RunAxis.y; // Same
    

        Vector3 _movHorizontal = transform.right * _xMov; 
        Vector3 _movVertical = transform.forward * _zMov;

        //final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        //Debug.Log("my _zMov is : " + _zMov);

        //Debug.Log("Animator name : " + animator);

        //Animate movement
        // reactivate this in order to have the animation of the thurster (I think...)
        //animator.SetFloat("ForwardVelocity", _zMov);
        animator.SetFloat("BlendY", _zMov);
        animator.SetFloat("BlendX", _xMov);

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


        // trying to remove joint effect
        /*
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
        */

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        // trying to remove joint effect
        //apply the thruster force
        //motor.ApplyThruster(_thrusterForce);


        weapon2buttonObject = GameObject.Find("Weapon 2 button");
        FixedButton weapon2FixedButton = weapon2buttonObject.GetComponent<FixedButton>();

        weapon1buttonObject = GameObject.Find("Weapon 1 button");
        FixedButton weapon1FixedButton = weapon1buttonObject.GetComponent<FixedButton>();

        if (weapon2FixedButton.Pressed & hasWeapon2)
        {
           
            weaponManager.EquipeWeapon(m4Weapon);
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("WesternAnimatorController2");
        }

        if (weapon1FixedButton.Pressed)
        {
           
            weaponManager.EquipeWeapon(weaponManager.primaryWeapon);
        }

    }
    private void OnTriggerEnter(Collider loot)
    {
        if (enter)
        {
            Debug.Log("entered");
            Destroy(loot.gameObject);

            weaponManager.hasM4 = true;

            Debug.Log("Looted m4" + weaponManager.hasM4);

            
            //FixedButton m4Button = m4B.GetComponent<FixedButton>();
            //Sprite m4BSprite  = m4B.GetComponent<Sprite>();

            Image image = GameObject.Find("Weapon 2 button").GetComponent<Image>();
            //Color imageColor = image.GetComponent<Color>();

            image.sprite = m4Sprite;
            image.color = new Color (1f,1f,1f,1f);
            
            image.preserveAspect = true;
            hasWeapon2 = true;

        }
    }



        private void SetJointSettings (float _jointSpring)
    {
        // trying to remove joint effect
        //joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}
