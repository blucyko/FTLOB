using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    //[SerializeField] public Image healthBar;
    [SerializeField] public Image[] healthPoints;
    [SerializeField] public int currentHealth, maxHealth = 25; // In GDD health should be 25
    public float lerpSpeed;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        lerpSpeed = 3f * Time.deltaTime;
        HealthBar();
    }

    void HealthBar()
    {
        //healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, lerpSpeed);
        for (int i = 0; i < healthPoints.Length; i++)
        {
            healthPoints[i].enabled = !DisplayHealth(currentHealth, i);
        }
    }

    bool DisplayHealth(int _health, int point)
    {
        return ((point * 1) >= _health);
    }

    public void Damage(int damagePoints)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damagePoints;
            Debug.Log("Ship damaged. Current health: " + currentHealth);
        }
    }

    public void Heal(int healPoints)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += healPoints;
            Debug.Log("Ship healed. Current health: " + currentHealth);
        }
    }
}
