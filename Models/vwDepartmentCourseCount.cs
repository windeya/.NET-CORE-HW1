﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HW.Models
{
    public partial class vwDepartmentCourseCount
    {
        public int DepartmentID { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public int? CourseCount { get; set; }
    }
}