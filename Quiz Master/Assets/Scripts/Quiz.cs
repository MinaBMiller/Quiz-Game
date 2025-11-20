using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    int correctAnswerIndex;
    bool hasAnsweredEarly;
    bool hasDisplayedAnswer = false;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;
    UnityEngine.UI.Image buttonImage;

    [Header("Timer")]
    [SerializeField] UnityEngine.UI.Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("Progress Bar")]
    [SerializeField] Slider progressBar;
    public bool isComplete;
    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    void Update()
{
    timerImage.fillAmount = timer.fillFraction;
    if (timer.loadNextQuestion)
    {
            if (progressBar.value == progressBar.maxValue)
            {
                isComplete = true;
                return;  
        }

        hasAnsweredEarly = false;
        hasDisplayedAnswer = false;  // Reset the flag
        timer.loadNextQuestion = false;
        GetNextQuestion();
    }
    else if (!hasAnsweredEarly && !timer.isAnsweringQuestion && !hasDisplayedAnswer)  // Add the check here
    {
        if (currentQuestion != null)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
            hasDisplayedAnswer = true;  // Set the flag so it doesn't run again
        }
    }
}

    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly = true;
        hasDisplayedAnswer = true;  // Also set it here
        DisplayAnswer(index);

        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
        
}

    void DisplayAnswer(int index)
    {
        if (index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct!";
            scoreKeeper.IncrementCorrectAnswers();
        }
        else
        {
            questionText.text = "Wrong! The correct asnwer is:\n " + currentQuestion.GetAnswer(currentQuestion.GetCorrectAnswerIndex());

        }

        buttonImage = answerButtons[correctAnswerIndex].GetComponent<UnityEngine.UI.Image>();
        buttonImage.sprite = correctAnswerSprite;
    }
    void GetNextQuestion()
{
        if (questions.Count > 0)
        {

            SetButtonState(true);
            SetDefaultButtonSprite();
            GetRandomQuestion();
            DisplayQuestion();
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
    }
        else
        {
            Debug.Log("No questions left!");
        }
}

    void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];
        correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();

        if (questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
        
    }

    void DisplayQuestion()
{
    Debug.Log("=== DisplayQuestion START ===");
    Debug.Log("Current question: " + (currentQuestion != null ? currentQuestion.GetQuestion() : "NULL"));
    
    questionText.text = currentQuestion.GetQuestion();
    Debug.Log("Question text set successfully");

    for (int i = 0; i < answerButtons.Length; i++)
    {
        Debug.Log($"Processing button {i}: {answerButtons[i].name}");
        
        TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        
        if (buttonText != null)
        {
            string answerText = currentQuestion.GetAnswer(i);
            Debug.Log($"Answer text for button {i}: '{answerText}'");
            buttonText.text = answerText;
            Debug.Log($"Button {i} text NOW reads: '{buttonText.text}'");
        }
        else
        {
            Debug.LogError($"Button {i} has no TextMeshProUGUI child!");
        }
    }
    Debug.Log("=== DisplayQuestion END ===");
}

    void SetButtonState(bool state)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    void SetDefaultButtonSprite()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            buttonImage = answerButtons[i].GetComponent<UnityEngine.UI.Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }


    }

}
