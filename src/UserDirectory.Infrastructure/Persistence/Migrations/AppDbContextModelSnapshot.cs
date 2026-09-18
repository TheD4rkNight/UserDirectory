using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserDirectory.Infrastructure.Persistence;

#nullable disable

namespace UserDirectory.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
sealed partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.20")
            .HasAnnotation("Relational:MaxIdentifierLength", 64);

        modelBuilder.Entity("UserDirectory.Domain.Entities.User", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<int>("Age")
                .HasColumnType("INTEGER");

            b.Property<string>("City")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("TEXT");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("TEXT");

            b.Property<string>("Pincode")
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("TEXT");

            b.Property<string>("State")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("TEXT");

            b.HasKey("Id");
            b.ToTable("Users");
        });
#pragma warning restore 612, 618
    }
}
