using AutoMapper;
using Icom.Entities;
using Icom.Services.Dtos;

namespace Icom.Students.Dtos
{
    public class StudentMapProfile : Profile
    {
        public StudentMapProfile()
        {
            CreateMap<StudentEntryInputDto, Student>();
            CreateMap<Student, StudentEntryInputDto>();
        }
    }
}
