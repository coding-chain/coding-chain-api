using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace NeosCodingApi.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            
            var expandoObjects = new List<ExpandoObject>();
            var propertyInfoList = ReflectionHelper.GetPropertyInfoListByFields<TSource>(fields);

            AddDataShapedObject(source, propertyInfoList, expandoObjects);

            return expandoObjects;
        }

        private static void AddDataShapedObject<TSource>(IEnumerable<TSource> source, List<PropertyInfo> propertyInfoList, List<ExpandoObject> expandoObjects)
        {
            foreach (var typedObject in source)
            {
                var dataShapedObject = new ExpandoObject();
                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(typedObject);
                    ((IDictionary<string, object>) dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                expandoObjects.Add(dataShapedObject);
            }
        }
    }
}