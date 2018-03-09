using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PeanutButter.Utils;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Ghoul
{
    public class WindowPosition
    {
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Bottom { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Top { get; private set; }

        private static readonly Dictionary<string, PropertyInfo> PropertyInfos =
            typeof(WindowPosition).GetProperties().ToDictionary(pi => pi.Name, pi => pi);

        public WindowPosition(Win32Api.RECT rect)
        {
            Top = rect.Top;
            Left = rect.Left;
            Right = rect.Right;
            Bottom = rect.Bottom;
            Width = rect.Right - rect.Left;
            Height = rect.Bottom - rect.Top;
        }

        public WindowPosition(string serialized)
        {
            (serialized ?? "")
                .Split(',')
                .ForEach(part =>
                {
                    var subs = part.Split(':');
                    if (subs.Length != 2)
                        return;
                    if (!PropertyInfos.TryGetValue(subs[0], out var propInfo))
                        return;
                    if (!TryConvert(subs[1], propInfo.PropertyType, out var propertyValue))
                        return;
                    propInfo.SetValue(this, propertyValue);
                });
        }

        private bool TryConvert(
            string str,
            Type propertyType,
            out object converted)
        {
            if (!Converters.TryGetValue(propertyType, out var converter))
            {
                converted = null;
                return false;
            }

            var args = new object[] {str, null};

            var success = (bool)converter.Invoke(null, args);
            converted = args[1];
            return success;
        }


        private static readonly Dictionary<Type, MethodInfo> Converters = CreateConverterLookup();

        private static Dictionary<Type, MethodInfo> CreateConverterLookup()
        {
            return typeof(WindowPosition).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(mi => mi.Name == nameof(TryConvert) &&
                             (mi.ReturnParameter?.ParameterType == typeof(bool)) &&
                             (mi.GetParameters().Skip(1).FirstOrDefault()?.IsOut ?? false))
                .ToDictionary(
                    mi => mi.GetParameters()[1].ParameterType.GetElementType(),
                    mi => mi
                );
        }

        private static bool TryConvert(string value, out int result)
        {
            try
            {
                result = Convert.ToInt32(value);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        public override string ToString()
        {
            return PropertyInfos
                .Select(kvp => $"{kvp.Key}:{kvp.Value.GetValue(this)}")
                .JoinWith(",");
        }
    }
}