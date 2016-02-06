using UnityEngine;
using System.Collections;

public class SemaphoreController : MonoBehaviour {

    public float timeChange;
    private bool inRouteOne;
    public GameObject[] routeOne;
    public GameObject[] routeTwo;
    private float timeAcum;
    public Color m_red;
    public Color m_green;
    public int m_minTimeChange = 1;
    public int m_maxTimeChange = 5;

	// Use this for initialization
	void Start () {
        timeChange = Random.Range(m_minTimeChange, m_maxTimeChange);
        timeAcum = 0;
        inRouteOne = Random.value < 0.5f;
        changeSemaphores();
	}
	
	// Update is called once per frame
	void Update () {
        timeAcum += Time.deltaTime;
        if(timeAcum >= timeChange)
        {
            timeAcum = 0;
            inRouteOne = !inRouteOne;
            changeSemaphores();
        }
	}

    private void changeSemaphores()
    {
        Color oneColor;
        Color twoColor;
        bool oneEnable;
        bool twoEnable;
        if (inRouteOne)
        {
            oneColor = m_green;
            twoColor = m_red;
            oneEnable = true;
            twoEnable = false;
        }
        else
        {
            oneColor = m_red;
            twoColor = m_green;
            oneEnable = false;
            twoEnable = true;
        }

        for(int i = 0; i < routeOne.Length; i++)
        {
            routeOne[i].GetComponent<Renderer>().material.color = oneColor;
            routeOne[i].GetComponent<Collider>().enabled = !oneEnable;
        }

        for (int i = 0; i < routeTwo.Length; i++)
        {
            routeTwo[i].GetComponent<Renderer>().material.color = twoColor;
            routeTwo[i].GetComponent<Collider>().enabled = !twoEnable;
        }

    }
}
