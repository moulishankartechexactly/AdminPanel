using System.Collections.Generic;

namespace Management.Admin.Models;

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public IDictionary<string, int> ProductsPerCategory { get; set; } = new Dictionary<string, int>();
}
