using UnityEngine;
using UnityEngine.InputSystem;


public class ShipMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accel = 10f;
    [SerializeField] private float turnSpeed = 5f;

    [SerializeField] private Vector2 moveVector = Vector2.zero;

    [SerializeField] private float moveThreshold = 0f;
    private Rigidbody2D rigidbody;


    private bool inCombatScreen = true;


        

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        // rigidbody.linearVelocity = Vector2.zero;
        moveSpeed = 0f;
        moveAction.action.performed += MoveShip;
        moveAction.action.canceled += StopShip;
    }
    void OnDisable()
    {
        
    }


    // Update is called once per frame
    void Update()
    {

            // moveSpeed = (Mathf.Abs(moveSpeed + moveVector.y * accel * Time.deltaTime) < maxSpeed) ? 
            // moveSpeed + moveVector.y * accel * Time.deltaTime
            // : maxSpeed * moveSpeed/Mathf.Abs(moveSpeed);

            moveSpeed = Mathf.Clamp(moveSpeed + moveVector.y * accel * Time.deltaTime, -1 * maxSpeed, maxSpeed);
        
        gameObject.transform.Rotate(0f, 0f, -1 * moveVector.x * turnSpeed * Time.deltaTime);

        rigidbody.AddForce(transform.right * moveSpeed * Time.deltaTime);

        // gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    public void ToggleInCombatScreen()
    {
        inCombatScreen = !inCombatScreen;
    }

    private void MoveShip(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }
    private void StopShip(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }


}
