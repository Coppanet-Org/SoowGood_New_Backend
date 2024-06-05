using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoowGoodWeb.StoredProcedureManager
{
    public class SqlParams
    {
        public string ParamName { get; set; }
        public SqlDbType SqlType { get; set; }
        public object Value { get; set; }
        public SqlParams(string paramName, SqlDbType oracleType, object value)
        {
            ParamName = paramName;
            SqlType = oracleType;
            Value = value;
        }
    }
}
