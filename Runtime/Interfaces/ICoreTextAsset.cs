namespace LLT
{
	public interface ICoreTextAsset
	{
		bool Shared { get; }
		int Length { get; }

		int ReadInt32(int position);
		float ReadSingle(int position);
		byte ReadByte(int position);
		ushort ReadUInt16(int position);
		void Write(int position, int val);
		void Write(int position, byte val);
		void Write(int position, float val);
		void Write(int position, ushort val);
	}
}