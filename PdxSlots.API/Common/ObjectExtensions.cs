using Newtonsoft.Json;
using System.Reflection;

namespace PdxSlots.API.Common
{
    public static class ObjectExtensions
    {
        public static string ToJSONString(this Object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string ToComparisonEventDescription(this Object obj1, Object comparisonObject)
        {
            List<string> comparisons = new List<string>();

            PropertyInfo[] comparisonProperties = comparisonObject.GetType().GetProperties();
            PropertyInfo[] objectProperties = obj1.GetType().GetProperties();

            foreach (PropertyInfo comparisonProperty in comparisonProperties)
            {
                foreach(PropertyInfo objectProperty in objectProperties)
                {
                    if(comparisonProperty.Name == objectProperty.Name)
                    {
                        comparisons.Add($"{objectProperty.Name}: {objectProperty.GetValue(obj1)?.ToString() ?? "(No Value)"} -> {comparisonProperty.GetValue(comparisonObject)?.ToString() ?? "(No Value)"}");
                    }
                }
            }
            return string.Join(", ", comparisons);
        }
    }
}
