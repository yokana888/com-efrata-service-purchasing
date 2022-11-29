using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;

namespace Com.Efrata.Service.Purchasing.Lib.Utilities
{
    public abstract class BaseModel : StandardEntity<long>
    {
        [MaxLength(255)]
        public string UId { get; set; } /* Object Id MongoDb */
    }
}
