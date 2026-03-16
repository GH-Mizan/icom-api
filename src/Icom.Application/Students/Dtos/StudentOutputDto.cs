using Icom.Enums;
using System;

namespace Icom.Students.Dtos
{
    public class StudentOutputDto
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public DateTime AdmisionDate { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public string BloodGroupText { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        public IccCourses? Course { get; set; }
        public string CourseName { get; set; }
        public CourseDuration Duration { get; set; }
        public int ClassRoll { get; set; }
        public string DurationText { get; set; }
        public int? BtebSessionId { get; set; }
        public string BtebSessionText { get; set; }
        public bool Bteb { get; set; }
        public bool BtebAdmitted { get; set; }
        public bool BtebRegistered { get; set; }
        public string BtebRegistrationNumber { get; set; }
        public bool DidExam { get; set; }
        public ResultStatus? ResultStatus { get; set; }
        public string ResultStatusText { get; set; }
        public bool IsSessionChanged { get; set; }
        public string Remarks { get; set; }
        public string Result { get; set; }
        public int CourseFee { get; set; }
        public int Discount { get; set; }
        public OfficePrograms RunningProgram { get; set; }
        public string RunningProgramText { get; set; }
        public bool IsActive { get; set; }
        public bool CourseCompleted { get; set; }
        public bool CertificateDistributed { get; set; }
        public int TenantId { get; set; }
    }
}
