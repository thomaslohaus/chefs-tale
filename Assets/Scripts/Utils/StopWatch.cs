using System;

public class StopWatch {

    private DateTime startTime;
    private bool started = false;

    public bool IsRunning { get { return started; } }

    public void Start() {
        startTime = DateTime.Now;
        started = true;

    }

    public void Stop() {
        started = false;
    }

    public float Elapsed {
        get {
            if (started) {
                DateTime now = DateTime.Now;
                TimeSpan interval = now - startTime;
                return (float)interval.TotalSeconds;
            } else {
                return -1f;
            }
        }
    }
}
