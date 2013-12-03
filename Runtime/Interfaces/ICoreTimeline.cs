namespace LLT
{
    public interface ICoreTimeline
    {
        float Time { get; set; }
        void Play();
        void Pause();
        float Length { get; }
        float Speed { set; }
        bool Loop { get; set; }
    }
}