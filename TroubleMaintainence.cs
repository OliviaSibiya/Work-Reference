using System.Collections.Generic;
using TMPro;
using TroubleshootingScript;
using UnityEngine;
using UnityEngine.Events;

public class TroubleMaintainence : MonoBehaviour
{
    public List<GameObject> wrongAnswers;

    public UnityEvent audioPlay;

    public UnityEvent sceneReload;
    public TMP_Text incorrectlySelected;

    public string wrongOption = "Incorrect ";
    public int incorrect = 0;
    public bool incorrectSelection;

    public bool reloadedScene;

    public void Update()
    {
        for (int i = 0; i < wrongAnswers.Count; i++)
        {
            if (wrongAnswers[i].activeInHierarchy)
            {
                incorrect = 0;
                AnswerIncorrect();
                incorrectSelection = true;
                Debug.Log("Incorrect option was selected");
            }
        }

        if (incorrect == 3)
        {
            incorrectSelection = true;
            Debug.Log("Incorrect Answer Selected 3 times");
            if (incorrectSelection == true)
            {
                sceneReload.Invoke();
                reloadedScene = true;
                Debug.Log("Restarted the scene");

                if (reloadedScene == true)
                {
                    audioPlay.Invoke();
                    Debug.Log("Audio has played");
                }
            }
        }

        incorrectlySelected.text = wrongOption.ToString() + "/" + incorrect.ToString();
    }

    public void AnswerIncorrect()
    {
        incorrect += 1;
    }

    //public void AfterCompleted()
    //{
    //    if (ScenarioDisplay.finished == true)
    //    {
    //        Debug.Log("Completed the needed Scenarios");
    //    }
    //}
}
