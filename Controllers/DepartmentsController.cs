using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HW.Models;
using Microsoft.Data.SqlClient;

namespace HW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.Where(x => x.IsDeleted == false).ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentID)
            {
                return BadRequest();
            }

            var model = await _context.Department.FindAsync(id);
            if (model.IsDeleted == true)
            {
                return NotFound();
            }

            //_context.Entry(department).State = EntityState.Modified;
            //department.DateModified = DateTime.Now;

            try
            {
                //await _context.SaveChangesAsync();

                var DepartmentID = new SqlParameter("@DepartmentID", department.DepartmentID);
                var Name = new SqlParameter("@Name", department.Name);
                var Budget = new SqlParameter("@Budget", department.Budget);
                var StartDate = new SqlParameter("@StartDate", department.StartDate.ToString("yyyy/MM/dd"));
                var InstructorID = new SqlParameter("@InstructorID", department.InstructorID);
                var RowVersion = new SqlParameter("@RowVersion_Original", department.RowVersion);

                await _context.Database.ExecuteSqlRawAsync("Department_Update @DepartmentID, @Name, @Budget, @StartDate, @InstructorID, @RowVersion_Original"
                    , DepartmentID, Name, Budget, StartDate, InstructorID, RowVersion);

                //await _context.Sp_Department_Result
                //    .FromSqlRaw("EXEC Department_Update @DepartmentID, @Name, @Budget, @StartDate, @InstructorID, @RowVersion_Original"
                //    , DepartmentID, Name, Budget, StartDate, InstructorID, RowVersion)
                //    .ToListAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            //_context.Department.Add(department);
            //await _context.SaveChangesAsync();

            var result = await _context.Sp_Department_Result
            .FromSqlInterpolated($"Department_Insert {department.Name}, {department.Budget}, {department.StartDate}, {department.InstructorID}").ToListAsync();

            department.DepartmentID = Convert.ToInt32(result.FirstOrDefault().DepartmentID);
            return CreatedAtAction("GetDepartment", new { id = department.DepartmentID }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            //_context.Department.Remove(department);

            //_context.Entry(department).State = EntityState.Modified;
            //department.IsDeleted = true;

            //await _context.SaveChangesAsync();

            var DepartmentID = new SqlParameter("@DepartmentID", department.DepartmentID);
            var RowVersion = new SqlParameter("@RowVersion", department.RowVersion);
            await _context.Database.ExecuteSqlRawAsync("Department_Delete @DepartmentID, @RowVersion", DepartmentID, RowVersion);
            return department;
        }

        [Route("vwDepartmentCourseCount")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<vwDepartmentCourseCount>>> GetvwDepartmentCourseCount()
        {
            return await _context.vwDepartmentCourseCount.FromSqlRaw("SELECT * FROM vwDepartmentCourseCount").ToListAsync();
            //return await _context.vwDepartmentCourseCount.ToListAsync();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentID == id);
        }
    }
}
