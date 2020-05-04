using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneToucheCare.Models
{
    public partial class db_oneTouchCareContext : DbContext
    {


        public db_oneTouchCareContext(DbContextOptions<db_oneTouchCareContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AccountType> AccountType { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Disease> Disease { get; set; }
        public virtual DbSet<Doctor> Doctor { get; set; }
        public virtual DbSet<DoctorImages> DoctorImages { get; set; }
        public virtual DbSet<Medicaments> Medicaments { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientDisease> PatientDisease { get; set; }
        public virtual DbSet<PatientDiseaseMedicaments> PatientDiseaseMedicaments { get; set; }
        public virtual DbSet<PatientTest> PatientTest { get; set; }
        public virtual DbSet<PatientTestImages> PatientTestImages { get; set; }
        public virtual DbSet<Profession> Profession { get; set; }
        public virtual DbSet<TestType> TestType { get; set; }
        public virtual DbSet<PatientDoctor> PatientDoctor { get; set; }
        public virtual DbSet<PatientSurgeries> PatientSurgeries { get; set; }
        public virtual DbSet<SurgeryType> SurgeryType { get; set; }
        public virtual DbSet<SurgeryImages> SurgeryImages { get; set; }
        public virtual DbSet<Appointments> Appointments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=pc-12451;Database=db_oneTouchCare;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Password).HasMaxLength(100);
                entity.Property(e => e.RecoveryAnswer).HasMaxLength(200);
                entity.Property(e => e.RecoveryQuestion).HasMaxLength(200);
                entity.Property(e => e.UserName).HasMaxLength(100);
                entity.Property(e => e.IsBlocked).HasColumnType("bit").HasColumnName("IsBlocked");
                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.AccountTypeId)
                    .HasConstraintName("FK_Account_AccountType");
            });

            modelBuilder.Entity<SurgeryImages>(entity=>{
                entity.HasKey(e => e.SurgeryImageId);
                entity.Property(e => e.ImageName).HasMaxLength(100);
                entity.Property(e => e.SurgeryImage).
                HasColumnName("SurgeryImage").
                HasColumnType("varbinary(max)");
                entity.HasOne(e => e.PatientSurgeries).
                WithMany(x => x.SurgeryImages).
                HasForeignKey(e => e.SurgeryId).
                HasConstraintName("FK_SurgeryImages_PatientSurgeries");
            });
            modelBuilder.Entity<SurgeryType>(entity =>
            {
                entity.HasKey(e => e.SurgeryTypeId);
                entity.Property(e => e.SocialSecurityNumber).HasColumnName("SocialSecurityNumber").HasColumnType("nvarchar(100)").HasMaxLength(100);
                entity.Property(e => e.SurgeryTypeDescription).HasColumnName("SurgeryTypeDescription").HasColumnType("nvarchar(250)").HasMaxLength(250);
                entity.Property(e => e.SurgeryTypeName).HasColumnName("SurgeryTypeName").HasColumnType("nvarchar(150)").HasMaxLength(150);
            });

            modelBuilder.Entity<PatientSurgeries>(entity =>
            {
                entity.HasKey(e => e.SurgeryId);
                entity.Property(e => e.DoctorName).HasMaxLength(100);
                entity.Property(e => e.SurgeryDate).HasColumnName("SurgeryDate").HasColumnType("date");
                entity.Property(e => e.SurgeryName).HasMaxLength(100);
                entity.Property(e => e.SurgeryResult).HasMaxLength(300);
                entity.Property(e => e.SurgeryDescription).HasMaxLength(300);
                entity.Property(e => e.SurgeryTypeId).HasColumnType("int");
                entity.Property(e => e.DoctorId).HasColumnType("int");
                entity.Property(e => e.SurgeryTypeId).HasColumnType("int");
                entity.Property(e => e.PatientId).HasColumnType("int");

                entity.HasOne(e => e.Patient).WithMany(x => x.PatientSurgeries).HasForeignKey(w => w.PatientId).HasConstraintName("FK_PatientSurgeries_Patient");
                entity.HasOne(e => e.SurgeryType).WithMany(x => x.PatientSurgeries).HasForeignKey(w => w.SurgeryTypeId).HasConstraintName("FK_PatientSurgeries_SurgeryType");
                entity.HasOne(e => e.Doctor).WithMany(x => x.PatientSurgeries).HasForeignKey(w => w.DoctorId).HasConstraintName("FK_PatientSurgeries_Doctor");
            });

            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.Property(e => e.AccountTypeId).ValueGeneratedNever();
                entity.Property(e => e.AccountTypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Disease>(entity =>
            {
                entity.Property(e => e.DiseaseName).HasMaxLength(200);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Doctor)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Doctor_Account");

                entity.HasOne(d => d.NationalityNavigation)
                    .WithMany(p => p.Doctor)
                    .HasForeignKey(d => d.Nationality)
                    .HasConstraintName("FK_Doctor_Country");

                entity.HasOne(d => d.Profession)
                    .WithMany(p => p.Doctor)
                    .HasForeignKey(d => d.ProfessionId)
                    .HasConstraintName("FK_Doctor_Profession");
            });

            modelBuilder.Entity<DoctorImages>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.Property(e => e.ImageName).HasMaxLength(100);

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.DoctorImages)
                    .HasForeignKey(d => d.DoctorId)
                    .HasConstraintName("FK_DoctorImages_Doctor");
            });

            modelBuilder.Entity<Medicaments>(entity =>
            {
                entity.HasKey(e => e.MedicamentId);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.FatherName).HasMaxLength(100);
                entity.Property(e => e.MotherName).HasMaxLength(200);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Patient)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Patient_Account");

                entity.HasOne(d => d.NationalityNavigation)
                    .WithMany(p => p.Patient)
                    .HasForeignKey(d => d.Nationality)
                    .HasConstraintName("FK_Patient_Country");
            });

            modelBuilder.Entity<PatientDisease>(entity =>
            {
                entity.Property(e => e.DateOfDisease).HasColumnType("date");

                entity.Property(e => e.DateOfFinish).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.DoctorName).HasMaxLength(100).HasColumnName("DoctorName");
                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.PatientDisease)
                    .HasForeignKey(d => d.DiseaseId)
                    .HasConstraintName("FK_PatientDisease_Disease");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientDisease)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK_PatientDisease_Patient");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.PatientDisease)
                    .HasForeignKey(d => d.DoctorId)
                    .HasConstraintName("FK_PatientDisease_Doctor");
            });

            modelBuilder.Entity<PatientDiseaseMedicaments>(entity =>
            {
                entity.HasKey(e => e.PatientDiseaseMedicamentId);
                entity.ToTable("PatientDisease_Medicaments");
                entity.Property(e => e.DateFinish).HasColumnType("date");
                entity.Property(e => e.DateStart).HasColumnType("date");
                entity.Property(e => e.Dose).HasMaxLength(10);
                entity.Property(e => e.TimesPerDay).HasMaxLength(5);
                entity.HasOne(d => d.Medicament)
                    .WithMany(p => p.PatientDiseaseMedicaments)
                    .HasForeignKey(d => d.MedicamentId)
                    .HasConstraintName("FK_PatientDisease_Medicaments_Medicaments");
                entity.HasOne(d => d.PatientDisease)
                    .WithMany(p => p.PatientDiseaseMedicaments)
                    .HasForeignKey(d => d.PatientDiseaseId)
                    .HasConstraintName("FK_PatientDisease_Medicaments_PatientDisease");
            });

            modelBuilder.Entity<PatientTest>(entity =>
            {
                entity.HasKey(e => e.PatientTestId);

                entity.Property(e => e.Deduction).HasMaxLength(200);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Result).HasMaxLength(200);

                entity.Property(e => e.TestDate).HasColumnType("date");

                entity.Property(e => e.TestName).HasMaxLength(100);

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientTest)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK_PatientTest_Patient");

                entity.HasOne(p => p.PatientDisease)
                .WithMany(p => p.PatientTest)
                .HasForeignKey(e => e.PatientDiseaseId)
                .HasConstraintName("FK_PatientTest_PatientDisease");

                entity.HasOne(d => d.TestType)
                    .WithMany(p => p.PatientTest)
                    .HasForeignKey(d => d.TestTypeId)
                    .HasConstraintName("FK_PatientTest_TestType");
            });

            modelBuilder.Entity<PatientTestImages>(entity =>
            {
                entity.HasKey(e => e.PatientTestImageId);

                entity.Property(e => e.Image).HasColumnName("Image").HasColumnType("Varbinary(max)");

                entity.HasOne(d => d.PatientTest)
                    .WithMany(p => p.PatientTestImages)
                    .HasForeignKey(d => d.PatientTestId)
                    .HasConstraintName("FK_PatientTestImages_PatientTest");
            });

            modelBuilder.Entity<Profession>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<TestType>(entity =>
            {
                entity.HasKey(e => e.TestTypeId);
                entity.Property(e => e.TestTypeName)
                    .HasColumnName("testTypeName")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<PatientDoctor>(entity =>
            {
                entity.HasKey(e => e.PatientDoctorId);

                entity.HasOne(e => e.Doctor).
                WithMany(p => p.PatientDoctor).
                HasForeignKey(d => d.DoctorId).
                HasConstraintName("FK_PatientDoctor_Doctor");

                entity.HasOne(e => e.Patient).
                WithMany(p => p.PatientDoctor).
                HasForeignKey(e => e.PatientId).
                HasConstraintName("FK_PatientDoctor_Patient");
            });

            modelBuilder.Entity<Appointments>(entity =>
            {
                entity.HasKey(e => e.AppointmentId);
                entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
                entity.Property(e => e.IsConfirmed).HasColumnType("bit");
                entity.Property(e => e.IsRequest).HasColumnType("bit");
                entity.Property(e => e.PatientFullName).HasColumnType("nvarchar(100)").HasMaxLength(100);
                entity.Property(e => e.Title).HasColumnType("nvarchar(200)").HasMaxLength(200);
                entity.Property(e => e.Subject).HasColumnType("nvarchar(500)").HasMaxLength(500);
                entity.HasOne(d => d.Doctor).WithMany(a => a.Appointments).HasForeignKey(d => d.DoctorId).HasConstraintName("FK_Appointments_Doctor");
                entity.HasOne(p => p.Patient).WithMany(a => a.Appointments).HasForeignKey(p => p.PatientId).HasConstraintName("FK_Appointments_Patient");
            });

        }
    }
}
