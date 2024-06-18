using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using videocall.Models;

namespace videocall.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["StudentData"].ConnectionString;

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_login", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                var result = cmd.ExecuteScalar();

                if (result != null)
                {
                    Session["Name"] = user.Name;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "Invalid UserName or Password. Please enter valid credentials.";
                    return View();
                }
            }
        }

        public ActionResult Index()
        {
            if (Session["Name"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            List<User> users = GetUsers();
            ViewBag.user = users;
            return View();
        }

        List<User> GetUsers()
        {
            List<User> users = new List<User>();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("disp", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                User user = new User()
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    Name = dr["Name"].ToString(),
                };
                users.Add(user);
            }
            return users;
        }
    }
}
