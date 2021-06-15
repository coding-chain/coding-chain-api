namespace CodingChainApi.Services
{
    public interface IPropertyCheckerService
    {
        bool TypeHasProperty<T>(string fields);
    }
}