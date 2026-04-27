using Ambev.DeveloperEvaluation.Common.Extensions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.ORM.Mapping.Mongo;

public static class CartMap
{
    public static void Configure()
        => BsonExtensions.RegisterIfNotCreated<Cart>(map =>
        {
            map.AutoMap();
        });
}

