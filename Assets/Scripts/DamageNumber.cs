using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float fadeSpeed = 0.5f;

    private TMP_Text damageText;

    private void Start()
    {
        damageText = GetComponent<TMP_Text>();
        Destroy(gameObject, 2.0f); // Destroy the damage number after 1 second
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        Color textColor = damageText.color;
        textColor.a -= fadeSpeed * Time.deltaTime;
        damageText.color = textColor;
    }
}
