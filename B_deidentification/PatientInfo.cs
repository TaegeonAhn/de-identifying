using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de_identifying
{
    class PatientInfo
    {
        public String chartNumber;
        public String patientName;
        public String pregiNumber;
        public String tel;

        public PatientInfo(OleDbDataReader reader)
        {
            chartNumber = reader[0].ToString();
            patientName = reader[1].ToString();
            pregiNumber = reader[2].ToString();
            tel = reader[3].ToString();

        }

        public void dump()
        {
            Console.WriteLine(chartNumber + " " + patientName + " " + pregiNumber + " " + tel);
        }
    }
}
