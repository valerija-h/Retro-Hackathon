using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float timeLeft; // time left currently

    private float initTimeLeft; // time eft at start of every level
    private bool startTime;

    private void Start()
    {
        initTimeLeft = timeLeft;
        startTime = false;
    }

    private void FixedUpdate()
    {
        if (startTime) {
            timeLeft -= Time.deltaTime;
        }
    }

    public void StartTimer() {
        startTime = true;
    }

    public void StopTimer() {
        startTime = false;
    }

    public float GetTimeLeft() {
        return timeLeft;
    }

    public void ResetTime() {
        StopTimer();
        timeLeft = initTimeLeft;
    }
}
