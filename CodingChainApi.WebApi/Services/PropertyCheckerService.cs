using System.Reflection;

namespace CodingChainApi.Services
{
    public class PropertyCheckerService : IPropertyCheckerService
    {
        public bool TypeHasProperty<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
                return true;
            var fieldsArr = fields.Split(',');
            foreach (var field in fieldsArr)
            {
                var trimmedField = field.Trim();
                var propertyInfo = typeof(T).GetProperty(trimmedField,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) return false;
            }

            return true;
        }
    }
}