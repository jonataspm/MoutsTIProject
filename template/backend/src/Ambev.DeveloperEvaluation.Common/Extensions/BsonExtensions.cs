using System;
using MongoDB.Bson.Serialization;

namespace Ambev.DeveloperEvaluation.Common.Extensions;

public static class BsonExtensions
{
    public static void RegisterIfNotCreated<T>(Action<BsonClassMap<T>> classMapInitializer)
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
        {
            BsonClassMap.RegisterClassMap(classMapInitializer);
        }
    }
}