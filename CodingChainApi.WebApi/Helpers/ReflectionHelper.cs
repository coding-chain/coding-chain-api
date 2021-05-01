using System;
using System.Collections.Generic;
using System.Reflection;

namespace NeosCodingApi.Helpers
{
    public static class ReflectionHelper
    {
        public static List<PropertyInfo>  GetPropertyInfoListByFields<TSource>(string fields)
        {
            var propertyInfoList = new List<PropertyInfo>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsList = fields.Split(',');
                foreach (var field in fieldsList)
                {
                    var trimmedField = field.Trim();
                    var propertyInfo =
                        typeof(TSource).GetProperty(trimmedField,
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {trimmedField} wasn't found on {typeof(TSource)}");
                    }

                    propertyInfoList.Add(propertyInfo);
                }
            }

            return propertyInfoList;
        }
    }
}