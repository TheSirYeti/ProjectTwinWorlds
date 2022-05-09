using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public List<GameObject> tutorialsInScene;
    private int index;

    public void NextTutorial()
    {
        tutorialsInScene[index].SetActive(false);
        index++;
        tutorialsInScene[index].SetActive(true);
    }

    public void SetTutorial(int id)
    {
        foreach (var tutorial in tutorialsInScene)
        {
            tutorial.SetActive(false);
        }
        tutorialsInScene[id].SetActive(true);
    }

    public void DisableTutorial()
    {
        foreach (var tutorial in tutorialsInScene)
        {
            tutorial.SetActive(false);
        }
    }
}
