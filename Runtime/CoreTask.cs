using System.Collections;
using System;

namespace LLT
{
	public sealed class CoreTask : IEnumerator
	{
		private IEnumerator _task;
		private bool? _prime;
		private bool _done;
		
		public bool Done
		{
			get
			{
				return _done;
			}
		}
		
		public static CoreTask Wrap(IEnumerator task)
		{
			return new CoreTask(task);
		}
		
		private CoreTask(IEnumerator task)
		{
			_task = task;
		}
		
		public bool Prime()
		{
			CoreAssert.Fatal(_task != null);
			_prime = true;
			return _task.MoveNext();
		}
		
		public void Stop()
		{
			_prime = null;
			_task = null;
			_done = true;
		}
		public bool MoveNext ()
		{
			if(_prime.HasValue)
			{
				var retVal = _prime.Value;
				_prime = null;
				return retVal;
			}
			else if(_task != null)
			{
				var moveNext = _task.MoveNext();
				_done = !moveNext;
				return moveNext;
			}
			
			_done = true;
			return false;
		}
	
		public void Reset ()
		{
			throw new System.NotSupportedException();
		}
	
		public object Current 
		{
			get 
			{
				CoreAssert.Fatal(_task != null);
				return _task.Current;
			}
		}
	}
}