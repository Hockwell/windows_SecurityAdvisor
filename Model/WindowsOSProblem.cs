using SecurityAdvisor.Infrastructure;
using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Model
{
    public class WindowsOSProblem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ProblemRaiting Raiting { get; set; }
        public DetectionTechnique Detection { get; set; }
        public string AdviceForUser { get; set; }
    }
}
