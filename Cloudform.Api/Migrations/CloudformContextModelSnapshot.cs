﻿// <auto-generated />
using Cloudform.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cloudform.Api.Migrations
{
    [DbContext(typeof(CloudformContext))]
    partial class CloudformContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity("Cloudform.Api.Models.Build", b =>
                {
                    b.Property<int>("BuildId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Event");

                    b.Property<int>("EventId");

                    b.HasKey("BuildId");

                    b.ToTable("Builds");
                });
#pragma warning restore 612, 618
        }
    }
}