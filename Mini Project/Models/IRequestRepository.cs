using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Models
{
    public interface IRequestRepository
    {
        Request Add(Request request);
        IEnumerable<Request> GetAllRequests();
        Request GetRequestById(int id);
        Request Update(Request requestChanges);
        IEnumerable<Request> GetRequestsByState(State state);
    }
}
