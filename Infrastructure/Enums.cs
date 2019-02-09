using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure
{
    public enum ProblemRaiting
    {
        Info, Recomended, Critical
    }

    public enum DetectionStatus
    {
        Error, NotFound, Found
    }
}
