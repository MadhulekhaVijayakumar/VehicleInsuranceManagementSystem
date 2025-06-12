using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Context
{
    public class InsuranceManagementContext : DbContext
    {
        public InsuranceManagementContext(DbContextOptions options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<InsuranceDetails> InsuranceDetails { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<InsuranceClaim> InsuranceClaims { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------------- USER RELATIONSHIPS ----------------

            // Ensure Username is PK
            modelBuilder.Entity<User>()
                .HasKey(u => u.Username);

            // One-to-one: Client.Email (FK) → User.Username (PK)
            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithOne(u => u.Client)
                .HasForeignKey<Client>(c => c.Email)          // Email is FK in Client
                .HasPrincipalKey<User>(u => u.Username)       // Username is PK in User
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Ensure Email is unique in Client
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Email)
                .IsUnique();


            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.Email)
                .HasPrincipalKey<User>(u => u.Username)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- CLIENT PROPERTIES ----------------

            modelBuilder.Entity<Client>()
                .Property(c => c.DateOfBirth)
                .HasColumnType("date");

            modelBuilder.Entity<Client>()
                .Property(c => c.AadhaarNumber)
                .IsRequired();

            modelBuilder.Entity<Client>()
                .Property(c => c.PANNumber)
                .IsRequired();

            modelBuilder.Entity<Client>()
                .Property(c => c.Address)
                .IsRequired();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.AadhaarNumber)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.PANNumber)
                .IsUnique();

            // ---------------- VEHICLE CONFIG ----------------

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Client)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VehicleNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.ChassisNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.EngineNumber)
                .IsUnique();

            // ---------------- PROPOSAL CONFIG ----------------

            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Vehicle)
                .WithMany(v => v.Proposals)
                .HasForeignKey(p => p.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Client)
                .WithMany(c => c.Proposals)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proposal>()
                .Property(p => p.FitnessValidUpto)
                .HasColumnType("date");

            modelBuilder.Entity<Proposal>()
                .Property(p => p.InsuranceValidUpto)
                .HasColumnType("date");

            // ---------------- INSURANCE DETAILS ----------------

            modelBuilder.Entity<InsuranceDetails>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.InsuranceStartDate)
                      .HasColumnType("date");

                entity.Property(i => i.InsuranceSum)
                      .HasColumnType("decimal(18,2)");

                entity.Property(i => i.DamageInsurance)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(i => i.LiabilityOption)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(i => i.Plan)
                      .HasMaxLength(20)
                      .IsRequired();

                // One-to-One: Proposal → InsuranceDetails
                entity.HasOne(i => i.Proposal)
                      .WithOne(p => p.InsuranceDetails)
                      .HasForeignKey<InsuranceDetails>(i => i.ProposalId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: Vehicle → InsuranceDetails
                entity.HasOne(i => i.Vehicle)
                      .WithMany(v => v.InsuranceDetails)
                      .HasForeignKey(i => i.VehicleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- INSURANCE CONFIG ----------------
            modelBuilder.Entity<Insurance>()
                  .HasKey(i => i.InsurancePolicyNumber);

            modelBuilder.Entity<Proposal>()
                        .HasOne(p => p.Insurance)
                        .WithOne(i => i.Proposal)
                        .HasForeignKey<Insurance>(i => i.ProposalId);
            modelBuilder.Entity<Insurance>()
                        .HasOne(i => i.Client)
                        .WithMany(c => c.Insurances)
                        .HasForeignKey(i => i.ClientId);
            modelBuilder.Entity<Insurance>()
                            .HasOne(i => i.Vehicle)
                            .WithMany(v => v.Insurances)
                            .HasForeignKey(i => i.VehicleId);



            modelBuilder.Entity<Insurance>()
                   .Property(i => i.PremiumAmount)
                   .HasPrecision(18, 2); // Or whatever you need
            modelBuilder.Entity<Insurance>()
                      .Property(i => i.InsuranceSum)
                      .HasPrecision(18, 2);

            //payment
            modelBuilder.Entity<Payment>()
                         .HasOne(p => p.Proposal)
                         .WithMany(p => p.Payments)
                         .HasForeignKey(p => p.ProposalId);

            modelBuilder.Entity<Payment>()
                .Property(p => p.AmountPaid)
                .HasPrecision(10, 2);
            //----Documents----
            modelBuilder.Entity<Document>()
           .HasOne(d => d.Proposal)
           .WithMany(p => p.Documents)
           .HasForeignKey(d => d.ProposalId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                        .HasOne(d => d.Claim)
                        .WithMany(c => c.Documents)
                        .HasForeignKey(d => d.ClaimId)
                        .OnDelete(DeleteBehavior.Cascade);

            //InsuranceClaim
            // ---- InsuranceClaim ----
            modelBuilder.Entity<InsuranceClaim>()
                .HasKey(c => c.ClaimId);

            modelBuilder.Entity<InsuranceClaim>()
                .Property(c => c.InsurancePolicyNumber)
                .IsRequired();

            modelBuilder.Entity<InsuranceClaim>()
                .Property(c => c.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<InsuranceClaim>()
                .Property(c => c.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<InsuranceClaim>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<InsuranceClaim>()
                .HasOne(c => c.Insurance)
                .WithMany(i => i.Claims)
                .HasForeignKey(c => c.InsurancePolicyNumber)
                .HasPrincipalKey(i => i.InsurancePolicyNumber)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InsuranceClaim>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Claim)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
