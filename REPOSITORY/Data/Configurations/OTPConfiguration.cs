using CORE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOSITORY.Data.Configurations
{
    public class OTPConfiguration : IEntityTypeConfiguration<OTP>
    {
        public void Configure(EntityTypeBuilder<OTP> builder)
        {
            builder.Property(o => o.OTPCode)
                   .IsRequired()
                   .HasMaxLength(4);
            builder.Property(o => o.Email)
                   .IsRequired();
            builder.Property(o => o.ExpiryDate)
                   .IsRequired();
            builder.Property(o => o.IsUsed).HasDefaultValue(false);

        }
    }
}
