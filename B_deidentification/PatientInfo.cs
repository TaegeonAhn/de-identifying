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
        public String telNumber;
        public String postNumber;
        public String address1;
        public String address2;


        public PatientInfo(OleDbDataReader reader)
        {
            chartNumber = reader[0].ToString();
            patientName = reader[1].ToString();
            pregiNumber = reader[2].ToString();
            telNumber = reader[3].ToString();
            postNumber = reader[4].ToString();
            address1 = reader[5].ToString();
            address2 = reader[6].ToString();

        }

        public void dump()
        {
            Console.WriteLine(chartNumber + " " + patientName + " " + telNumber);
        }
    }
}
