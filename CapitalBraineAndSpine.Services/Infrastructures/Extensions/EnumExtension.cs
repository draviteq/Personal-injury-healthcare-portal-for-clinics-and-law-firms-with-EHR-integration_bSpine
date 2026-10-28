using System.ComponentModel;
using System.Reflection;

namespace CapitalBraineAndSpine.Service.Infrastructures.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            if (field == null)
            {
                return null;
            }

            DescriptionAttribute attribute =
                (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));

            return attribute != null ? attribute.Description : value.ToString();
        }
    }
}
