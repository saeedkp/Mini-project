using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Models
{
    public class SQLRequestRepository : IRequestRepository
    {
        private readonly AppDbContext context;

        public SQLRequestRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Request Add(Request request)
        {
            context.Requests.Add(request);
            context.SaveChanges();
            return request;
        }

        public IEnumerable<Request> GetAllRequests()
        {
            return context.Requests;
        }

        public Request GetRequestById(int id)
        {
            return context.Requests.Find(id); 
        }

        public IEnumerable<Request> GetRequestsByState(State state)
        {
            IEnumerable<Request> requests = GetAllRequests();
            IEnumerable<Request> filterdRequests = requests.Where(request => request.state == state);
            return filterdRequests;
        }

        public Request Update(Request requestChanges)
        {
          var request = context.Requests.Attach(requestChanges);
          request.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          context.SaveChanges();
          return requestChanges;
        }

        public Request GetRequestByFollowUpCode(string code)
        {
            return context.Requests.FirstOrDefault(r => r.followUpCode == code) ?? null;
        }

        public IEnumerable<Request> GetRequestByEmail(string email)
        {
            IEnumerable<Request> requests = GetAllRequests();
            IEnumerable<Request> filterdRequests = requests.Where(request => request.Email == email);
            return filterdRequests;
        }
    }
}
