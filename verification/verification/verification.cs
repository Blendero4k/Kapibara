using Microsoft.Data.SqlClient;
using System.Data;
namespace verification
{
    public class verification
    {
        public string Enter(string login,string password)
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
                catch (Exception ex)
                {
                    return "Системная ошибка! Попробуйте зайти позже";
                }
            }
            switch (result)
            {
                case null:
                    return "Неверно введён логин или пароль!";
                case 1:
                    return "";
                default:
                    using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=main;Integrated Security=True;TrustServerCertificate=True;"))
                    {
                        try
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("INSERT INTO Errors ([level],description) VALUES (101,@Desc)", conn);
                            cmd.Parameters.Add("Desc", SqlDbType.NVarChar).Value = $"Пользователь {login} имеет несколько " +
                                $"одинковых учётных записей общим количеством: {result}";
                            cmd.ExecuteNonQuery();
                        }
                        catch(Exception ex)
                        {
                            return "Системная ошибка! Попробуйте зайти позже";
                        }
                    }
                    return "Системная ошибка! Попробуйте зайти позже";
            }
        }
        public string Registration(string login,string password,string name)
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
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return "Системная ошибка! Попробуйте зайти позже";
                }
            }
        }
    }

}
