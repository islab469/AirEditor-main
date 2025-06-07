using System;
using UnityEngine;

public class QuestionManager: MonoBehaviour{

    [Serializable]
    public class Questions
    {
        /*       
        'title':question[0],
        'truthes':question[1],
        'falses':question[2]
        */
        public QuestionData[] data;


    }
    [Serializable]
    public class QuestionData
    {
        public string[] title;
        public string[] truthes;
        public string[] falses;
    }


    public static void printQuestions(Questions questions) {
        int ind = 0;
        foreach (QuestionData qd in questions.data){
            ind += 1;
            Debug.Log("[QuestionManager] PrintQuestion " + ind.ToString());
            printQuestionData(qd);


        }
        
    }
    public static void printQuestionData(QuestionData questionData){
        Debug.Log("===== Titles =====");
        for (int j = 0; j < questionData.title.Length; j++)
        {
            Debug.Log(questionData.title[j]);
        }
        Debug.Log("===== Truthes =====");
        for (int j = 0; j < questionData.truthes.Length; j++)
        {
            Debug.Log(questionData.truthes[j]);
        }
        Debug.Log("===== Falses =====");
        for (int j = 0; j < questionData.falses.Length; j++)
        {
            Debug.Log(questionData.falses[j]);
        }
    }
}
