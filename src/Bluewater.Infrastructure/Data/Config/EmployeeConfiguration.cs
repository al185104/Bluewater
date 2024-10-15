using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bluewater.Infrastructure.Data.Config;
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
  public void Configure(EntityTypeBuilder<Employee> builder)
  {
    // Key Configuration
    builder.HasKey(e => e.Id);

    // Property Configuration
    builder.Property(e => e.FirstName)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(e => e.LastName)
        .IsRequired()
        .HasMaxLength(100);

    builder.Property(e => e.MiddleName)
        .HasMaxLength(100);

    builder.Property(e => e.DateOfBirth);

    builder.Property(e => e.Gender)
        .HasConversion<int>(); // Assuming Gender is an enum

    builder.Property(e => e.CivilStatus)
        .HasConversion<int>(); // Assuming CivilStatus is an enum

    builder.Property(e => e.BloodType)
        .HasConversion<int>(); // Assuming BloodType is an enum

    builder.Property(e => e.Status)
        .HasConversion<int>(); // Assuming Status is an enum

    builder.Property(e => e.Height);

    builder.Property(e => e.Weight);

    builder.Property(e => e.IsDeleted);

    builder.Property(e => e.ImageUrl);

    builder.Property(e => e.Remarks)
        .HasMaxLength(500);

    // Timestamp Properties
    builder.Property(e => e.CreatedDate);

    builder.Property(e => e.CreateBy);

    builder.Property(e => e.UpdatedDate);

    builder.Property(e => e.UpdateBy);

    // Value Object Configuration for ContactInfo
    builder.OwnsOne(e => e.ContactInfo, ci =>
    {
      ci.Property(c => c.Email)
          .IsRequired()
          .HasMaxLength(200);

      ci.Property(c => c.TelNumber)
          .IsRequired()
          .HasMaxLength(20);

      ci.Property(c => c.MobileNumber)
          .IsRequired()
          .HasMaxLength(20);

      ci.Property(c => c.Address)
          .IsRequired()
          .HasMaxLength(300);

      ci.Property(c => c.ProvincialAddress)
          .IsRequired()
          .HasMaxLength(300);

      ci.Property(c => c.MothersMaidenName)
          .HasMaxLength(200);

      ci.Property(c => c.FathersName)
          .HasMaxLength(200);

      ci.Property(c => c.EmergencyContact)
          .HasMaxLength(200);

      ci.Property(c => c.RelationshipContact)
          .HasMaxLength(100);

      ci.Property(c => c.AddressContact)
          .HasMaxLength(300);

      ci.Property(c => c.TelNoContact)
          .HasMaxLength(20);

      ci.Property(c => c.MobileNoContact)
          .HasMaxLength(20);

      // Index on Email (Optional)
      ci.HasIndex(c => c.Email).IsUnique();
    });

    // Value Object Configuration for EmploymentInfo
    builder.OwnsOne(e => e.EmploymentInfo, ei =>
    {
      ei.Property(ei => ei.DateHired);
      ei.Property(ei => ei.DateRegularized);
      ei.Property(ei => ei.DateResigned);
      ei.Property(ei => ei.DateTerminated);
      ei.Property(ei => ei.TINNo)
          .HasMaxLength(20);
      ei.Property(ei => ei.SSSNo)
          .HasMaxLength(20);
      ei.Property(ei => ei.HDMFNo)
          .HasMaxLength(20);
      ei.Property(ei => ei.PHICNo)
          .HasMaxLength(20);
      ei.Property(ei => ei.BankAccount)
          .HasMaxLength(50);
      ei.Property(ei => ei.HasServiceCharge);
    });

    // Value Object Configuration for EducationInfo
    builder.OwnsOne(e => e.EducationInfo, edu =>
    {
        edu.Property(e => e.EducationalAttainment)
            .HasConversion<int>(); // Assuming EducationalAttainment is an enum

        edu.Property(e => e.CourseGraduated)
            .HasMaxLength(200);

        edu.Property(e => e.UniversityGraduated)
            .HasMaxLength(200);
    //   edu.Property(e => e.PrimarySchool)
    //       .IsRequired()
    //       .HasMaxLength(200);

    //   edu.Property(e => e.SecondarySchool)
    //       .IsRequired()
    //       .HasMaxLength(200);

    //   edu.Property(e => e.TertiarySchool)
    //       .IsRequired()
    //       .HasMaxLength(200);

    //   edu.Property(e => e.VocationalSchool)
    //       .IsRequired()
    //       .HasMaxLength(200);

    //   edu.Property(e => e.PrimaryDegree)
    //       .HasMaxLength(200);

    //   edu.Property(e => e.SecondaryDegree)
    //       .HasMaxLength(200);

    //   edu.Property(e => e.TertiaryDegree)
    //       .HasMaxLength(200);

    //   edu.Property(e => e.VocationalDegree)
    //       .HasMaxLength(200);
    });

    // Foreign Key and Relationship Configuration

    // EmployeeType Relationship
    builder.HasOne(e => e.Type)
        .WithMany()
        .HasForeignKey(e => e.TypeId)
        .OnDelete(DeleteBehavior.Restrict);

    // Position Relationship
    builder.HasOne(e => e.Position)
        .WithMany()
        .HasForeignKey(e => e.PositionId)
        .OnDelete(DeleteBehavior.Restrict);

    // Shift Relationship (Assuming you have a Shift entity)
    //builder.HasOne<Shift>()
    //    .WithMany()
    //    .HasForeignKey(e => e.ShiftId)
    //    .OnDelete(DeleteBehavior.Restrict);

    // Pay Relationship
    builder.HasOne(e => e.Pay)
        .WithMany()
        .HasForeignKey(e => e.PayId)
        .OnDelete(DeleteBehavior.Restrict);

    // Charging Relationship (Adjust based on your model)
    // Assuming you have a Charging entity and navigation property
    builder.HasOne(e => e.Charging)
        .WithMany()
        .HasForeignKey(e => e.ChargingId)
        .OnDelete(DeleteBehavior.Restrict);

    // Level Relationship
    builder.HasOne(e => e.Level)
        .WithMany()
        .HasForeignKey("LevelId") // Shadow property if LevelId is not in Employee
        .OnDelete(DeleteBehavior.Restrict);

    // User Relationship (Assuming one-to-one with User entity)
    builder.HasOne(e => e.User)
        .WithOne(u => u.Employee)
        .HasForeignKey<User>(u => u.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict);

    // Collections Configuration

    // Dependents Relationship
    builder.HasMany(e => e.Dependents)
        .WithOne(d => d.Employee)
        .HasForeignKey(d => d.EmployeeId)
        .OnDelete(DeleteBehavior.Cascade);

    // Indexes (Optional)
    builder.HasIndex(e => new { e.FirstName, e.LastName });
  }
}
