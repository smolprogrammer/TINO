using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace AOI_PPM
{
    public static class LinqHelper
    {
        public static DataTable toDataTable<T>(this IQueryable items)
        {
            Type type = typeof(T);

            //var properties = type.GetProperties().OrderBy(p => p.FirstAttribute<DisplayAttribute>()?.GetOrder() ?? defaultOrder).ToArray();
            /*
            var props = TypeDescriptor.GetProperties(type)                                      
                                      .Cast<PropertyDescriptor>()
                                      .Where  (propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                      .Where  (propertyInfo => propertyInfo.IsReadOnly == false)                                                                            
                                      .ToArray();
                                      */

            /*
            props.OrderBy(p => p.GetCustomAttributes(typeof(DisplayAttribute), true)
                               .Cast<DisplayAttribute>()
                               .Select(a => a.Order)
                               .FirstOrDefault());

    */
            
            var props2 = typeof(T)
                .GetProperties()
                .OrderBy(p => p.GetCustomAttributes(typeof(DisplayAttribute), true)
                               .Cast<DisplayAttribute>()
                               .Select(a => a.Order)
                               .FirstOrDefault()).ToArray();
                               


            var table = new DataTable();

            foreach (var propertyInfo in props2)
            {
                table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
            }

            foreach (var item in items)
            {
                table.Rows.Add(props2.Select(property => property.GetValue(item)).ToArray());
            }

            return table;
        }
    }
}
