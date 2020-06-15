using AutoMapper;
using Lexicon_LMS.Models;
using Lexicon_LMS.Models.ViewModels;
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
            CreateMap<User, UserViewModel>().ForMember(
                    dest => dest.CourseName,
                    from => from.MapFrom(u => u.Course.CourseName)
                    );
             
          //  CreateMap<UserViewModel, User>().ForMember(
            //        dest => dest.Course.CourseName,
           ////         from => from.MapFrom(u => u.CourseName)
              //  ).ForMember(
              //      dest => dest.UserName,
             //       from => from.MapFrom(u => u.Email)
             //   );
            CreateMap<User, EditUserViewModel>();
            CreateMap<ModuleViewModel, Module>();
            CreateMap<Module, ModuleViewModel>();
            CreateMap<CourseActivityViewModel, CourseActivity>();
            CreateMap<CourseActivity, CourseActivityViewModel>();
            CreateMap<DocumentViewModel, Document>();
            CreateMap<Document, DocumentViewModel>();

        }



    }
}
