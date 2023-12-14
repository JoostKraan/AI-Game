using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeTime = 1f;
    public TextMeshProUGUI damageText; 
    void Start()
    {
        damageText = GetComponent<TextMeshProUGUI>();  // Assign damageText before checking if it is null

        if (damageText == null)
        {
            Debug.LogError("TextMeshPro component not found on the DamageNumber GameObject.");
        }

        Destroy(gameObject, fadeTime);
    }
    void Update()
    {
        MoveAndFade();
    }
    void MoveAndFade()
    {
        if (damageText != null)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, Mathf.Lerp(damageText.color.a, 0f, Time.deltaTime / fadeTime));
        }
    }

    public void SetDamageText(int damage)
    {
            damageText.text = damage.ToString();
    }
}