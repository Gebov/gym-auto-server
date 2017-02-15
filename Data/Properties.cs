using System.ComponentModel;
using System.Reflection;
using System;

namespace Gym.Data
{
    internal class Properties
    {
        internal static void Copy(object source, object target)
        {
            var sourceProperties = TypeDescriptor.GetProperties(source);

            var targetProperties = TypeDescriptor.GetProperties(target);

            var isIDataItem = typeof(IDataItem).IsAssignableFrom(source.GetType());
            foreach (PropertyDescriptor sourceProp in sourceProperties)
            {   
                var targetProp = targetProperties.Find(sourceProp.Name, false);

                var sourceVal = sourceProp.GetValue(source);
                
                if (!Type.Equals(sourceProp.PropertyType, targetProp.PropertyType))
                {
                    var convertor = targetProp.Converter;
                    if (convertor.CanConvertFrom(sourceProp.PropertyType))
                    {
                        sourceVal = convertor.ConvertFrom(sourceVal);
                    }
                    else 
                    {
                        continue;
                    }
                }

                var targetVal = targetProp.GetValue(target);

                if (object.Equals(sourceVal, targetVal))
                    continue;

                targetProp.SetValue(target, sourceVal);
            }
        }
    }
}