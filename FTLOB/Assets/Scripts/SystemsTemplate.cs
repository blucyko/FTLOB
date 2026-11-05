using System.Collections.Generic;
using UnityEngine;

public class SystemsTemplate : MonoBehaviour
{
    // list of available crew memebers for managing systems
    private List<string> crewList = new List<string>
        {"gunner", 
        "shielder", 
        "engineer", 
        "scientist"};

    // list of availble anomalies for slotting into systems
    private List<string> scpList = new List<string>
        {"none",
        "starchild", 
        "tentacles", 
        "waldo"};

    // list of modifiers based on what crew member is currently manning the system:
    // index 0 = 25% for no crew, index 1 = 50% for wrong crew, index 2 = 100% for correct crew
    private static List<float> crewModList = new List<float>{0.25f, 0.5f, 1.0f};


    [SerializeField] private string pluggedCrew = "", pluggedSCP = "none";
    private string preferredCrew;
    private float crewMod;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCrew(pluggedCrew);
    }

    public void SetCrew(string newCrew)
    {
        pluggedCrew = newCrew;
        if(!crewList.Contains(pluggedCrew))
        {
            crewMod = crewModList[0];
        }
        else if(pluggedCrew.Equals(preferredCrew))
        {
            crewMod = crewModList[2];
        }
        else
        {
            crewMod = crewModList[1];
        }
        Debug.Log("current crewMod is: " + crewMod);
    }

    public void SetSCP(string newSCP)
    {
        pluggedSCP = newSCP;
    }
}
