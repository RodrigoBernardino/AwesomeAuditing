using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace AwesomeAuditing.Domain.Utils.Extensions
{
    internal static class ObjectExtensions
    {
        public static T DeepClone<T>(this T obj) where T : class
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static void InheritedPropertyMap<T, TU>(this T source, TU destination)
            where T : class, new()
            where TU : class, new()
        {
            List<PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList();
            List<PropertyInfo> destinationProperties = destination.GetType().GetProperties().ToList();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                if (sourceProperty.Name != "Id")
                {
                    PropertyInfo destinationProperty = destinationProperties.Find(item => item.Name == sourceProperty.Name);

                    if (destinationProperty != null)
                    {
                        try
                        {
                            destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                }
            }
        }

        public static T DeleteNestedProperties<T>(this T entity)
            where T : class, new()
        {
            Type entityType = entity.GetType();
            foreach (PropertyInfo property in entityType.GetProperties().Where(p =>
                                     p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal))
            {
                // ReSharper disable once PossibleNullReferenceException
                entityType.GetProperty(property.Name).SetValue(entity, null);
            }

            return entity;
        }

        public static string ConvertToJsonWithNestedProperties<T>(this T entity, bool camelCaseSerialization)
            where T : class
        {
            if(camelCaseSerialization)
                return JsonConvert.SerializeObject(entity, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            else
                return JsonConvert.SerializeObject(entity);
        }

        public static string ConvertToJsonWithoutNestedProperties<T>(this T entity)
            where T : class
        {
            JObject jsonObject = new JObject();
            Type entityType = entity.GetType();
            foreach (PropertyInfo property in entityType.GetProperties().Where(p =>
                                     !(p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal)))
            {
                object propertyValue = property.GetValue(entity);
                jsonObject.Add(property.Name, propertyValue != null ? propertyValue.ToString() : String.Empty);
            }

            return JsonConvert.SerializeObject(jsonObject);
        }

        public static T2 ConvertTo<T1, T2>(this T1 source)
            where T1 : class, new()
            where T2 : class, new()
        {
            return JsonConvert.DeserializeObject<T2>(JsonConvert.SerializeObject(source));
        }

        public static void PropertyMap<T, TU>(this T destination, TU source)
            where T : class, new()
            where TU : class, new()
        {
            List<PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList();
            List<PropertyInfo> destinationProperties = destination.GetType().GetProperties().ToList();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                PropertyInfo destinationProperty = destinationProperties.Find(item => item.Name == sourceProperty.Name);

                if (destinationProperty == null)
                    continue;

                try
                {
                    destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                }
                catch (ArgumentException)
                { }
            }
        }

        public static DataTable CreateDataTable<T>(this T source)
        {
            Type sourceType = typeof(T);
            bool isEnumerable = sourceType.IsGenericType &&
                    sourceType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable));

            Type type;
            IEnumerable list;
            if (isEnumerable)
            {
                type = sourceType.GetGenericArguments()[0];
                list = source as IEnumerable;
            }
            else
            {
                type = sourceType;
                list = new List<T> { source };
            }

            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
