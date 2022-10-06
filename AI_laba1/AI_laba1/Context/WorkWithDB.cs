using AI_laba1.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_laba1.Controllers
{
    public class WorkWithDB
    {
        public static readonly string strConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Transports;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        public static List<Question> ReadAllRow()
        {
            string textCommand = "SELECT * FROM Question";
            var list = new List<Question>();
            using (SqlConnection connection = new SqlConnection(strConnection))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand(textCommand,connection);
                var reader = sqlCommand.ExecuteReader();
                list = StringToQuestion(reader);

                connection.Close();
            }

            return list;
        }

        private static List<Question> StringToQuestion(SqlDataReader reader)
        {
            var resultList = new List<Question>();
            while(reader.Read())
            {
                var quest = new Question();
                quest.Id = int.Parse(reader.GetValue(0).ToString());
                quest.Tr = reader.GetValue(1).ToString();
                quest.Quest = reader.GetValue(2).ToString();
                quest.Answer = reader.GetValue(3).ToString();

                resultList.Add(quest);
            }
            return resultList;
        }

        public static void SaveQuestion(Question question)
        {
            string textCommand = $"insert into dbo.Question values(N'{question.Tr}', N'{question.Quest}', N'{question.Answer}')";
            using (SqlConnection connection = new SqlConnection(strConnection))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand(textCommand, connection);
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }
       
        public static List<Question> GetOneQuestion(string nameTr)
        {
            
            string textCommand = $"select * from Question where Tr = N'{nameTr}'";
            var list = new List<Question>();
            using (SqlConnection connection = new SqlConnection(strConnection))
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand(textCommand, connection);
                var reader = sqlCommand.ExecuteReader();
                list = StringToQuestion(reader);
                connection.Close();
            }
            return list;
        }


    }
}
