using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FetchListFromUIToConverterC360Mono : MonoBehaviour
{

    public InputField  m_input;
    public Eloi.PrimitiveUnityEvent_StringArray m_onLinesPushed;
    [ContextMenu("Push Selection")]
    public void PushSelection() {
        string [] lines=   m_input.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] =
            lines[i].Trim();
        }
        m_onLinesPushed.Invoke(lines);
    }
}
