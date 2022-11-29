using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface ICreateable
    {
        Task<int> Create(object model);
    }
}
