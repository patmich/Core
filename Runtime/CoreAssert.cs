using System;

/// <summary>
/// Assert.
/// </summary>
/// 
namespace LLT
{
	public static class CoreAssert
	{
		private static Action<string, object> _log;
		private static Action _break;

		public static void Init(Action<string, object> log, Action break_)
		{
			_log = log;
			_break = break_;
		}
		
		/// <summary>
		/// Fatal assert.
		/// </summary>
		/// <param name='value'>
		/// Value.
		/// </param>
	    [System.Diagnostics.Conditional("ASSERT_ENABLED")]
	    public static void Fatal(bool value)
	    {
	        Fatal(value, string.Empty);
	    }
	
		/// <summary>
		/// Fatal assert.
		/// </summary>
		/// <param name='value'>
		/// Value.
		/// </param>
		/// <param name='message'>
		/// Message.
		/// </param>
		/// <param name='context'>
		/// Context.
		/// </param>
		[System.Diagnostics.Conditional("ASSERT_ENABLED")]
	    public static void Fatal(bool value, string message)
	    {
	        if (!value)
	        {
				if(_log != null)
				{
	            	_log(message, null);
				}
				if(_break != null)
				{
					_break();
				}
	        }
	    }
	}
}