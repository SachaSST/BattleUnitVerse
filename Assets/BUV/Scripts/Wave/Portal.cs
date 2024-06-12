using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    public int maxHP = 50;
    private int currentHP;
    public TextMeshPro healthText;
    private WaveManager waveManager;

    void Start()
    {
        currentHP = maxHP;
        UpdateHealthText();
    }

    public void SetWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        UpdateHealthText();

        if (currentHP <= 0)
        {
            DeactivatePortal();
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHP + "/" + maxHP;
        }
    }

    public void DeactivatePortal()
    {
        waveManager.PortalDestroyed();
        gameObject.SetActive(false); // Désactive le portail au lieu de le détruire
    }
}