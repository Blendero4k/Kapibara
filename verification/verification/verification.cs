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
    }

}
