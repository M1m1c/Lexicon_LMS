using AutoMapper;
using Lexicon_LMS.Models;
using Lexicon_LMS.ViewModels;
using Lexicon_LMS.ViewModels.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CourseCreateViewModel, Course>();
            CreateMap<Course, CourseDetailsViewModel>();
        }



    }
}
