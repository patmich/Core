using System.Collections.Generic;

namespace LLT
{
	public sealed class CoreDictionary<K,V> : Dictionary<K,V>
		where V : class, new()
	{
	    public new V this[K k]
	    {
	        get
	        {
	            if (base.ContainsKey(k))
	            {
	                return base[k];
	            }
				
				var v = new V();
				base.Add(k, v);
	            return v; 
	        }
	        set
	        {
	            if (base.ContainsKey(k))
	            {
	                base[k] = value;
	            }
	            else
	            {
	                base.Add(k, value);
	            }
	        }
	    }
	}
}