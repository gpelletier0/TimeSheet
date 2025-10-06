using TimeSheet.Specifications;

namespace TimeSheet.Interfaces;

public interface ISpecification {
    public SqlQuery GetQuery();
    public string GetFilterNames();
}