using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] private List<GameObject> optionPanels;


    void Start() => ShowOptions(0);
    
    
    public void LoadScene(int scene) => SceneManager.LoadScene(scene);

    public void ShowOptions(int index)
    {
        for (int i = 0; i < optionPanels.Count; i++)
        {
            GameObject optionPanel = optionPanels[i];
            optionPanel.SetActive(index == i);
        }
    }
}
