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
    }
}
