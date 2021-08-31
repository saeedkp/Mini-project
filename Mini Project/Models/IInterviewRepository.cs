using System.Collections.Generic;

namespace Mini_Project.Models
{
    public interface IInterviewRepository
    {
         Interview Add(Interview interview);
         Interview GetInterview(int id);
         Interview Update (Interview interview);
         IEnumerable<Interview> GetAllInterview();
    }
}