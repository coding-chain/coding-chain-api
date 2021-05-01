using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using ReflectionHelper = NeosCodingApi.Helpers.ReflectionHelper;

namespace NeosCodingApi.Helpers
{
    public static class ObjectExtensions
    {
         public static ExpandoObject ShapeData<TSource>(this TSource source, string fields = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var propertyInfoList = ReflectionHelper.GetPropertyInfoListByFields<TSource>(fields);
            
            return GetDataShappedObject(source, propertyInfoList);
        }
         
         public static IDictionary<string, T> ToDictionary<T>(this object source)
         {
             var dictionary = new Dictionary<string, T>();
             foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                 AddPropertyToDictionary<T>(property, source, dictionary);
             return dictionary;
         }
         private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
         {
             object value = property.GetValue(source) ?? throw new InvalidOperationException($"Cannot get value for property {property.Name}");
             if (value is T value1)
                 dictionary.Add(property.Name, value1);
         }

         public static IDictionary<TKey, TValue> Copy<TKey, TValue>
             (this IDictionary<TKey, TValue> original) 
         {
             var ret = new Dictionary<TKey, TValue>(original.Count);
             foreach (var (key, value) in original)
             {
                 ret.Add(key, value);
             }
             return ret;
         }


       

         private static ExpandoObject GetDataShappedObject<TSource>( TSource source, List<PropertyInfo> propertyInfoList)
        {
            var dataShapedObject = new ExpandoObject();
            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(source);
                ((IDictionary<string, object>) dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }

            return dataShapedObject;
        }
    }
}