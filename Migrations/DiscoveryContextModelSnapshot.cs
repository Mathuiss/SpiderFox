﻿// <auto-generated />
using InternetExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SpiderFox.Migrations
{
    [DbContext(typeof(DiscoveryContext))]
    partial class DiscoveryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity("InternetExplorer.Discovery", b =>
                {
                    b.Property<long>("Id");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Discoverys");
                });
#pragma warning restore 612, 618
        }
    }
}
