using System.Reflection;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j;

public static class Reflection
{
    public const BindingFlags EntityBindingFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
}

public static class ReflectionHelper
{
    public static object? ExtractValue(Type type, object? obj)
    {
        if (obj is null)
        {
            return null;
        }
        
        var isNullable = false;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type)!;
            isNullable = true;
        }

        if (type == typeof(string))
        {
        }
        else if (type == typeof(bool))
        {
            obj = bool.Parse(obj.ToString()!);
        }
        else if (type.BaseType == typeof(Enum))
        {
            obj = Enum.Parse(type, obj.ToString()!);
        }
        else
        {
            if (obj.ToString() == "" && isNullable)
            {
                obj = null;
            }
            else
            {
                obj = Activator.CreateInstance(type, obj);
            }
        }

        return obj;
    }
    
    public static string ExtractFieldNameFromInfo(FieldInfo field)
    {
        int start, end;
        
        if (field.Name.Contains('<'))
        {
            start = field.Name.IndexOf('<') + 1;
            end = field.Name.IndexOf('>');    
        }
        else
        {
            start = field.Name.LastIndexOf(' ') + 1;
            end = field.Name.Length;
        }

        return field.Name[start..end];
    }
}

public static class ReflectionHelper<TEntity> where TEntity : Entity
{
    public static object? GetPropertyValue(TEntity entity, string property)
        => entity.GetType().GetField(GetRealFieldName(property), Reflection.EntityBindingFlags)!.GetValue(entity);

    public static void SetPropertyValue(TEntity entity, string property, object? value, bool extractValue = true)
    {
        var entityType = typeof(TEntity);
        while (entityType is not null)
        {
            var fieldInfo = entityType.GetField(GetRealFieldName(property), Reflection.EntityBindingFlags);
            if (fieldInfo is not null)
            {
                if (extractValue)
                {
                    value = ReflectionHelper.ExtractValue(fieldInfo.FieldType, value);
                }
                
                fieldInfo.SetValue(entity, value);
                break;
            }
            
            entityType = entityType.BaseType;
        }
    }

    public static void AddEntryToListPropertyValue<TValue>(TEntity entity, string property, TValue value, bool extractValue = true)
    {
        var entityType = typeof(TEntity);
        while (entityType is not null)
        {
            var fieldInfo = entityType.GetField(GetRealFieldName(property), Reflection.EntityBindingFlags);
            if (fieldInfo is not null)
            {
                if (extractValue)
                {
                    value = (TValue) ReflectionHelper.ExtractValue(fieldInfo.FieldType, value)!;
                }

                var list = (List<TValue>) GetPropertyValue(entity, property)!;
                list.Add(value);
                
                fieldInfo.SetValue(entity, list);
                break;
            }
            
            entityType = entityType.BaseType;
        }
    }
    
    public static List<FieldInfo> GetFieldsInfo()
    {
        var entityType = typeof(TEntity);
        
        var fields = new List<FieldInfo>();
        while (entityType is not null)
        {
            fields = fields.Concat(entityType.GetFields(Reflection.EntityBindingFlags)).ToList();
            entityType = entityType.BaseType;
        }
        
        return fields;
    }

    private static string GetRealFieldName(string fieldName)
        => GetFieldsInfo()
            .Where(fieldInfo => ReflectionHelper.ExtractFieldNameFromInfo(fieldInfo) == fieldName)
            .Select(fieldInfo => fieldInfo.Name)
            .SingleOrDefault()!;
}