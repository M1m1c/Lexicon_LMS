using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Repositories
{
    public interface IUnitOfWork
    {
        ICourseRepository CourseRepository { get; }
        IUserRepository UserRepository { get; set; }
        Task CompleateAsync(); 
    }
}
