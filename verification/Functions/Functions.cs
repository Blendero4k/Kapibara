using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net;
using System.Windows;
namespace Functions
{
    public static class Constants
    {
        public static List<Message> stackMessages = new List<Message>();
        public static string login = "";
    }
    public class Message
    {
        int? identificate = null;
        string text = null;
        int? idSender = null;
        int? idConfirence = null;
        public Message(int identificator,string TextMessage,int idSenderMessage,int Confirence) 
        {
            identificate = identificator;
            text = TextMessage;
            idSender = idSenderMessage;
            idConfirence = Confirence; 
        }
    }
    public static class Functions
    {
        private static string[] Pass = new string[2];
        public static string Enter(string login,string password)
        {
            int? result;
            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=main;Integrated Security=True;TrustServerCertificate=True;"))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SElECT COUNT(*) from Users where login=@LOG AND password=@PASS", conn);
                    cmd.Parameters.Add("LOG", SqlDbType.NVarChar).Value = login;
                    cmd.Parameters.Add("PASS", SqlDbType.NVarChar).Value = password;
                    result = (int?)cmd.ExecuteScalar();
                }
                catch
                {
                    return "Системная ошибка! Попробуйте зайти позже";
                }
            }
            switch (result)
            {
                case null:
                    return "Неверно введён логин или пароль!";
                case 1:
                    Constants.login = login;
                    return "";
                default:
                    using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=main;Integrated Security=True;TrustServerCertificate=True;"))
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand("INSERT INTO Errors ([level],description) VALUES (101,@Desc)", conn);
                            cmd.Parameters.Add("Desc", SqlDbType.NVarChar).Value = $"Пользователь {login} имеет несколько " +
                                $"одинковых учётных записей общим количеством: {result}";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            return "Системная ошибка! Попробуйте зайти позже";
                        }
                    }
                    return "Системная ошибка! Попробуйте зайти позже";
            }
        }
        public static string Registration(string login,string password,string name)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrWhiteSpace(login)) return "Введите логин";
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) return "Введите пароль";
            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=main;Integrated Security=True;TrustServerCertificate=True;"))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Users (login,password,name,permission) VALUES (@Log,@Pass,@Name,@Per)", conn);
                    cmd.Parameters.Add("Log", SqlDbType.NVarChar).Value = login;
                    cmd.Parameters.Add("Pass", SqlDbType.NVarChar).Value = password;
                    cmd.Parameters.Add("Name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("Per", SqlDbType.Bit).Value = null;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        SqlCommand cmd1 = new SqlCommand("INSERT INTO Errors ([level],description) VALUES (102,@Desc)", conn);
                        cmd1.Parameters.Add("Desc", SqlDbType.NVarChar).Value = $"Не удаётся создать аккаунт информация: логин: {login}, пароль: {password}, имя: {name}";
                        cmd1.ExecuteNonQuery();
                    }
                    return "";
                }
                catch
                {
                    return "Системная ошибка! Попробуйте зайти позже";
                }
            }
        }
        public static string AutoLoadMessage(int idLastMessage,int idConfirence)
        {
            string result = null;
            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=main;Integrated Security=True;TrustServerCertificate=True;"))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT FROM {idConfirence} WHERE idMEssage<@ID", conn);
                    cmd.Parameters.Add("ID", SqlDbType.NVarChar).Value = idLastMessage;
                    result = (string)cmd.ExecuteScalar();
                    return result;
                }
                catch
                {
                    return "Системная ошибка! Попробуйте зайти позже";
                }
            }
        }
        public static bool LoadMessageToStack(string message)
        {
            try
            {
                int identifitator = 0;
                string text = "";
                int idSender = 0;
                int idCorrespondence = 0;

                string identifitatorSTR = "";
                string idCorrespondenceSTR = "";
                string idSenderSTR = "";
                for (int i = 0, j = 0; i < message.Length; i++)
                {
                    if ((int)message[i] == 00)
                    {
                        j++;
                        continue;
                    }
                    if (j == 0)
                    {
                        identifitatorSTR += message[i];
                    }
                    else if (j == 1)
                    {
                        text += message[i];
                    }
                    else if (j == 2)
                    {
                        idSenderSTR += message[i];
                    }
                    else
                    {
                        idCorrespondenceSTR += message[i];
                    }
                }
                identifitator = Convert.ToInt32(identifitatorSTR);
                idSender = Convert.ToInt32(idSenderSTR);
                idCorrespondence = Convert.ToInt32(idCorrespondenceSTR);
                Message message1 = new Message(identifitator, text, idSender, idCorrespondence);
                Constants.stackMessages.Add(message1);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static string WriteMyIP()
        {
            string ip = new WebClient().DownloadString("http://api.ipify.org");
            File.WriteAllText("MyIP.txt", ip);
            return ip;
        }
        public static string ReadMyIP()
        {
            string content;
            if (File.Exists("MyIP.txt"))
            {
                content = File.ReadAllText("MyIP.txt");
            }
            else
            {
                WriteMyIP();
                ReadMyIP();
                return "";
            }
            if(content!= new WebClient().DownloadString("http://api.ipify.org"))
            {
                WriteMyIP();
                return new WebClient().DownloadString("http://api.ipify.org");
            }
            return "";
        }
    }

}
