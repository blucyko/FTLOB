using UnityEngine;
using UnityEngine.SceneManagement;
public class Node : MonoBehaviour
{
    public void LocationTravel()
    {
        // Set tag of node travelled to "Current Node" and set tag of previous node to "Node"
        if(GameObject.FindGameObjectWithTag("Current Node") != null)
        {
            GameObject previousNode = GameObject.FindGameObjectWithTag("Current Node");
            previousNode.tag = "Node";
        }
        this.gameObject.tag = "Current Node";
        Debug.Log(this.gameObject.tag);

        // Load the scene associated with this node
        SceneManager.LoadScene("Ship");
    }

    public void OnMouseExit()
    {
        // Hide undiscovered location and lines showing connected nodes
        // This can be done by disabling the relevant UI elements or changing their visibility
    }
    public void OnMouseEnter()
    {
        // Show undiscovered location and lines showing connected nodes
        // This can be done by enabling the relevant UI elements or changing their visibility
    }

    // TODO:
    // Add a method to set the tag of the node to "Node" when the player leaves the node, so that only the current node has the tag "Current Node"
    // This method can be called when the player leaves the node, for example, when they travel to another node

    // See how to make it so when a player hovers over a node, it shows undiscovered location and the lines showing connected nodes
}
