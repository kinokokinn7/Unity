// QuizQuestion.cs
using System;
using UnityEngine;

[Serializable]
public class QuizQuestion
{
    public string Id;
    [TextArea] public string Question;
    public string[] Choices = new string[4];
    [Range(0, 3)] public int CorrectIndex;
    public string ExamTag;
    public string Topic;
    [TextArea] public string FallbackExplanation;
}