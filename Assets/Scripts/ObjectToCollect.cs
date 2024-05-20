using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectToCollect : MonoBehaviour
{
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] TMP_Text uiTotalScore;
    [SerializeField] TMP_Text uiPoints;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] int minScore = 1; 
    [SerializeField] int maxScore = 30; 
    float startTime;
    int score;

    void Start()
    {
        ChangePosition();
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        transform.Rotate(0, 0, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            int pointsToAdd = Score();
            ShowScore(pointsToAdd);
            ChangePosition();
        }
    }

    void ChangePosition()
    {
        int randomIndex = Random.Range(0, spawnPositions.Length);
        transform.position = spawnPositions[randomIndex].position;
        startTime = Time.time;
    }

    int Score()
    {
        float timeElapsed = Time.time - startTime;
        int pointsToAdd = CalculatePointsToAdd(timeElapsed);

        score += pointsToAdd;
        return pointsToAdd;
    }

    int CalculatePointsToAdd(float timeElapsed)
    {
        float maxTime = 15f;
        float normalizedTime = Mathf.Clamp01(timeElapsed / maxTime);
        int points = Mathf.RoundToInt(maxScore - (maxScore - minScore) * normalizedTime);

        return points;
    }

    void ShowScore(int pointsToAdd)
    {
        uiTotalScore.text = "Total Score: " + score.ToString();
        uiPoints.text = "+" + pointsToAdd.ToString();
        StartCoroutine(ShowAndHideScore());
    }

    IEnumerator ShowAndHideScore()
    {
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.5f)); //Fade in
        
        yield return new WaitForSeconds(2f); //Show points and total score for 2s
        
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, 0.5f)); //Fade out
    }


    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float counter = 0f;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, counter / duration);
            yield return null;
        }
        cg.alpha = end;
    }
}
