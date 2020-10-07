public interface IInitializeWith<in T>
{
    void Initialize(T nConfig);
}