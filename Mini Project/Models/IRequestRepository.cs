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
<<<<<<< HEAD
        IEnumerable<Request> GetRequestsByState(State state);
=======
        Request GetRequestByFollowUpCode(string code);
>>>>>>> 3c40af269bf5c79b8b1e276acb94903c6d95fb19
    }
}
