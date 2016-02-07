using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CityLog {

    #region singleton
    private static CityLog m_log;
    public static CityLog getInstance()
    {
        if(m_log == null)
        {
            m_log = new CityLog();
        }
        return m_log;
    }
    #endregion

    private CityLog()
    {
        initialize();
    }
    private void initialize()
    {
        timeIA = new List<float>();
        timeWait = new List<float>();
    }
    private List<float> timeIA;
    private float minTimeIA = float.MaxValue;
    private float maxTimeIA = float.MinValue;

    private List<float> timeWait;
    private float minTimeWait = float.MaxValue;
    private float maxTimeWait = float.MinValue;

    public void addTimeIA(float time)
    {
        time = time / 10000;
        timeIA.Add(time);
        if(time < minTimeIA)
        {
            minTimeIA = time;
        }
        if(time > maxTimeIA)
        {
            maxTimeIA = time;
        }
    }
    public void addTimeWait(float time)
    {
        time = time / 10000;
        timeWait.Add(time);
        if (time < minTimeWait)
        {
            minTimeWait = time;
        }
        if (time > maxTimeWait)
        {
            maxTimeWait = time;
        }
    }

    public string getStatics()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Statics");
        sb.AppendLine("--------");
        sb.AppendLine("Min time waiting to ask for ia => " + minTimeWait + "ms");
        sb.AppendLine("Max time waiting to ask for ia => " + maxTimeWait + "ms");
        float sumWait= 0;
        for (int i = 0; i < timeWait.Count; i++)
        {
            sumWait += timeWait[i];
        }
        sb.AppendLine("Average time waiting to ask for ia => " + (sumWait / timeWait.Count).ToString() + "ms");

        sb.AppendLine("Min time waiting for ia => " + minTimeIA + "ms");
        sb.AppendLine("Max time waiting for ia => " + maxTimeIA + "ms");
        float sumIA = 0;
        for(int i = 0; i < timeIA.Count; i++)
        {
            sumIA += timeIA[i];
        }
        sb.AppendLine("Average time waiting for ia => " + (sumIA / timeIA.Count).ToString() + "ms");

        return sb.ToString();
    }
}
