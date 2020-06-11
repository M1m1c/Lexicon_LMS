using AutoMapper;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICourseRepository CourseRepository { get; private set; }
        public IUserRepository UserRepository { get; set; }

        public UnitOfWork(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            CourseRepository = new CourseRepository(db, mapper);
            UserRepository = new UserRepository(db, mapper);
        }

        public async Task CompleateAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
