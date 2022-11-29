using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Lib.Interfaces
{
    public interface IDeleteable
    {
        Task<int> Delete(int id);
    }
}
