using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Models
{
    public enum State
    {
        FirstCheck,
        RejectByHRM,
        InterviewWithHRM,
        RejectAfterInterviewWithHRM,
        TechInterview,
        RejectAfterTechInterview,
        ObtainDocumentsAndOfficialProcess,
        WaitingForDocumentAcception,
        SuccessfulSignUp,
        NeedDocumentsCorrection,
        EndInterviewWithHRM
    }
}
