using UnityEngine;

public class WeaponsTemplate : SystemsTemplate
{
    void Start()
    {
        SetPreferredCrew(1);
        Debug.Log("plugged SCP is:" + this.GetSCP());
    }
}
