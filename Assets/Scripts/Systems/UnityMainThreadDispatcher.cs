// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class UnityMainThreadDispatcher : MonoBehaviour
// {
//     private static readonly Queue<Action> _executionQueue = new();

//     private static UnityMainThreadDispatcher _instance;

//     public static UnityMainThreadDispatcher Instance()
//     {
//         if (!_instance)
//         {
//             var obj = new GameObject("UnityMainThreadDispatcher");
//             _instance = obj.AddComponent<UnityMainThreadDispatcher>();
//             DontDestroyOnLoad(obj);
//         }
//         return _instance;
//     }

//     void Update()
//     {
//         lock (_executionQueue)
//         {
//             while (_executionQueue.Count > 0)
//             {
//                 _executionQueue.Dequeue().Invoke();
//             }
//         }
//     }

//     public void Enqueue(Action action)
//     {
//         if (action == null)
//         {
//             throw new ArgumentNullException(nameof(action));
//         }

//         lock (_executionQueue)
//         {
//             _executionQueue.Enqueue(action);
//         }
//     }
// }