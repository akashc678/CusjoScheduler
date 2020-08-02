using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

public partial class SendEmailtoUser : System.Web.UI.Page
{
    private SqlConnection DBConnection;
    private string appConnectionString;
    private string conStringExcel = System.Configuration.ConfigurationManager.AppSettings["SqlConn"].ToString();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["Id"] != null)
        {
            int id = Convert.ToInt32(Request.QueryString["Id"]);
            updWelcomeMail(id);
            SendEmail(Request.QueryString["EmailId"], null, "Welcome");
        }
        else
        {
            getEmailIdOneByOne();
        }
    }

    public void InitializeDbConnection()
    {
        try
        {
            appConnectionString = System.Configuration.ConfigurationManager.AppSettings["SqlConn"].ToString();
            if (DBConnection == null)
            {
                DBConnection = new SqlConnection(appConnectionString);
                DBConnection.Open();
            }
            if (!DBConnection.State.Equals(System.Data.ConnectionState.Open))
            {
                DBConnection.Open();
            }
        }
        catch (System.Exception ex)
        {
            Response.Write("<script> alert('Db Connection not initialized :Error : " + ex.Message + "');</script>");
        }
    }

    protected void CloseDbConnection()
    {
        //DBConnection.Dispose();
        DBConnection.Close();
    }

    public DataSet getUserListForEmail()
    {
        DataSet dsEmailList = new DataSet();
        try
        {
            InitializeDbConnection();
            using (SqlConnection mycon = new SqlConnection(appConnectionString))
            {
                // mycon.Open();
                using (SqlCommand cmd = new SqlCommand("sp_CusJoEmail", mycon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@mode", "GetEmailList");
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dsEmailList);
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            CloseDbConnection();
        }
        return dsEmailList;
    }

    public string updWelcomeMail(int id)
    {
        string str_res = "";
        try
        {
            InitializeDbConnection();
            using (SqlConnection mycon = new SqlConnection(appConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CusJoEmail", DBConnection))
                {
                    // mycon.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@mode", "UpdIsEmailLinkOpen");
                    cmd.Parameters.AddWithValue("@id", id);
                    str_res = cmd.ExecuteNonQuery().ToString();
                }
            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            CloseDbConnection();
        }
        return str_res;
    }

    public string updRemainderCnt(int id)
    {
        string str_res = "";
        try
        {
            InitializeDbConnection();
            using (SqlConnection mycon = new SqlConnection(appConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CusJoEmail", DBConnection))
                {
                    // mycon.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@mode", "UpdEmailReminderCnt");
                    cmd.Parameters.AddWithValue("@Id", id);
                    str_res = cmd.ExecuteNonQuery().ToString();
                }
            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            CloseDbConnection();
        }
        return str_res;
    }

    public void getEmailIdOneByOne()
    {
        string strEmailId;
        string strId;
        DataSet lds = getUserListForEmail();
        if (lds != null && lds.Tables.Count > 0)
        {
            if (lds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i <= lds.Tables[0].Rows.Count - 1; i++)
                {
                    strEmailId = lds.Tables[0].Rows[i]["EmailId"].ToString();
                    strId = lds.Tables[0].Rows[i]["Id"].ToString();
                    string emailresult = SendEmail(strEmailId, strId, "New Mail");
                    if (emailresult == "Success")
                    {
                        updRemainderCnt(Convert.ToInt32(strId));
                    }
                }
            }
        }
    }

    public string SendEmail(string strEmailID, string strId, string mailType)
    {
        string result;
        try
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(strEmailID);
            mail.To.Add(strEmailID);
            mail.From = new MailAddress("UserFrom@gmail.com");
            mail.Attachments.Add(new Attachment(Server.MapPath("~/UploadedFiles/DbScript_Cusjo.sql")));

            string Body = "";
            string strimg = "";
            if (mailType == "Welcome")
            {
                mail.Subject = "Welcome to CusJo.";
                string strImgUrl = " <img src=http://localhost:2344/UploadedFiles/registration%20success.png/>";
                Body = "<div>Hi, welcome to Cusjo Famliy ,Thank You for registration  <br />" + strImgUrl + "</div>";
                strimg = "welcomeimg.jpg";
            }
            else
            {
                mail.Subject = "Confirmation of Registration on CusJo.";
                string strUrl = "http://localhost:2344/SendEmailtoUser.aspx?id=" + strId + "&EmailId=" + strEmailID + "";
                Body = "<div>Hi, this mail is to test sending mail using Gmail " + strId + " <br /><a href=" + strUrl + ">Click here for welcome mail</a></div>";
                strimg = "registrationsuccess.png";
            }

            mail.AlternateViews.Add(Mail_Body(Body, strimg));
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("AkUserFrom@gmail.com", "AkGmailPassword");
            smtp.Send(mail);


            result = "Success";
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }
        //if (mailMsg != null)
        //{
        //    mailMsg.Dispose();
        //}
        return result;
    }

    private AlternateView Mail_Body(string body, string strimg)
    {
        //<img src=cid:MyImage  id='img' alt='' width='100px' height='100px'/>   
        string path = Server.MapPath(@"UploadedFiles/" + strimg + "");
        LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
        Img.ContentId = "MyImage";
        string str = @"  
            <table>  
                <tr>  
                    <td> '" + body + @"'  
                    </td>  
                </tr>  
                <tr>  
                    <td>  
                      <img src=cid:MyImage  id='img' alt=''/>   
                    </td>  
                </tr></table>  
            ";
        AlternateView AV =
        AlternateView.CreateAlternateViewFromString(str, null, MediaTypeNames.Text.Html);
        AV.LinkedResources.Add(Img);
        return AV;
    }
s}