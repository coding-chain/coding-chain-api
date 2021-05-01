namespace NeosCodingApi.Services
{
    public interface IPropertyCheckerService
    {
        bool TypeHasProperty<T>(string fields);
    }
}