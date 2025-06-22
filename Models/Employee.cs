using System;
using System.Collections.Generic;

namespace MyAPI.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public decimal Salary { get; set; }

    public int? DepartmentId { get; set; }

    public int? ManagerId { get; set; }
   }
