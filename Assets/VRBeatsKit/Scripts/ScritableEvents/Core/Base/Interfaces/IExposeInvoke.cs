namespace VRBeats.ScriptableEvents
{
    public interface IExposeInvoke<T>
    {
        void Invoke(T value);
    }

}
