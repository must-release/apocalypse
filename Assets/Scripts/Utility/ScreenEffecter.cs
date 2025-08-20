using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ScreenEffecter : MonoBehaviour, IUtilityTool
{
    /****** Single tone instance ******/
    public static ScreenEffecter Instance;


    /****** Private fields ******/
    private Canvas effectCanvas;
    private Image fadeImage;
    private Color fadeColor = Color.black;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            effectCanvas = transform.Find("Screen Effect Canvas").GetComponent<Canvas>();
            fadeImage = effectCanvas.transform.Find("Fade Effect Image").GetComponent<Image>();
        }
    }

    public void Start()
    {
        UtilityManager.Instance.AddUtilityTool(this);
    }

    // Stop all playing effects
    public void ResetTool()
    {
        StopAllCoroutines();
    }

    public Coroutine FadeIn(float duration)
    {
        return StartCoroutine(StartFade(1, 0, duration));
    }

    public Coroutine FadeOut(float duration)
    {
        return StartCoroutine(StartFade(0, 1, duration));
    }

    private IEnumerator StartFade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0.0f;
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, startAlpha);
        fadeImage.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, endAlpha);

        if (endAlpha == 0)
        {
            fadeImage.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}