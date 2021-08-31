using System.Collections.Generic;

namespace Mini_Project.Models
{
    public class SQLInterviewRepository:IInterviewRepository
    {
        private readonly AppDbContext context;

        public SQLInterviewRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Interview Add(Interview interview)
        {
            context.interviews.Add(interview);
            context.SaveChanges();
            return interview;
        }

        public IEnumerable<Interview> GetAllInterview()
        {
            return context.interviews;
        }

        public Interview GetInterview(int Id)
        {
            return context.interviews.Find(Id);
        }

        public Interview Update(Interview interviewChange)
        {
            var interview = context.interviews.Attach(interviewChange);
            interview.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return interviewChange;
        }
    }
}