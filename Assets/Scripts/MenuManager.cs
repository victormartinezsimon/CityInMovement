using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    public string SizeBoard
    {
        set { Generator.getInstance().TamBoard = int.Parse(value); }
    }
    public string NumberOfCars
    {
        set { Generator.getInstance().m_totalCars = int.Parse(value); }
    }
    public bool Semaphores
    {
        set { Generator.getInstance().withSemaphores = value; }
    }


    public void onButton()
    {
        SceneManager.LoadScene("MainScene");
    }

}
