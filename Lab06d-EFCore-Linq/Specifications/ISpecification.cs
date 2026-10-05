using System.Linq.Expressions;

namespace Lab06d.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}
