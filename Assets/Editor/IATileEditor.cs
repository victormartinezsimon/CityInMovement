using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

[CustomEditor(typeof(IATile))]
public class IATileEditor : Editor
{
    AnimBool m_showCheckpoints;
    AnimBool m_showWays;
    Vector2 point;

    void OnEnable()
    {
        m_showCheckpoints = new AnimBool(true);
        //m_showCheckpoints.valueChanged.AddListener(Repaint);

        m_showWays = new AnimBool(true);
        //m_showWays.valueChanged.AddListener(Repaint);
    }
    public override void OnInspectorGUI()
    {
        IATile myTarget = (IATile)target;
        for(int i = 0; i < myTarget.tileNumber.Length; i++)
        {
            EditorGUILayout.LabelField(i.ToString() + "-" + myTarget.tileNumber[i].ToString());
        }

        myTarget.entranceNorth = EditorGUILayout.IntField("Entrance North", myTarget.entranceNorth);
        myTarget.entranceSouth = EditorGUILayout.IntField("Entrance South", myTarget.entranceSouth);
        myTarget.entranceEast = EditorGUILayout.IntField("Entrance East", myTarget.entranceEast);
        myTarget.entranceWest = EditorGUILayout.IntField("Entrance West", myTarget.entranceWest);

        m_showCheckpoints.target = EditorGUILayout.Toggle("Checkpoints", m_showCheckpoints.target);
        
        if (EditorGUILayout.BeginFadeGroup(m_showCheckpoints.faded))
        {
            int actualSize = 0;
            if (myTarget.m_checkpoints != null)
            {
                actualSize = myTarget.m_checkpoints.Length;
            }
            int newSize = EditorGUILayout.IntField("Size", actualSize);//if a new array size is specified
            if (newSize != actualSize)
            {//resize the array
                GameObject[] aux = new GameObject[newSize];
                GameObject[] old = myTarget.m_checkpoints;
                int min = Mathf.Min(newSize, actualSize);
                for (int i = 0; i < min; i++)
                {
                    aux[i] = old[i];
                }
                myTarget.m_checkpoints = aux;
                actualSize = newSize;
            }
            for (int i = 0; i < actualSize; i++)
            {//display  all the elements of the array
                string name = myTarget.m_checkpoints[i] != null ? myTarget.m_checkpoints[i].name : i.ToString();
                myTarget.m_checkpoints[i] = EditorGUILayout.ObjectField(name, myTarget.m_checkpoints[i] , typeof(GameObject), true) as GameObject;
            }
        }
        EditorGUILayout.EndFadeGroup();
        
        m_showWays.target = EditorGUILayout.Toggle("Ways", m_showWays.target);

        int widhtExtra = 40;
        int widthNormal = 40;
        if (EditorGUILayout.BeginFadeGroup(m_showWays.faded))
        {
            int actualSize = 0;
            if(myTarget._m_waysEditor != null)
            {
                actualSize = Mathf.RoundToInt(Mathf.Sqrt(myTarget._m_waysEditor.Length));
            }
            int newSize = EditorGUILayout.IntField("Size", actualSize);

            if(newSize != actualSize)
            {
                bool[] aux = new bool[newSize * newSize];
                bool[] old = myTarget._m_waysEditor;
                int min = Mathf.Min(newSize * newSize, actualSize * actualSize);
                for (int i = 0; i < min; i++)
                {
                    aux[i] = old[i];
                }
                myTarget._m_waysEditor = aux;
                actualSize = newSize;
            }

            //labels
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("=>", GUILayout.Width(widhtExtra));
            for (int i = 0; i < actualSize; i++)
            {
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(widthNormal));
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginVertical();
            for (int i = 0; i < actualSize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(widhtExtra));
                for (int j = 0; j < actualSize; j++)
                {
                    int pos = i * actualSize + j;
                    myTarget._m_waysEditor[pos] = EditorGUILayout.Toggle(myTarget._m_waysEditor[pos], GUILayout.Width(widthNormal));
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFadeGroup();
    }
}
