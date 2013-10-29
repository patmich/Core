using System;

namespace LLT
{
	public static class CoreMath
	{
	    public static int NextPowerOfTwo(float val)
	    {
	        return 1 << (int)Math.Ceiling((Math.Log(val)/Math.Log(2)));
	    }
	
	    public static int NextPowerOfTwo(int val)
	    {
	        return 1 << (int)Math.Ceiling((Math.Log(val)/Math.Log(2)));
	    }
	}
}