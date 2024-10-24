using System.Data.SqlClient;

namespace demo_part2.Models
{
    public class register
    {
        //getters and setters for user info collection

        public string username { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string password { get; set; }



        //connection string class

        connection connect = new connection();







        public string insert_user(string name,string emails,string roles,string password)
        {
            //temp va\riable foe mesage

            string message = "";

            //connect to database
            try
            {
                using (SqlConnection connects = new SqlConnection(connect.connecting())) {
                    //open
                    connects.Open();

                    //query 
                    string query = "insert into users values('"+name+"','"+emails+"','"+roles+"','"+password+"');";

                    //execute command
                    using (SqlCommand add_new_user = new SqlCommand(query,connects)) {

                        //then execute it 
                        add_new_user.ExecuteNonQuery();

                        //assign the message 
                        message = "done";
                    
                    
                    
                    
                    }
                        
                    //then close connection
                    connects.Close();
                }

            }
            catch (IOException error)
            {
                message = error.Message;
            }

            return message;
        }
    }
}
