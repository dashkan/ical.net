﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ical.Net.Serialization;

internal class SerializationUtil
{
    public static void OnDeserializing(object obj)
    {
        foreach (var mi in GetDeserializingMethods(obj.GetType()))
        {
            mi.Invoke(obj, new object[] {new StreamingContext() });
        }
    }

    public static void OnDeserialized(object obj)
    {
        foreach (var mi in GetDeserializedMethods(obj.GetType()))
        {
            mi.Invoke(obj, new object[] { new StreamingContext() });
        }
    }

    private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;

    private static readonly ConcurrentDictionary<Type, List<MethodInfo>> _onDeserializingMethods = new ConcurrentDictionary<Type, List<MethodInfo>>();
    private static List<MethodInfo> GetDeserializingMethods(Type targetType)
    {
        if (targetType == null)
        {
            return new List<MethodInfo>();
        }

        if (_onDeserializingMethods.ContainsKey(targetType))
        {
            return _onDeserializingMethods[targetType];
        }

        return _onDeserializingMethods.GetOrAdd(targetType, tt => tt
            .GetMethods(BindingFlags)
            .Where(targetTypeMethodInfo => targetTypeMethodInfo
                .GetCustomAttributes(typeof(OnDeserializingAttribute), false).Any())
            .ToList());
    }

    private static ConcurrentDictionary<Type, List<MethodInfo>> _onDeserializedMethods = new ConcurrentDictionary<Type, List<MethodInfo>>();
    private static List<MethodInfo> GetDeserializedMethods(Type targetType)
    {
        if (targetType == null)
        {
            return new List<MethodInfo>();
        }

        if (_onDeserializedMethods.TryGetValue(targetType, out var methodInfos))
        {
            return methodInfos;
        }

        methodInfos = targetType.GetMethods(BindingFlags)
            .Select(targetTypeMethodInfo => new
            {
                targetTypeMethodInfo,
                attrs = targetTypeMethodInfo.GetCustomAttributes(typeof(OnDeserializedAttribute), false).ToList()
            })
            .Where(t => t.attrs.Count > 0)
            .Select(t => t.targetTypeMethodInfo)
            .ToList();

        _onDeserializedMethods.AddOrUpdate(targetType, methodInfos, (_, _) => methodInfos);
        return methodInfos;
    }
}