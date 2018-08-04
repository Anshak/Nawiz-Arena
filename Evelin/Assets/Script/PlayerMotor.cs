
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]

public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotation = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;


    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //gets a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    //gets a rotational vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    //gets a rotational vector for the camera
    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

    //gets a vector for thruster
    public void ApplyThruster (Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    //run every physics iteration
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    //perform movement based on velocity
    void PerformMovement ()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation ()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            //set rotation and clamp it
            currentCameraRotation -= cameraRotationX;
            currentCameraRotation = Mathf.Clamp(currentCameraRotation, -cameraRotationLimit, cameraRotationLimit);

            // apply rotation to the transform of our camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotation, 0f, 0f);
        }
    }
}
