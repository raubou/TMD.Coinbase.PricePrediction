using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TMD.Coinbase.PricePrediction.Helpers
{
    public static class Reflection
    {
        public static dynamic GetPropertyValue(string property)
        {
            var prop = property.GetType().GetProperty(property);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Name: " + prop.Name + "Value: " + prop.GetValue(property, null));
            var val = prop.GetValue(property, null);
            return val;
        }

        public static dynamic GetAllProperties(dynamic item)
        {
            List<dynamic> values = new List<dynamic>();
            //foreach(var item in items)
            //{
            var t = item.GetType();
            var p = t.GetProperties();
            foreach(var p1 in p)
            {
                var pt = p1.GetType();
                var val = pt.GetValue(p);
            }
                //foreach (PropertyInfo prop in item.GetType().GetProperties())
                //{
                //    Console.ForegroundColor = ConsoleColor.DarkRed;
                //try
                //{
                //    Console.WriteLine("Name: " + prop.Name + "Value: " + prop.GetValue(item, null));
                //    //var value = prop.GetValue(items, null);
                //}
                //catch (Exception ex)
                //{

                //    //throw;
                //}

                //    //
                //    //values.Add(prop.GetValue(items, null));
                //}
            //}
            return values;
        }
    }
}
