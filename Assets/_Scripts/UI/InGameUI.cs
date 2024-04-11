using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private TMP_Text healthText;
    [SerializeField]
    private TMP_Text ammoCount;

    private float hbSize;
    private float healthBarVOffset;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        hbSize = healthBar.GetComponent<RectTransform>().rect.width;
        healthBarVOffset = healthBar.transform.localPosition.y;
    }

    public void UpdateHealth(float health, float maxHealth)
    {
        if (health <= 0) { health = maxHealth; }
        healthBar.transform.localPosition = new Vector3(-1 * ((maxHealth - health) / 100f) * hbSize, healthBarVOffset);
        healthText.text = ((int)health).ToString();
    }

    public void UpdateAmmo(uint ammo)
    {
        ammoCount.text = ammo.ToString();
    }
}
