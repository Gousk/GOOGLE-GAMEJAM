using UnityEngine;
using System.Collections;

public class WellDoneScreenManager : MonoBehaviour
{
    [SerializeField] Star[] Stars;

    [SerializeField] float EnlargeScale = 1.5f;
    [SerializeField] float ShrinkScale = 1.0f;
    [SerializeField] float EnlargeDuration = 0.25f;
    [SerializeField] float ShrinkDuration = 0.25f;


    public void ShowStars(int numberOfStars)
    {
        StartCoroutine(ShowStarRoutine(numberOfStars));
    }

    private IEnumerator ShowStarRoutine(int numberOfStars)
    {
        foreach (Star star in Stars)
        {
            star.YellowStar.transform.localScale = Vector3.zero;
        }

        for (int i = 0; i < numberOfStars; i++)
        {
            yield return StartCoroutine(EnlargeAndShrinkStar(Stars[i]));
        }
    }

    private IEnumerator EnlargeAndShrinkStar(Star star)
    {
        yield return StartCoroutine(ChangeStarScale(star, EnlargeScale, EnlargeDuration));
        yield return StartCoroutine(ChangeStarScale(star, ShrinkScale, ShrinkDuration));
    }


    private IEnumerator ChangeStarScale(Star star, float targetScale, float duration)
    {
        Vector3 initialScale = star.YellowStar.transform.localScale;
        Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            star.YellowStar.transform.localScale = Vector3.Lerp(initialScale, finalScale, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        star.YellowStar.transform.localScale = finalScale;
    }

    private void Start()
    {
        ShowStars(5);
    }

    
}
