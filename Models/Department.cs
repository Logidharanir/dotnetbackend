using System;
using System.Collections.Generic;

namespace MyAPI.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;
}
