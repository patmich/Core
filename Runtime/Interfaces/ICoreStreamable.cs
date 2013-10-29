using System.IO;

namespace LLT
{
	public interface ICoreStreamable
	{
	    int SizeOf();
	    void Write(BinaryWriter writer);
	    void Read(BinaryReader reader);
	}
}