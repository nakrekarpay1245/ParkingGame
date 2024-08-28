using System;
using System.Collections.Generic;

/// <summary>
/// ServiceLocator is a simple service registry for managing game services.
/// </summary>
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    /// <summary>
    /// Registers a service of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <param name="service">The service instance.</param>
    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (!_services.ContainsKey(type))
        {
            _services.Add(type, service);
        }
    }

    /// <summary>
    /// Retrieves the service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <returns>The service instance.</returns>
    public static T Get<T>() where T : class
    {
        _services.TryGetValue(typeof(T), out var service);
        return service as T;
    }

    /// <summary>
    /// Clears all registered services.
    /// </summary>
    public static void ClearServices()
    {
        _services.Clear();
    }
}