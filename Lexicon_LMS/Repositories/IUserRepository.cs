using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Repositories
{
    public interface IUserRepository
    {
        void AddStudentToCourse(string id, int courseId);
   
    }
}
