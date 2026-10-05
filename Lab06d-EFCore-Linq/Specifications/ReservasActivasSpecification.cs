using System.Linq.Expressions;
using Lab06d.Models;

namespace Lab06d.Specifications;

public sealed class ReservasActivasSpecification : ISpecification<Reserva>
{
    public Expression<Func<Reserva, bool>> Criteria =>
        r => r.Estado != "Cancelada";
}
