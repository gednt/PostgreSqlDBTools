using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTools.Model
{
    public class GenericObject
    {
        public String[] columns { get; set; }
        public Object[] values { get; set; }
        public String [] valuesString { get; set; }
     //  public DataView dataView { get; set; }
        public String[] types { get; set; }
        
    }
}
