using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    public Image realHpBar;
    public Image ghostHpBar;
    public float maxHp = 100f;
    public float regenPerSecond = 1f;
    private float currentHp;
    public float delayBeforeEase = 0.4f;
    public float easeDuration = 0.6f;
    private Coroutine ghostCoroutine;

    private void Start()
    {
        currentHp = maxHp;
        UpdateHpBarsInstant();
    }

    private void Update()
    {
        if (currentHp > 0f && currentHp < maxHp)
        {
            currentHp = Mathf.Clamp(currentHp + regenPerSecond * Time.deltaTime, 0f, maxHp);
            float fill = currentHp / maxHp;
            realHpBar.fillAmount = fill;

            if (ghostHpBar.fillAmount < fill)
            {
                ghostHpBar.fillAmount = fill;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHp = Mathf.Clamp(currentHp - damageAmount, 0f, maxHp);
        float targetFill = currentHp / maxHp;

        realHpBar.fillAmount = targetFill;

        if (ghostCoroutine != null)
        {
            StopCoroutine(ghostCoroutine);
        }
        ghostCoroutine = StartCoroutine(AnimateGhostBar(targetFill));
    }

    private IEnumerator AnimateGhostBar(float targetFill)
    {
        yield return new WaitForSeconds(delayBeforeEase);

        float initialFill = ghostHpBar.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < easeDuration)
        {
            elapsedTime += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsedTime / easeDuration);
            float easedT = Ease.EaseOutQuad(rawT);

            ghostHpBar.fillAmount = Mathf.Lerp(initialFill, targetFill, easedT);
            yield return null;
        }

        ghostHpBar.fillAmount = targetFill;
    }

    private void UpdateHpBarsInstant()
    {
        float fill = currentHp / maxHp;
        realHpBar.fillAmount = fill;
        ghostHpBar.fillAmount = fill;
    }
}

