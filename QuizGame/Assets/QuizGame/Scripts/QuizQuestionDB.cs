using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quiz/QuestionDB")]
public class QuizQuestionDB : ScriptableObject
{
    public QuizQuestion[] Questions;
}