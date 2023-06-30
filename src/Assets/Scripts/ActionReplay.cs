using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Recorder
{
    public Vector3 pos;

    public Quaternion rot;
}

public class ActionReplay : MonoBehaviour
{
    public bool recording;
    public float current;
    public List<Recorder> Recorder = new();

    public IEnumerator SetRecorder()
    {
        recording = true;
        Recorder.Capacity = 100;

        if (Recorder.Count == 100)
        {
            Recorder.Remove(Recorder[0]);
        }
        Recorder.Add(new Recorder { pos = transform.position, rot = transform.rotation });
        yield return null;

    }

    public void Playback(int index)
    {
        recording= false;
        Recorder recorder = Recorder[index];
        transform.position = recorder.pos;
        transform.rotation = recorder.rot;

        //transform.SetPositionAndRotation(recorder.pos, recorder.rot);
        //yield return null;
        //StartCoroutine(SetRecorder());
        //StopCoroutine(Playback(index));
    }

}


