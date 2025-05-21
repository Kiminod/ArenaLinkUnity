using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArenaLink.ArenaLink.MainThreadDispatcher
{
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        public static UnityMainThreadDispatcher Instance { get; private set; }

        private readonly Queue<Action> _executionQueue = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
    
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Enqueue(Action action)
        {
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }
        }
    
        private void Update()
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    var action = _executionQueue.Dequeue();
                    action?.Invoke();
                }
            }
        }
    }
}