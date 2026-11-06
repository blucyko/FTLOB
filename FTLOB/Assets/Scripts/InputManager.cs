using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Camera cam;
    void Awake()
    {
        cam = Camera.main;
    }
    
    // void OnClick(InputAction.CallbackContext context)
    // {
    //     if(!context.started)
    //     {
    //         Debug.Log("couldn't find context!");
    //         return;
    //     }

    //     var rayHit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Mouse.current.position.ReadValue()));
    //     if(!rayHit.collider)
    //     {
    //         Debug.Log("didn't hit!");
    //     }

    //     Debug.Log("Hit something: " + rayHit.collider.gameObject.name);
    // }


}
