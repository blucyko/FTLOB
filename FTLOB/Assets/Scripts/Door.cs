using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject door;
    //[SerializeField] private Animator doorAnimator; will make door animations later
    [SerializeField] private Sprite doorOpen;
    [SerializeField] private Sprite doorClosed;
    [SerializeField] private bool isOpen = false;

    // When player clicks on the door, toggle its state
    public void OnClick() => ToggleDoor();

    // Toggle the door's open/closed state
    public void ToggleDoor()
    {
        isOpen = !isOpen;
        Debug.Log("Door is now " + (isOpen ? "open" : "closed"));
        //doorAnimator.SetBool("IsOpen", isOpen);
        if (isOpen)
        {
            door.GetComponent<SpriteRenderer>().sprite = doorOpen;
        }
        else
        {
            door.GetComponent<SpriteRenderer>().sprite = doorClosed;
        }
    }
}
