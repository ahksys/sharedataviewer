using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    // Used as the data container for passing Data checking information from 
    // the CheckDataFileMeetsMinimumRequirements() method.
    public class DataCheckResult
    {
        public bool passed { get; set; }
        public string comments { get; set; }
    }
}
