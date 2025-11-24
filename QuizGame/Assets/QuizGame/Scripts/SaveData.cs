using System;
using System.Collections.Generic;
[Serializable]
public class TopicStats
{
    public int Correct;
    public int Total;
}

[Serializable]
public class SaveData
{
    public Dictionary<string, TopicStats> ExamStats = new();
    public Dictionary<string, TopicStats> TopicStats = new();
    public string LastExam;
    public string LastTopic;
    public int LastScore;
    public int LastTotal;
}