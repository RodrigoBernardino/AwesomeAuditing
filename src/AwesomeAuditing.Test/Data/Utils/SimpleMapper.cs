using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AwesomeAuditing.Test.Data.Utils
{
    public static class SimpleMapper
    {
        public static void PropertyMap<T, TU>(T source, TU destination)
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
    }
}
