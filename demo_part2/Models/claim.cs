using System.Data.SqlClient;

namespace demo_part2.Models
{
    public class claim
    {
        public string user_email {  get; set; }
        public string user_id {  get; set; }
        public string hours_worked {  get; set; }

        public string hour_rate {  get; set; }
        public string description {  get; set; }

        connection connect = new connection();

        public string insert_claim(string module,string hour_work,string rate,string note,string filename)
        {
            //temp variable message 
            string message = "";

            string user_ID = get_id();
            string user_EMAIL = get_email();
            string total = "" +(int.Parse(hour_work) * int.Parse(rate));


            string query = "insert into claming values('"+user_EMAIL+"','"+module+"','"+user_ID+"','"+hour_work+"','"+rate+"','"+note+"','none','none','"+total+"','"+filename+"','pending');";
            try
            {
                using(SqlConnection connects = new SqlConnection(connect.connecting()))
                {
                    connects.Open();
                    using(SqlCommand done = new SqlCommand(query,connects))
                    {
                        done.ExecuteNonQuery();
                        message = "done";

                    }
                    connects.Close();
                }

            }
            catch (IOException error)
            {
                message = error.Message;
            }


            return message;
        }
        //get id
        public string get_id()
        {
            //hold id variable
            string hold_id = "";
            try
            {
                using(SqlConnection connects = new SqlConnection(connect.connecting()))
                {
                    connects.Open();

                    using(SqlCommand Prepare = new SqlCommand("select * from active", connects))
                    {
                        using(SqlDataReader getID = Prepare.ExecuteReader())
                        {
                            if (getID.HasRows)
                            {
                                //CHECK ALL,BUT ONE
                                while (getID.Read())
                                {
                                    hold_id = getID["id"].ToString();
                                }
                            }
                            getID.Close();

                        }
                    }

                    connects.Close();
                }

            }catch(IOException error)
            {
                Console.WriteLine(error.Message);
                hold_id = error.Message;
            }
            return hold_id;
        }
        //get email
        public string get_email()
        {
            //hold email variable
            string hold_email = "";
            try
            {
                using (SqlConnection connects = new SqlConnection(connect.connecting()))
                {
                    connects.Open();

                    using (SqlCommand Prepare = new SqlCommand("select * from active", connects))
                    {
                        using (SqlDataReader getemail = Prepare.ExecuteReader())
                        {
                            if (getemail.HasRows)
                            {
                                //CHECK ALL,BUT ONE
                                while (getemail.Read())
                                {
                                    hold_email = getemail["email"].ToString();
                                }
                            }

                            getemail.Close();

                        }
                    }

                    connects.Close();
                }

            }
            catch (IOException error)
            {
                Console.WriteLine(error.Message);
                hold_email= error.Message;
            }
            return hold_email;
        }
    }
}
