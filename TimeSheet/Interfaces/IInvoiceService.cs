
namespace TimeSheet.Interfaces;

public interface IInvoiceService {
    public Task Generate(int id);
}