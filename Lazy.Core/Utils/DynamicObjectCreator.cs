using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Lazy.Core.Utils
{
    public static class DynamicObjectCreator
    {
        // 核心方法：根据类型名和字典创建对象
        public static object CreateObjectFromDictionary(string typeFullName, IDictionary<string, object> propertyValues)
        {
            if (string.IsNullOrWhiteSpace(typeFullName))
                throw new ArgumentNullException(nameof(typeFullName));

            if (propertyValues == null)
                throw new ArgumentNullException(nameof(propertyValues));

            // 1. 根据类型名获取Type
            Type targetType = GetTypeByName(typeFullName);
            if (targetType == null)
                throw new TypeLoadException($"无法找到类型: {typeFullName}");

            // 2. 创建对象实例
            object instance = Activator.CreateInstance(targetType);

            // 3. 设置属性值
            SetPropertiesFromDictionary(instance, propertyValues);

            return instance;
        }

        // 泛型版本，返回具体类型
        public static T CreateObjectFromDictionary<T>(IDictionary<string, object> propertyValues) where T : new()
        {
            if (propertyValues == null)
                throw new ArgumentNullException(nameof(propertyValues));

            T instance = new T();
            SetPropertiesFromDictionary(instance, propertyValues);
            return instance;
        }

        // 获取类型（支持多种查找方式）
        private static Type GetTypeByName(string typeFullName)
        {
            // 方式1：直接获取
            Type type = Type.GetType(typeFullName);
            if (type != null) return type;

            // 方式2：从当前程序集获取
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            type = currentAssembly.GetType(typeFullName);
            if (type != null) return type;

            // 方式3：从调用程序集获取
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            type = callingAssembly.GetType(typeFullName);
            if (type != null) return type;

            // 方式4：搜索所有已加载的程序集
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    type = assembly.GetType(typeFullName);
                    if (type != null) return type;
                }
                catch
                {
                    // 忽略加载错误的程序集
                }
            }

            // 方式5：模糊匹配（按名称）
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in assembly.GetTypes())
                    {
                        if (t.FullName == typeFullName || t.Name == typeFullName)
                        {
                            return t;
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // 忽略无法完全加载的程序集
                }
            }

            return null;
        }

        // 设置属性值
        private static void SetPropertiesFromDictionary(object instance, IDictionary<string, object> propertyValues)
        {
            if (instance == null) return;

            Type type = instance.GetType();

            foreach (var kvp in propertyValues)
            {
                string propertyName = kvp.Key;
                object value = kvp.Value;

                // 查找属性
                PropertyInfo property = type.GetProperty(propertyName);
                if (property != null && property.CanWrite)
                {
                    try
                    {
                        // 转换并设置值
                        object convertedValue = ConvertValue(value, property.PropertyType);
                        property.SetValue(instance, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"设置属性 {propertyName} 失败: {ex.Message}");
                    }
                }
                else
                {
                    // 如果找不到属性，尝试查找字段
                    FieldInfo field = type.GetField(propertyName);
                    if (field != null)
                    {
                        try
                        {
                            object convertedValue = ConvertValue(value, field.FieldType);
                            field.SetValue(instance, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"设置字段 {propertyName} 失败: {ex.Message}");
                        }
                    }
                }
            }
        }

        // 类型转换器
        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                // 如果是值类型且不可空，返回默认值
                if (targetType.IsValueType && !IsNullableType(targetType))
                    return Activator.CreateInstance(targetType);
                return null;
            }

            // 如果已经是目标类型，直接返回
            if (targetType.IsInstanceOfType(value))
                return value;

            // 处理可空类型
            if (IsNullableType(targetType))
            {
                Type underlyingType = Nullable.GetUnderlyingType(targetType);
                return ConvertValue(value, underlyingType);
            }

            // 处理特殊类型转换
            if (targetType == typeof(string))
                return value.ToString();

            if (targetType.IsEnum)
            {
                if (value is string stringValue)
                    return Enum.Parse(targetType, stringValue);
                return Enum.ToObject(targetType, value);
            }

            // 处理数组和集合
            if (targetType.IsArray && value is System.Collections.IEnumerable enumerable)
            {
                Type elementType = targetType.GetElementType();
                var list = new System.Collections.ArrayList();

                foreach (var item in enumerable)
                {
                    list.Add(ConvertValue(item, elementType));
                }

                return list.ToArray(elementType);
            }

            // 处理List<T>
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = targetType.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(elementType);
                var list = Activator.CreateInstance(listType) as System.Collections.IList;

                if (value is System.Collections.IEnumerable enumerable2)
                {
                    foreach (var item in enumerable2)
                    {
                        list.Add(ConvertValue(item, elementType));
                    }
                }

                return list;
            }

            // 使用Convert.ChangeType进行基本类型转换
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                // 如果转换失败，尝试使用目标类型的Parse方法
                try
                {
                    MethodInfo parseMethod = targetType.GetMethod("Parse",
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new Type[] { typeof(string) },
                        null);

                    if (parseMethod != null)
                    {
                        return parseMethod.Invoke(null, new object[] { value.ToString() });
                    }
                }
                catch
                {
                    // 忽略解析失败
                }

                throw new InvalidCastException($"无法将值 '{value}' ({value.GetType()}) 转换为类型 {targetType}");
            }
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
