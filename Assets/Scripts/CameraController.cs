using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float m_increment = 0.2f;
    public float minValueOrthographicSize = 1;
    public float maxValueOrthographicSize = 6;

    float marginMovement = 0.1f;

    public float[] limitValues;

    void Start()
    {
        Generator.getInstance().GenerateCity();
        Camera.main.orthographicSize = (maxValueOrthographicSize - minValueOrthographicSize) / 2 + minValueOrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
        {
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - m_increment, minValueOrthographicSize);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + m_increment, maxValueOrthographicSize);
        }

        Vector3 mouse = Input.mousePosition;
        if(mouse.x < Screen.width * marginMovement)
        {
            transform.position += new Vector3(-1, 0, 0) * m_increment;
        }
        if (mouse.y < Screen.height * marginMovement)
        {
            transform.position += new Vector3(0, -1, 0) * m_increment;
        }
        if (mouse.x > Screen.width * (1 - marginMovement))
        {
            transform.position += new Vector3(1, 0, 0) * m_increment;
        }
        if (mouse.y > Screen.height * (1 - marginMovement))
        {
            transform.position += new Vector3(0, 1, 0) * m_increment;
        }

        if(limitValues.Length == 4)
        {
            if ((transform.position.x - Camera.main.orthographicSize) < limitValues[0])
            {
                transform.position += new Vector3(1, 0, 0) * m_increment;
            }
            if ((transform.position.x + Camera.main.orthographicSize) > limitValues[1])
            {
                transform.position += new Vector3(-1, 0, 0) * m_increment;
            }
            if ((transform.position.y - Camera.main.orthographicSize) < limitValues[2])
            {
                transform.position += new Vector3(0, 1, 0) * m_increment;
            }
            if ((transform.position.y + Camera.main.orthographicSize) > limitValues[3])
            {
                transform.position += new Vector3(0, -1, 0) * m_increment;
            }
        }
    }
}
