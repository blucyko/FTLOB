using System;
using UnityEngine;

public class AnomalyStress : MonoBehaviour
{
    [Header("Anomaly Settings")]
    [SerializeField] private int maxStress = 10;
    [SerializeField] private int currentStress = 0;
    [SerializeField] private float stressDecayDelay = 25f;

    public event Action MaxStressReached;
    private float previousStressDuration;
    private bool maxStressed;

    public int CurrentStress => currentStress;
    public int MaxStress => maxStress;

    private void Start()
    {
        currentStress = 0;
        previousStressDuration = Time.time;
        maxStressed = false;
    }

    private void Update()
    {
        HandleStressDecay();
    }

    public void AddStress(int amount)
    {
        if (maxStressed || amount <= 0)
            return;
        currentStress = Mathf.Clamp(currentStress + amount, 0, maxStress);
        previousStressDuration = Time.time;
        CheckMaxStress();
    }

    public void ReduceStress(int amount)
    {
        if (amount <= 0)
            return;
        currentStress = Mathf.Clamp(currentStress - amount, 0, maxStress);
    }

    private void HandleStressDecay()
    {
        if (currentStress <= 0 || maxStressed)
            return;
        if (Time.time - previousStressDuration >= stressDecayDelay)
        {
            ReduceStress(1);
            previousStressDuration = Time.time;
        }
    }

    private void CheckMaxStress()
    {
        if (currentStress >= maxStress && !maxStressed)
        {
            maxStressed = true;
            MaxStressReached?.Invoke();
        }
    }

    public void ResetStress()
    {
        currentStress = 0;
        maxStressed = false;
        previousStressDuration = Time.time;
    }
}
