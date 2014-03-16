using System;

/// <summary>
/// Assert.
/// </summary>
/// 
namespace LLT
{
	public static class CoreAssert
	{
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
				UnityEngine.Debug.LogError(message);
	        }
	    }
	}
}