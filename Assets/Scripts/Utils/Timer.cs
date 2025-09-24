using System;
using UnityEngine;

public class Timer {

    public event Action<float> OnTick;
    public event Action OnFinish;
    private float time, currentTime;
    private GameObject gameObject;
    private bool isRunning;

    public Timer () {
        Reset();
        this.gameObject = new GameObject("Timer", typeof(MonoBehaviorTimer));
        gameObject.GetComponent<MonoBehaviorTimer>().onUpdate = Tick;
    }

    public Timer(UnityEngine.Transform parent) : this() {
        this.gameObject.transform.SetParent(parent);
    }

    private class MonoBehaviorTimer : MonoBehaviour {
        public Action onUpdate;
        private void Update() {
            if (onUpdate != null) onUpdate();
        }
    }

    private void Tick() {
        if (isRunning) {
            currentTime += Time.deltaTime;
            if (currentTime >= time) {
                Stop();
                OnFinish?.Invoke();
            } else { 
                OnTick?.Invoke(currentTime);
            }
        }
    }

    public void Start(float time) {
        this.time = time;
        this.currentTime = 0f;
        isRunning = true;
    }

    private void Stop() {
        isRunning = false;
    }

    private void Reset() {
        isRunning = false;
        currentTime = 0f;
    }

    public bool IsRunning { get { return isRunning; } }
}
