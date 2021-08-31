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
<<<<<<< HEAD
        } 
=======
        }

        public Request GetRequestById(int id)
        {
            return context.Requests.Find(id); 
        }

         public Request Update(Request requestChanges)
        {
          var request = context.Requests.Attach(requestChanges);
          request.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          context.SaveChanges();
          return requestChanges;
        }
>>>>>>> 3a3f3b4286f475d0096b8ae1a4447c4bff20d0c0
    }
}
