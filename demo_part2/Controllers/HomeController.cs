using demo_part2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace demo_part2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //check the connection
            try
            {
                //get connection string from connection class
                connection conn = new connection();

                //then check
                using (SqlConnection connect = new SqlConnection(conn.connecting()))
                {
                    //open connection
                    connect.Open();
                    Console.WriteLine("Connected");
                    connect.Close();

                }

            }
            catch (IOException error)
            {
                //error 
                Console.WriteLine("Error : " + error.Message);

            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //http post for the register

        //from the register form
        [HttpPost]
        public IActionResult Register_user(register add_user)
        {
            //collect user's value
            string name = add_user.username;
            string email = add_user.email;
            string password = add_user.password;
            string role = add_user.role;


            ///check if all are collected 

            //Console.WriteLine("Name: " + name + "\nEmail: " + email + "Role: " + role);

            //passs all the va\lues to insert method
            string message = add_user.insert_user(name, email, role, password);

            //then check if the user is inserted
            if (message == "done")
            {
                //track error output
                Console.Write(message);
                //reirect
                return RedirectToAction("Login", "Home");

            }
            else
            {

                //track error output
                Console.Write(message);

                //redirect
                return RedirectToAction("Index", "Home");

            }
        }
        //for login page 
        public IActionResult Login()
        {
            return View();
        }

        //open dashboard
        public IActionResult Dashboard()
        {
            return View();
        }



        //login page
        [HttpPost]
        public IActionResult login_user(check_login user)

        {
            string email = user.email;
            string role = user.role;
            string password = user.password;

            string message = user.login_user(email, role, password);
            if (message == "found")
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                Console.WriteLine(message);
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]

        public IActionResult claim_sub(IFormFile file, claim insert)
        {
            //assign
            string module_name = insert.user_email;
            string hour_work = insert.hours_worked;
            string hour_rate = insert.hour_rate;
            string description = insert.description;



            //file info
            string filename = "no file";
            if (file != null && file.Length > 0)
            {
                // Get the file name
                filename = Path.GetFileName(file.FileName);

                // Define the folder path (pdf folder)
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf");

                // Ensure the pdf folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Define the full path where the file will be saved
                string filePath = Path.Combine(folderPath, filename);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);


                }
            }


            string message = insert.insert_claim(module_name, hour_work, hour_rate, description, filename);


            if (message == "done")
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }

        }

        public IActionResult view_claims()
        {
            //constructor for it to refresh automatically
            get_claims collect = new get_claims();



            return View(collect);
        }




        [HttpGet]
        
        public IActionResult approve(int id) 
        {
            approve claim = null;

            connection conn = new connection();


            using (SqlConnection connect = new SqlConnection(conn.connecting()))
            {
                {
                    connect.Open();
                    // Adjust table name and column names as needed

                    string query = "SELECT * FROM claming  WHERE id = @UserId"; 
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@UserId", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                claim = new approve
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Module = reader.GetString(reader.GetOrdinal("Module")),
                                    HoursWorked = reader.GetDouble(reader.GetOrdinal("HoursWorked")),
                                    HourRate = reader.GetDouble(reader.GetOrdinal("HourRate")),
                                    TotalAmount = reader.GetDouble(reader.GetOrdinal("TotalAmount")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                    Note = reader.GetString(reader.GetOrdinal("Note")),
                                    DocumentFilename = reader.GetString(reader.GetOrdinal("DocumentFilename")),
                                    SubmissionDate = reader.GetDateTime(reader.GetOrdinal("SubmissionDate")),
                                    ApprovalDate = reader.IsDBNull(reader.GetOrdinal("ApprovalDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ApprovalDate")),
                                    RejectionDate = reader.IsDBNull(reader.GetOrdinal("RejectionDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("RejectionDate")),
                                    ApprovedByUserId = reader.IsDBNull(reader.GetOrdinal("ApprovedByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ApprovedByUserId")),
                                    RejectedByUserId = reader.IsDBNull(reader.GetOrdinal("RejectedByUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RejectedByUserId")),
                                    RejectionReason = reader.IsDBNull(reader.GetOrdinal("RejectionReason")) ? null : reader.GetString(reader.GetOrdinal("RejectionReason")),
                                    IsValid = reader.GetBoolean(reader.GetOrdinal("IsValid"))
                                };
                            }
                        }
                    }
                }

                if (claim == null)
                {
                    return NotFound(); 
                }

                return View(claim); 
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(approve model, int approverId)
        {
            if (ModelState.IsValid)

            {
                connection conn = new connection();

                using (SqlConnection connect = new SqlConnection(conn.connecting()))
                {
                    connect.Open();
                    string query = "UPDATE claming SET Status = @Status, ApprovalDate = @ApprovalDate, ApprovedByUserId = @ApprovedByUserId WHERE id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    { 
                        cmd.Parameters.AddWithValue("@Status", "Approved");
                        cmd.Parameters.AddWithValue("@ApprovalDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ApprovedByUserId", approverId);
                        cmd.Parameters.AddWithValue("@UserId", model.UserId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Redirect to the list of claims after approval
                            return RedirectToAction("Index"); 
                        }
                    }
                }
            }

            // If we reach here, it means the model state is invalid or no rows were affected
            // Return the same view with the model to show validation errors or other messages
            return View(model);
        }



        public IActionResult approves()
        {
            List<approve> claims = new List<approve>();

            using (var connect = new SqlConnection(new connection().connecting()))
            {
                connect.Open();
                string query = "SELECT id AS UserId, email AS Email, module AS Module, hours AS HoursWorked, rate AS HourRate, total AS TotalAmount, status AS Status, note AS Note, file_name AS DocumentFilename FROM claming WHERE status = 'Pending'";

                using (var cmd = new SqlCommand(query, connect))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            claims.Add(new approve
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Module = reader.GetString(reader.GetOrdinal("Module")),
                                // Parse string to double
                                HoursWorked = double.TryParse(reader.GetString(reader.GetOrdinal("HoursWorked")), out double hoursWorked) ? hoursWorked : 0,
                                HourRate = double.TryParse(reader.GetString(reader.GetOrdinal("HourRate")), out double hourRate) ? hourRate : 0,
                                TotalAmount = double.TryParse(reader.GetString(reader.GetOrdinal("TotalAmount")), out double totalAmount) ? totalAmount : 0,
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Note = reader.GetString(reader.GetOrdinal("Note")),
                                DocumentFilename = reader.GetString(reader.GetOrdinal("DocumentFilename")),
                                // Add default values for remaining properties
                                ApprovalDate = null,
                                RejectionDate = null,
                                ApprovedByUserId = null,
                                RejectedByUserId = null,
                                RejectionReason = null,
                                IsValid = true // Assuming pending claims are valid
                            });
                        }
                    }
                }
            }

            return View(claims); // Pass the list of claims to the view
        }

        public IActionResult ApproveClaim(int userId)
        {
            using (var connect = new SqlConnection(new connection().connecting()))
            {
                connect.Open();

                // Update the claim status to 'Approved' in the database
                string updateQuery = "UPDATE claming SET status = 'Approved' WHERE id = @UserId";
                using (var cmd = new SqlCommand(updateQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Message"] = "Claim approved successfully.";
            return RedirectToAction("view_claims");
        }

        public IActionResult RejectClaim(int userId)
        {
            using (var connect = new SqlConnection(new connection().connecting()))
            {
                connect.Open();

                // Update the claim status to 'Rejected' in the database
                string updateQuery = "UPDATE claming SET status = 'Rejected' WHERE id = @UserId";
                using (var cmd = new SqlCommand(updateQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Message"] = "Claim rejected successfully.";
            return RedirectToAction("view_claims");
        }

    }
    }


        
    






