using AutoMapper;
using Lexicon_LMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Repositories
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext db;
        private IMapper mapper;

        public UserRepository(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public void AddStudentToCourse(string id, int courseId)
        {
      
        }
    }
}
