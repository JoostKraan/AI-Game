using UnityEngine;
using System.Collections;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeTime = 1f;
    public float destroyTime = 2f; // Adjust this to set the time before the parent object is destroyed
    public TextMeshProUGUI damageText;

    private float currentDestroyTimer;

    void Start()
    {
        damageText = GetComponent<TextMeshProUGUI>();  // Assign damageText before checking if it is null

        if (damageText == null)
        {
            Debug.LogError("TextMeshPro component not found on the DamageNumber GameObject.");
        }

        StartCoroutine(DestroyAfterTimer());
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

    IEnumerator DestroyAfterTimer()
    {
        yield return new WaitForSeconds(destroyTime);
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void SetDamageText(int damage)
    {
        damageText.text = damage.ToString();
    }
}