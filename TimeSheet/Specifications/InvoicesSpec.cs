using TimeSheet.Interfaces;
using TimeSheet.Models.Entities;

namespace TimeSheet.Specifications;

public class InvoicesSpec : ISpecification {

    public SqlQuery GetQuery() {
        var builder = new SqlQueryBuilder()
            .Select(
                "i.Number", 
                "c.Name         AS ClientName", 
                "i.IssueDate", 
                "i.DueDate", 
                "s.ColorArgb")
            .From("Invoices i")
            .LeftJoin("Clients c", "i.ClientId", "c.Id")
            .LeftJoin("Statuses s", "i.StatusId", "s.Id")
            .OrderBy(nameof(Invoice.IssueDate), false);

        return builder.ToQuery();
    }

    public string GetFilterNames() {
        return string.Empty;
    }
}