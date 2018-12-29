using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using winForm = System.Windows.Forms;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net.Mail;

namespace _11._10._2018_P
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection(@"data source=DESKTOP-24R619J\HODAYASQL;initial catalog=SourceNet;integrated security=True;");
        SqlCommand cmd;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void choose_folder(object sender, RoutedEventArgs e)
        {
            string path1 = @"C:\משתמשים\";
            string pathToNewFolder = System.IO.Path.Combine(path1, "BATABASE");
            Directory.CreateDirectory(pathToNewFolder);
            insertManagerUser("hodiyoav@gmail.com");
            winForm.FolderBrowserDialog folderDialog = new winForm.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;
            folderDialog.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            winForm.DialogResult result = folderDialog.ShowDialog();
            string messageToEmail;
            int index = 1;
            if (result == winForm.DialogResult.OK)
            {
                string pathS = folderDialog.SelectedPath;
                try
                {
                    DirectoryInfo folder = new DirectoryInfo(pathS);
                    if (folder.Exists)
                    {
                        con.Open();
                        foreach (DirectoryInfo folderI in folder.GetDirectories())
                        {
                            sortFiles(folderI, 0,(index++)*10);
                        }
                        con.Close();
                    }
                    MessageBox.Show(" !!!הצליח להעתיק");
                    messageToEmail = "Hello,/n Welcome to SourceNet,/n It appears that you are the system administrator /n/n UserName:SystemAdministrator /n Password:manger123 /n Regards";
                    SendEmailToUser("Message From SourceNet System", messageToEmail, "hodiyoav@gmail.com");

                }
                catch (Exception)
                {
                    MessageBox.Show("לא הצליח להעתיק");
                    throw;
                }
            }
        }
        public void insertManagerUser(string email)
        {
            try
            {
                cmd = new SqlCommand("insert into Permissions(permissionsCode,permissionsType) values(1,'director')", con);
                con.Open();
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("insert into Users(userCode,username,password,email,permissionCode) values(10,'SystemAdministrator','manger123','hodiyoav@gmaol.com',1)", con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void addPermissions()
        {
            con.Open();
            cmd = new SqlCommand("insert into ResourcePermissions(ResoucePermisCode,ResoucePermisType) values(1,'R')", con);
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("insert into ResourcePermissions(ResoucePermisCode,ResoucePermisType) values(2,'D')", con);
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("insert into ResourcePermissions(ResoucePermisCode,ResoucePermisType) values(3,'P')", con);
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("insert into ResourcePermissions(ResoucePermisCode,ResoucePermisType) values(4,'W')", con);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void sortFiles(DirectoryInfo folder, int parentCategory,int categoryCode)
        {
            insertToCategoryTbl(categoryCode, parentCategory, folder.Name);
            string targetPath = @"C:\משתמשים\BATABASE";
            //אם יש עוד תקיות אז
            int i = 0;
            foreach (DirectoryInfo folderI in folder.GetDirectories())
            {
                sortFiles(folderI, categoryCode , (categoryCode+1)+100*i++);
            }
            string Path = @"";
            string sourcePath;
            //רק אם יש קבצים בתקיה
            foreach (FileInfo fi in folder.GetFiles())
            {
                string fileName = fi.ToString();
                sourcePath = Path + fi.DirectoryName;
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileName);
                System.IO.File.Copy(sourceFile, destFile, true);
                int counterSources = getCounterResources();
                insertToResourcesTbl(counterSources, fileName, targetPath, 1);
                insertToResourceCategoryTbl(counterSources, categoryCode);
            }
        }
        public void insertToCategoryTbl(int categoryCode,int parentCategory, string folderName)
        {
            try
            {
                cmd = new SqlCommand("insert into Categories(categoryCode,categoryName,parentCategory) values(@categoryCode,@categoryName,@parentCategory)", con);
                cmd.Parameters.Add(new SqlParameter("@categoryCode", categoryCode));
                cmd.Parameters.Add(new SqlParameter("@categoryName", folderName));
                if (parentCategory == 0)//אב קטגוריה ראשית
                    ///////////// int לשאול למה הוא לא רוצה לקבל 
                    cmd.Parameters.Add(new SqlParameter("@parentCategory", 0.ToString()));
                else
                    cmd.Parameters.Add(new SqlParameter("@parentCategory", parentCategory.ToString()));
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void insertToResourcesTbl(int code,string fileName,string targetPath,int i)
        {
            try
            {
                cmd = new SqlCommand("insert into Resources(resourceCode,resourceName,filePath,version,authorName,resourcePermission) values(@resourceCode,@resourceName,@filePath,@version,@authorName,@resoursePermission)", con);
                cmd.Parameters.Add(new SqlParameter("@resourceCode", code));
                cmd.Parameters.Add(new SqlParameter("@resourceName", fileName));
                cmd.Parameters.Add(new SqlParameter("@filePath", targetPath));
                cmd.Parameters.Add(new SqlParameter("@version", fileName));
                cmd.Parameters.Add(new SqlParameter("@authorName", fileName));
                //cmd.Parameters.Add(new SqlParameter("@date", fi.));
                cmd.Parameters.Add(new SqlParameter("@resoursePermission", i));
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void insertToResourceCategoryTbl(int code,int i)
        {
            try
            {
                cmd = new SqlCommand("insert into ResourceCategory(resourceCode,categoryCode) values(@resourceCode,@categoryCode)", con);
                cmd.Parameters.Add(new SqlParameter("@resourceCode", code));
                cmd.Parameters.Add(new SqlParameter("@categoryCode", i));
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int getCounterResources()
        {
            int counter = 0;
            try
            {
                cmd = new SqlCommand("SELECT COUNT(*) FROM Resources", con);
                counter = (int)cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
            return counter;
        }
        public static bool SendEmailToUser(string subject, string body, string emailAddress)
        {
            using (System.Net.Mail.MailMessage mail = new MailMessage())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.Append(body).AppendLine();
                sb.AppendLine();
                try
                {
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress("hodiyoav@gmail.com");
                    mail.To.Add(emailAddress);
                    mail.Subject = subject;
                    string bodyStr = sb.ToString();
                    mail.Body = bodyStr;
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("hodiyoav@gmail.com", "56410525");
                    SmtpServer.EnableSsl = true;


                    SmtpServer.Send(mail);
                    return true;
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                    return false;
                }
            }


        }
    }
}

