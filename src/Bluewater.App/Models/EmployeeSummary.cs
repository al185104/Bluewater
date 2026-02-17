using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.App.Models;

public class EmployeeSummary : IRowIndexed
{
		public Guid Id { get; init; }
		public string FirstName { get; init; } = string.Empty;
		public string LastName { get; init; } = string.Empty;
		public string? MiddleName { get; init; }
		public DateTime? DateOfBirth { get; init; }
		public Gender Gender { get; init; }
		public CivilStatus CivilStatus { get; init; }
		public BloodType BloodType { get; init; }
		public Status Status { get; init; }
		public decimal? Height { get; init; }
		public decimal? Weight { get; init; }
		public bool IsDeleted { get; init; }
		public string? Remarks { get; init; }
		public int MealCredits { get; init; }
		public Tenant Tenant { get; init; }
		public string? Position { get; init; }
		public string? Section { get; init; }
		public string? Department { get; init; }
		public string? Type { get; init; }
		public string? Level { get; init; }
		public string? Image { get; init; }
		public Guid? UserId { get; init; }
		public Guid? PositionId { get; init; }
		public Guid? PayId { get; init; }
		public Guid? TypeId { get; init; }
		public Guid? LevelId { get; init; }
		public Guid? ChargingId { get; init; }
		public DateTime CreatedDate { get; init; }
		public Guid CreateBy { get; init; }
		public DateTime UpdatedDate { get; init; }
		public Guid UpdateBy { get; init; }
		public ContactInfoSummary ContactInfo { get; init; } = new();
		public EducationInfoSummary EducationInfo { get; init; } = new();
		public EmploymentInfoSummary EmploymentInfo { get; init; } = new();
		public UserSummary User { get; init; } = new();
		public PaySummary Pay { get; init; } = new();
		public int RowIndex { get; set; }

		public string FullName
		{
				get
				{
						return string.Join(' ', new[] { FirstName, MiddleName, LastName }
							.Where(part => !string.IsNullOrWhiteSpace(part)));
				}
		}

		public string? Email => ContactInfo.Email;

		public string PositionDisplay => FormatDetail("Position", Position);

		public string DepartmentDisplay => FormatDetail("Department", Department);

		public string SectionDisplay => FormatDetail("Section", Section);

		public string TypeLevelDisplay
		{
				get
				{
						if (string.IsNullOrWhiteSpace(Type) && string.IsNullOrWhiteSpace(Level))
						{
								return string.Empty;
						}

						if (string.IsNullOrWhiteSpace(Type))
						{
								return $"Level: {Level}";
						}

						if (string.IsNullOrWhiteSpace(Level))
						{
								return $"Type: {Type}";
						}

						return $"{Type} • {Level}";
				}
		}

		public string EmailDisplay => string.IsNullOrWhiteSpace(Email) ? string.Empty : Email;

		public bool HasImage => !string.IsNullOrWhiteSpace(Image);

		private static string FormatDetail(string label, string? value)
		{
				return string.IsNullOrWhiteSpace(value) ? string.Empty : $"{label}: {value}";
		}
}

public record class ContactInfoSummary
{
		public string? Email { get; init; }
		public string? TelNumber { get; init; }
		public string? MobileNumber { get; init; }
		public string? Address { get; init; }
		public string? ProvincialAddress { get; init; }
		public string? MothersMaidenName { get; init; }
		public string? FathersName { get; init; }
		public string? EmergencyContact { get; init; }
		public string? RelationshipContact { get; init; }
		public string? AddressContact { get; init; }
		public string? TelNoContact { get; init; }
		public string? MobileNoContact { get; init; }
}

public record class EducationInfoSummary
{
		public EducationalAttainment EducationalAttainment { get; init; }
		public string? CourseGraduated { get; init; }
		public string? UniversityGraduated { get; init; }
}

public record class EmploymentInfoSummary
{
		public DateTime? DateHired { get; init; }
		public DateTime? DateRegularized { get; init; }
		public DateTime? DateResigned { get; init; }
		public DateTime? DateTerminated { get; init; }
		public string? TinNo { get; init; }
		public string? SssNo { get; init; }
		public string? HdmfNo { get; init; }
		public string? PhicNo { get; init; }
		public string? BankAccount { get; init; }
		public bool HasServiceCharge { get; init; }
}

public record class UserSummary
{
		public string Username { get; init; } = string.Empty;
		public string PasswordHash { get; init; } = string.Empty;
		public Credential Credential { get; init; } = Credential.None;
		public Guid? SupervisedGroup { get; init; }
		public bool IsGlobalSupervisor { get; init; }
}

public record class PaySummary
{
		public decimal? BasicPay { get; init; }
		public decimal? DailyRate { get; init; }
		public decimal? HourlyRate { get; init; }
		public decimal? HdmfCon { get; init; }
		public decimal? HdmfEr { get; init; }
}
