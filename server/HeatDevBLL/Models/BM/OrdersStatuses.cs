using System;
using System.Collections.Generic;
using System.Text;

namespace HeatDevBLL.Models
{
    public enum OrdersStatuses
    {
        Awaiting = 0,
        Confirmed = 1,
        Diagnostics = 2,
        Performing = 3,
        Completed = 4,
        CanceledByClient = 5,
        CanceledByWorker = 6
    }
}
