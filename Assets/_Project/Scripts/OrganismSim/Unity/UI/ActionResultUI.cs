using System.Collections;
using OrganismSim.PlayerActions;
using UnityEngine;
using TMPro;

public sealed class ActionResultUI : MonoBehaviour
{
    [SerializeField] private PatientController patientController;
    [SerializeField] private TMP_Text actionResultText;
    
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 5f;
    
    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
        patientController.OnActionResult += ShowActionResult;
    }
    
    private void OnDisable()
    {
        patientController.OnActionResult -= ShowActionResult;
    }
    
    private void ShowActionResult(ActionResult result)
    {
        actionResultText.text = result.ToString();
        actionResultText.gameObject.SetActive(true);

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeActionResultText());
    }

    private IEnumerator FadeActionResultText()
    {
        Color initialColor = actionResultText.color;
        initialColor.a = 1f;
        actionResultText.color = initialColor;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            
            Color newColor = actionResultText.color;
            newColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            actionResultText.color = newColor;

            yield return null;
        }

        actionResultText.gameObject.SetActive(false);
        fadeCoroutine = null;
    }
}