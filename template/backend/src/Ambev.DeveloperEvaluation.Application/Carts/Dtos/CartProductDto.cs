using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.Dtos
{
    public record CartProductDto
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
