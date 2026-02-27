using System;

namespace Bluewater.App.Models;

public class DepartmentSummary : IRowIndexed
{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public Guid DivisionId { get; set; }
		public string? DivisionName { get; set; }
		public string? DivisionDescription { get; internal set; }
		public int RowIndex { get; set; }
}
