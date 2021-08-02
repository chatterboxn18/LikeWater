using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace DungeonQuest
{
    

	public class ActionQueue : MonoBehaviour
	{
		private ConcurrentQueue<Action> _actionQueue = new ConcurrentQueue<Action>();

		public ConcurrentQueue<Action> CurrentQueue => _actionQueue;

		private static float _delay = 0;
		private float _timer = 0f;

		private Action OnDequeue = delegate {  };

		public void Init(float delay)
		{
			_delay = delay;
		}
    
		private void Update()
		{
			if (!_actionQueue.IsEmpty && _timer > _delay)
			{
				_actionQueue.TryDequeue(out var action);
				action.Invoke();
				
				_timer = 0;
			}

			_timer += Time.deltaTime;
		}

		public void AddToQueue(Action action)
		{
			_actionQueue.Enqueue(action);
		}
	}
}