using System.Collections.Generic;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.Lib.Services.GarmentDebtBalance
{
    public class DispositionDto
    {
        public DispositionDto(int dispositionId, string dispositionNo, List<MemoDetail> memoDetails)
        {
            DispositionId = dispositionId;
            DispositionNo = dispositionNo;
            MemoDetails = memoDetails;
        }

        public int DispositionId { get; private set; }
        public string DispositionNo { get; private set; }
        public List<MemoDetail> MemoDetails { get; private set; }

        public void OrderByInternalNoteNo()
        {
            MemoDetails = MemoDetails.OrderBy(element => element.InternalNoteNo).ToList();
        }
    }
}