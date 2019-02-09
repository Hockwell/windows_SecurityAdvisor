using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityAdvisor.Infrastructure
{
    abstract class RegistryDT : DetectionTechnique //работает с реестром
    {
        protected string pathToKey;
        protected string keyControlValue; 


    }
}
