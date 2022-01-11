using SalesWebMvc.Data;
using SalesWebMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SalesWebMvc.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMvcContext _context;

        public SalesRecordService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }

            return await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public Dictionary<Department, List<SalesRecord>> FindByDateGrouping(DateTime? minDate, DateTime? maxDate)
        {
            var departments = _context.Department.ToList();
            var sales = this.FindByDateAsync(minDate, maxDate);
            var result = new Dictionary<Department, List<SalesRecord>>();

            foreach (var department in departments)
            {
                foreach (var sale in sales.Result)
                {
                    if (!result.ContainsKey(department))
                    {
                        result.Add(department, new List<SalesRecord>());
                    }
                    if (sale.Seller.Department == department)
                    {
                        result[department].Add(sale);
                    }
                }
            }

            return result;

        }
    }
}
