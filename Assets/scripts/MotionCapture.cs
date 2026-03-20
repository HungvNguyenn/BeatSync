using System.Collections.Generic;
using UnityEngine;

public class MotionCaptureDebug : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public float recordInterval = 0.1f; // record every 0.1 sec (10 times/sec)
    
    private float timer = 0f;
    private bool isRecording = false;

    class FrameData
    {
        public float time;
        public Vector3 headPos;
        public Vector3 leftPos;
        public Vector3 rightPos;
    }

    private List<FrameData> framesSet = new List<FrameData>();
    private float startTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Debug.Log("Recording Start");
            framesSet.Clear();
            startTime = Time.time;
            isRecording = true;
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            isRecording = false;
            Debug.Log("recording finish");

            foreach (var frame in framesSet)
            {
                Debug.Log(
                    "Time: " + frame.time.ToString("F2") +
                    " | H: " + frame.headPos +
                    " | L: " + frame.leftPos +
                    " | R: " + frame.rightPos
                );
            }
        }

        if (isRecording)
        {
            timer += Time.deltaTime;

            if (timer >= recordInterval)
            {
                timer = 0f;

                FrameData data = new FrameData();
                data.time = Time.time - startTime;
                data.headPos = head.position;
                data.leftPos = leftHand.position;
                data.rightPos = rightHand.position;

                framesSet.Add(data);
            }
        }
    }
}