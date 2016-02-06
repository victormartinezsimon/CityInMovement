using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StaticsManager : MonoBehaviour {

    public float timeRefresh = 2;
    private float timeAcum = 0;

    public GameObject staticsGameObject;
    public Text m_text;
    private bool showStatics = false;

    void Start()
    {
        staticsGameObject.SetActive(showStatics);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            showStatics = !showStatics;
            staticsGameObject.SetActive(showStatics);
        }
        if (showStatics)
        {
            show();
        }
    }

    private void show()
    {
        timeAcum += Time.deltaTime;
        if (timeAcum >= timeRefresh)
        {
            timeAcum = 0;
            m_text.text = CityLog.getInstance().getStatics();
        }
    }
}
