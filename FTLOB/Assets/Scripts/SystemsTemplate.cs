using System.Collections.Generic;
using UnityEngine;

public class SystemsTemplate : MonoBehaviour
{    
    /// <summary>
    /// <para>list of available crew members for managing systems </para>
    /// <para>index 0 = none </para>
    /// <para>index 1 = gunner </para>
    /// <para>index 2 = shielder </para>
    /// <para>index 3 = engineer </para>
    /// <para>index 4 = scientist </para>
    /// </summary>
    /// <param name="i"></param>
    public static readonly List<string> crewList = new List<string>
        {"none",
        "gunner", 
        "shielder", 
        "engineer", 
        "scientist"};

    /// <summary>
    /// <para>list of availble anomalies for slotting into systems </para>
    /// <para>index 0 = none </para>
    /// <para>index 1 = starchild </para>
    /// <para>index 2 = tentacles </para>
    /// <para>index 3 = waldo </para>
    /// </summary>
    /// <param name="i"></param>
    public static readonly List<string> scpList = new List<string>
        {"none",
        "starchild", 
        "tentacles", 
        "waldo"};

    /// <summary>
    /// <para>list of modifiers based on what crew member is currently manning the system </para>
    /// <para>index 0 = no crew (x0.25) </para>
    /// <para>index 1 = wrong crew (x0.5) </para>
    /// <para>index 2 = correct crew (x1.0) </para>
    /// </summary>
    /// <param name="i"></param>
    public static readonly List<float> crewModList = new List<float>{0.25f, 0.5f, 1.0f};


    // the currently plugged crew member and anomaly, set to the indicies of the respective lists
    [SerializeField] protected int pluggedCrew, pluggedSCP;
    protected int preferredCrew; // index of the preferred crew member within crewList
    protected float crewMod = crewModList[2];

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /// <summary>
    /// <para>sets the slotted crew of the current system using index i of crewList </para>
    /// <para>index 0 = none </para>
    /// <para>index 1 = gunner </para>
    /// <para>index 2 = shielder </para>
    /// <para>index 3 = engineer </para>
    /// <para>index 4 = scientist </para>
    /// </summary>
    /// <param name="i"></param>

    public void SetCrew(int i)
    {
        pluggedCrew = i;

        // if there is no crew currently slotted, set the system modifier to x0.25
        if(i == 0) 
        {
            crewMod = crewModList[0];
        }
        // if the slotted crew member is the preferred crew, set the system modifier to x1.0
        else if(pluggedCrew == preferredCrew) 
        {
            crewMod = crewModList[2];
        }
        // if the slotted crew member is not the preferred crew, set the system modifier to x0.5
        else
        {
            crewMod = crewModList[1];
        }
        Debug.Log("current crewMod is: " + crewMod);
    }

    
    /// <summary>
    /// <para>sets the currently slotted anomaly as an index of scpList</para>
    /// <para>index 0 = none </para>
    /// <para>index 1 = starchild </para>
    /// <para>index 2 = tentacles </para>
    /// <para>index 3 = waldo </para>
    /// </summary>
    /// <param name="i"></param>
    public void SetSCP(int i)
    {
        pluggedSCP = i;
    }



    /// <summary>
    /// <para>sets the preferred crew of the current system using index i of crewList </para>
    /// <para>index 0 = none </para>
    /// <para>index 1 = gunner </para>
    /// <para>index 2 = shielder </para>
    /// <para>index 3 = engineer </para>
    /// <para>index 4 = scientist </para>
    /// </summary>
    /// <param name="i"></param>
    public void SetPreferredCrew(int i)
    {
        
        preferredCrew = i;
    }

    /// <summary>
    /// <para>gets the currently slotted anomaly as an index of scpList</para>
    /// <para>index 0 = none </para>
    /// <para>index 1 = starchild </para>
    /// <para>index 2 = tentacles </para>
    /// <para>index 3 = waldo </para>
    /// </summary>
    /// <param name="i"></param>
    public int GetSCP()
    {
        return pluggedSCP;
    }
    
    public float GetCrewMod()
    {
        return crewMod;
    }

    /// <summary>
    /// <para>gets the preferred crew member as an index of crewList </para>
    /// <para>index 0 = none </para>
    /// <para>index 1 = gunner </para>
    /// <para>index 2 = shielder </para>
    /// <para>index 3 = engineer </para>
    /// <para>index 4 = scientist </para>
    /// </summary>

    public int GetPreferredCrew()
    {
        return preferredCrew;
    }

    /// <summary>
    /// <para>gets the currently slotted crew member as an index of crewList </para>
    /// <para>index 0 = none </para>
    /// <para>index 1 = gunner </para>
    /// <para>index 2 = shielder </para>
    /// <para>index 3 = engineer </para>
    /// <para>index 4 = scientist </para>
    /// </summary>
    public int GetCrew()
    {
        return pluggedCrew;
    }
}
