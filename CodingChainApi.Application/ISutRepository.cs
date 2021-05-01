namespace Application
{
    public interface ISutRepository
    {
        string GetArgumentsByLanguage(string language);
        string GetRuntimeByLanguage(string language);
    }
}