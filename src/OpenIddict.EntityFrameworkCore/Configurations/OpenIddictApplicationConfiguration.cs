﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;

namespace OpenIddict.EntityFrameworkCore
{
    /// <summary>
    /// Defines a relational mapping for the Application entity.
    /// </summary>
    /// <typeparam name="TApplication">The type of the Application entity.</typeparam>
    /// <typeparam name="TAuthorization">The type of the Authorization entity.</typeparam>
    /// <typeparam name="TToken">The type of the Token entity.</typeparam>
    /// <typeparam name="TKey">The type of the Key entity.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class OpenIddictApplicationConfiguration<TApplication, TAuthorization, TToken, TKey> : IEntityTypeConfiguration<TApplication>
        where TApplication : OpenIddictApplication<TKey, TAuthorization, TToken>
        where TAuthorization : OpenIddictAuthorization<TKey, TApplication, TToken>
        where TToken : OpenIddictToken<TKey, TApplication, TAuthorization>
        where TKey : IEquatable<TKey>
    {
        public void Configure([NotNull] EntityTypeBuilder<TApplication> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Warning: optional foreign keys MUST NOT be added as CLR properties because
            // Entity Framework would throw an exception due to the TKey generic parameter
            // being non-nullable when using value types like short, int, long or Guid.

            // If primary/foreign keys are strings, limit their length to ensure
            // they can be safely used in indexes, specially when the underlying
            // provider is known to not restrict the default length (e.g MySQL).
            if (typeof(TKey) == typeof(string))
            {
                builder.Property(application => application.Id)
                       .HasMaxLength(50);
            }

            builder.HasKey(application => application.Id);

            builder.HasIndex(application => application.ClientId)
                   .IsUnique();

            builder.Property(application => application.ClientId)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(application => application.ConcurrencyToken)
                   .HasMaxLength(50)
                   .IsConcurrencyToken();

            builder.Property(application => application.Type)
                   .HasMaxLength(25)
                   .IsRequired();

            builder.HasMany(application => application.Authorizations)
                   .WithOne(authorization => authorization.Application)
                   .HasForeignKey(nameof(OpenIddictAuthorization.Application) + nameof(OpenIddictApplication.Id))
                   .IsRequired(required: false);

            builder.HasMany(application => application.Tokens)
                   .WithOne(token => token.Application)
                   .HasForeignKey(nameof(OpenIddictToken.Application) + nameof(OpenIddictApplication.Id))
                   .IsRequired(required: false);

            builder.ToTable("OpenIddictApplications");
        }
    }
}
