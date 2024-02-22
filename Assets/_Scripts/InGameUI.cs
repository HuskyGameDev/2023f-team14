using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    private Transform healthBar;
    [SerializeField]
    private TMP_Text ammoCount;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateHealth(float health, float maxHealth)
    {
        healthBar.localScale = new(health / maxHealth, 1f, 1f);
    }

    public void UpdateAmmo(uint ammo)
    {
        ammoCount.text = ammo.ToString();
    }
}
