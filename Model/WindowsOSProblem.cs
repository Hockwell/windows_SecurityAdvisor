using SecurityAdvisor.Infrastructure;
using SecurityAdvisor.Infrastructure.Generic;

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
