using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Sql;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace SEPM_ScannerUtility
{
    public class DataAccess
    {
        public enum TransactionStatus
        {
            NONE = 0, OPEN = 1, INPROCESS = 2,
            COMPLETE = 3, TIMEOUT = 4
        };
        public static String conStr;

        #region CONSTRUCTORS
        public DataAccess()
        {
            if( conStr ==String.Empty)
            conStr = ConfigurationSettings.AppSettings["DBConStr"];
        }

        public DataAccess(String conStr)
        {
            DataAccess.conStr = conStr;
        }
        #endregion

       



     


        


      


        public int addContact(string number, string name)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into contacts(number , name)
                    values('{0}','{1}')";
            qry = String.Format(qry, number, name);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            qry = String.Empty;
            qry = @"select slNo from contacts where number = '{0}' and name ='{1}'";
            qry = String.Format(qry, number, name);
            cmd = new SqlCommand(qry, con);

            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();




            con.Close();
            con.Dispose();

            return (int)dt.Rows[0][0];
        }


        public void removeContact(int slNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from contacts where slNo = {0}";
            qry = String.Format(qry, slNo);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            qry = @"delete from sms_entity_map where contact= {0}";
            qry = String.Format(qry, slNo);
            cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void addToEscalationList(int id, int line, int department, int level)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into sms_entity_map(contact ,line , department,level)
                    values({0},{1},{2},{3})";
            qry = String.Format(qry, id, line, department, level);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void removeContactFromEscalationList(int id, int line, int department, int level)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"delete from sms_entity_map where contact={0} and line={1} and department={2}and level={3}";
            qry = String.Format(qry, id, line, department, level);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public DataTable getStationsInfo(String stations)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT stations.id , stations.description AS STATION_DESCRIPTION 
                    FROM stations ";
            String qry1 = String.Empty;
            if (stations != String.Empty)
            {
                qry1 = " where id in ({0})";
                qry1 = String.Format(qry1, stations);
            }

            qry = String.Format(qry, stations);
            qry += qry1;

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            con.Close();
            con.Dispose();

            return dt;
        }

        public DataTable getDepartmentInfo(String departments)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select * from departments";
            if (departments != String.Empty)
            {
                qry += " where id in ({0})";
                qry = String.Format(qry, departments);
            }

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            return dt;
        }

        public int findIssueRecord(int line, int station, int department, String data)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select slNo from issues where line = {0} and
                    station = {1} and department = {2} and data = '{3}' 
                    and status <> 'resolved'";
            qry = String.Format(qry,line, station, department, data);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();

            if ((dt.Rows.Count <= 0) || (dt.Rows.Count > 1))
                return -1;

            else return (int)dt.Rows[0][0];
        }


        public int findIssueRecord(int line, int station, int department)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select slNo from issues where line = {0} and
                    station = {1} and department = {2} and status <> 'resolved'";
            qry = String.Format(qry, line, station, department);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();

            if ((dt.Rows.Count <= 0) || (dt.Rows.Count > 1))
                return -1;

            else return (int)dt.Rows[0][0];
        }

        public void updateIssueStatus(int slNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update issues set status = 'resolved' , timestamp = '{0}' where slNo={1}";
            qry = String.Format(qry, DateTime.Now, slNo);
            SqlCommand cmd = new SqlCommand(qry, con);
             
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void updateIssueStatus(DataTable dt)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            for(int i = 0 ;i < dt.Rows.Count;i++)

            {
                String qry = String.Empty;
                qry = @"update issues set status = 'resolved' , timestamp = '{0}' where slNo={1}";
                qry = String.Format(qry, DateTime.Now,(int) dt.Rows[i]["SlNo"]);
                SqlCommand cmd = new SqlCommand(qry, con);
             
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            con.Close();
            con.Dispose();
        }




        public int getActualQuantity(int line, int shift)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select actualQuantity from ProductionQuantity where line = {0} and shift = {1}";
            qry = String.Format(qry, line, shift);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();

            if (dt.Rows.Count == 1)
                return (int)dt.Rows[0]["actualQuantity"];
            else return 0;
        }

        public void updateActualQuantity(int line, int quantity)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into actual(line,quantity,timestamp)
                    values({0},{1},'{2}')";
            qry = String.Format(qry,line,quantity, DateTime.Now.ToString());
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        public void updateTargetQuantity(int line, int shift,int session, int quantity)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"insert into target(line,shift,session,quantity,timestamp)
                    values({0},{1},{2},{3},'{4}')";
            qry = String.Format(qry, line, shift, session, quantity, DateTime.Now.ToString());
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }


        public int insertIssueRecord(int line ,int station, int department, String data,String message)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;

            SqlCommand cmd = new SqlCommand(qry, con);
            DataTable dt = new DataTable();

            cmd = new SqlCommand("insertIssueRecord", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@line", line);
            cmd.Parameters.AddWithValue("@station", station);
            cmd.Parameters.AddWithValue("@department", (int)department);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString());
            cmd.Parameters.AddWithValue("@message", message);
            SqlDataReader dr = cmd.ExecuteReader();
            dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            int slNo = (int)dt.Rows[0][0];
            return slNo;

        }
        public string getIssueStatus(int slNo)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"select status from issues where slNo = {0}";
            qry = String.Format(qry, slNo);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            cmd.Dispose();

            if (dt.Rows.Count == 1)
                return (String)dt.Rows[0][0];
            else return String.Empty;
        }

        public void updateIssueStatus(int slNo, String status)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = String.Empty;
            qry = @"update issues set status = '{0}' , timestamp = '{1}' where slNo={2}";
            qry = String.Format(qry, status, DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"), slNo);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }



        public DataTable getCommands()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            String qry = @"select * from Command where [status] = {0} and request_date='{1}'";

            qry = String.Format(qry, (int)TransactionStatus.OPEN, DateTime.Today.ToShortDateString());

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return dt;
        }


        public bool updateCommandStatus(int id, TransactionStatus status)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"update Command set [status] = {0} where id = {1}";
            qry = String.Format(qry, (int)status, id);


            SqlCommand cmd = new SqlCommand(qry, con);
            try
            {
                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException s)
            {
                result = 0;
            }
            return (result == 1);
        }

        public void updateIssueMarquee()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            string issueMarquee = String.Empty;
            String qry = @"select message from issues where [status] <> 'resolved' ";

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                issueMarquee += (String)dt.Rows[i]["message"] + "; ";
            }
            qry = @"update config set value ='{0}' where [key]='issueMarquee'";
            qry = String.Format(qry, issueMarquee);

            cmd = new SqlCommand(qry, con);
            cmd.ExecuteNonQuery();
        }

        public List<Byte> getIssueInfo(int slNo,out int  l)
        {

            SqlConnection con = new SqlConnection(conStr);
            con.Open();
             l = -1;
            String qry = @"select * from issues where [slNo] = {0}";

            qry = String.Format(qry, slNo);


            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            if (dt.Rows.Count == 0)
                return null;
            short lineNO = Convert.ToInt16((int)dt.Rows[0]["line"]);

            Byte[] line = new byte[2];
            line = BitConverter.GetBytes(lineNO);
            l = lineNO;


            Byte[] stn = 
                BitConverter.GetBytes(Convert.ToInt16((int)dt.Rows[0]["station"]));
            Byte dep =  Convert.ToByte((int)dt.Rows[0]["department"]);

          

            List<Byte> data = new List<byte>();

            data.Add(stn[1]);
            data.Add(stn[0]);

            data.Add(dep);


            for (int i = 0; i < data.Count; i++)
                data[i] += (Byte)'0';


            if (dt.Rows[0]["data"] != DBNull.Value)
            {
                Byte[] msg = new byte[40];
                char[] d = ((String)dt.Rows[0]["data"]).ToCharArray();
                foreach (char c in d)
                    data.Add((Byte)c);
               

            }
        

            return data;

            
            
        }


        #region ManageTab

        public DataTable getReceivers(int line,int department , int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select receiver as Receivers from sms_receiver 
                inner join sms_entity_map on sms_receiver.slNo = sms_entity_map.receiver_id where entity_1_id = {0}
                and entity_2_id = {1} and parameter_1 = {2}";

            qry = String.Format(qry, department,line, escalationLevel);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return dt;
        }


        public Dictionary<String, int> loadReceiverList()
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from sms_receiver";

            Dictionary<String, int> receiverList = new Dictionary<string, int>();

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                receiverList.Add((String)dt.Rows[i]["receiver"], (int)dt.Rows[i]["slNo"]);
            }

            return receiverList;
        }

        public int getReceiverSlNo(String receiver)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select * from sms_receiver where receiver = '{0}'";
            qry = String.Format(qry, receiver);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            int slNo = (dt.Rows.Count > 0) ? (int)(dt.Rows[0]["slNo"]) : -1;
            return slNo;
        }
        public String getMessage(int department)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            String qry = String.Empty;
            qry = @"select message as Message from departments
                 where id = {0}";

            qry = String.Format(qry, department);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            return (String)dt.Rows[0][0];

        }


        public bool updateMessage(String message, int department)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"update departments set message = '{0}' where [id] = {1}";

            qry = String.Format(qry, message, department);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not

        }


        public void addReceiver(String receiver,int line, int department, int slNo,int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = String.Empty;
            SqlCommand cmd = null;


            if (slNo == -1)
            {
                qry = @"insert into sms_receiver(receiver) values('{0}')";
                qry = String.Format(qry, receiver);

                cmd = new SqlCommand(qry, con);
                result = cmd.ExecuteNonQuery();

                qry = @"select Count(*) from sms_receiver";
                cmd = new SqlCommand(qry, con);

                SqlDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dr.Close();

                slNo = (int)dt.Rows[0][0];
            }

            qry = @"insert into sms_entity_map(receiver_id , entity_1_id,entity_2_id,parameter_1) values({0},{1},{2},{3})";
            qry = String.Format(qry, slNo, department,line, escalationLevel);

            cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();
        }

        public bool removeReceiver(int slNo,int line, int department,int escalationLevel)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            int result = 0;
            String qry = @"delete from sms_entity_map where receiver_id = {0} and entity_1_id = {1} 
            and entity_2_id = {2} and parameter_1 = {3} ";

            qry = String.Format(qry, slNo, department,line,escalationLevel);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not
        }

        public bool insertSmsTrigger(string receiver, string message, int status,String timestamp)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            int result = 0;
            String qry = @"insert into sms_trigger(receiver , message , status,timestamp) values('{0}','{1}',{2},'{3}') ";

            qry = String.Format(qry, receiver, message, status,timestamp);
            SqlCommand cmd = new SqlCommand(qry, con);
            result = cmd.ExecuteNonQuery();

            return (result == 1);//If result=1, means update is successful, otherwise not

        }




        #endregion

        #region ReportTab

        public DataTable GetReportData(DateTime from , DateTime to)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();


            to = to.AddDays(1);
            String qry = @"select Substring(Convert(nvarchar,raised.timestamp,0),0,12) as DATE, 
                        
                        stations.description as LINE , 
                        departments.description as ISSUE , 
                        issues.data as DETAILS,
                        CONVERT(TIME(0), raised.timestamp,0) as RAISED , 
                        CONVERT(TIME(0), resolved.timestamp,0) as RESOLVED ,
                        CONVERT(Time(0) , resolved.timestamp - raised.timestamp , 0) as DOWNTIME 
                        from issues
                        inner join stations on stations.id = issues.station
                        inner join lines on lines.id = stations.line_id
                        inner join departments on issues.department = departments.id
                        inner join ( select issue_no, timestamp from issue_tracker where status = 'raised') as raised 
                        on raised.issue_no = issues.slNo

                        left outer join (select issue_no , timestamp from issue_tracker where status = 'resolved')
                        as resolved on resolved.issue_no = issues.slNo 
                        where raised.timestamp >= '{0}' and raised.timestamp <= '{1}'";

            qry = String.Format(qry , from.ToShortDateString(), to.ToShortDateString());

            SqlCommand cmd = new SqlCommand(qry,con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);

            dr.Close();
            
            return dt;


        }


        public DataTable GetOpenIssues()
        {
            SqlConnection localCon = new SqlConnection(conStr);
            String qry = @"select distinct Substring(Convert(nvarchar,issues.timestamp,0),0,12) as DATE, 
                        issues.slNo as SlNo,
                        issues.station as STATION_NO,
                        lines.description as LINE , 
                        stations.description as STATION_NAME,
                        departments.description as ISSUE , 
                        issues.data as DETAILS,
                        CONVERT(TIME(0), issues.timestamp,0) as RAISED 
                        from issues
                        LEFT OUTER JOIN stations on (stations.id = issues.station and stations.line = issues.line)
                        inner join lines on lines.id = issues.line
                        inner join departments on issues.department = departments.id
                        where status<>'resolved'";


            localCon.Open();

            SqlCommand cmd = new SqlCommand(qry, localCon);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);

            dr.Close();

            localCon.Close();
            localCon.Dispose();

            return dt;
        }

        #endregion


        ~DataAccess()
        {

        }

        public void close()
        {

        }
    }
}
