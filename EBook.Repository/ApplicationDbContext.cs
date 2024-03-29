﻿using EBook.Domain;
using EBook.Domain.DomainModels;
using EBook.Domain.Identity;
using EBook.Domain.Relations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace EBook.Repository
{
    public class ApplicationDbContext : IdentityDbContext<EShopAppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public virtual DbSet<BookInShoppingCart> BookInShoppingCarts { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //za nov zapis
            builder.Entity<Book>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();


            builder.Entity<ShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            //primaren kluc na ovaa tabela praime kompoziten kluc
            builder.Entity<BookInShoppingCart>()
                .HasKey(z => new { z.BookId, z.ShoppingCartId });


            //deka se relacii
            builder.Entity<BookInShoppingCart>()
                .HasOne(z => z.CurrnetBook)
                .WithMany(z => z.BookInShoppingCarts)
                .HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<BookInShoppingCart>()
                .HasOne(z => z.UserCart)
                .WithMany(z => z.BookInShoppingCarts)
                .HasForeignKey(z => z.BookId);

            //za 1-1 
            builder.Entity<ShoppingCart>()
                .HasOne<EShopAppUser>(z => z.Owner)
                .WithOne(z => z.UserCart)
                .HasForeignKey<ShoppingCart>(z => z.OwnerId);

            builder.Entity<BookInOrder>()
               .Property(z => z.Id)
               .ValueGeneratedOnAdd();

            builder.Entity<BookInOrder>()
                .HasOne(z => z.Book)
                .WithMany(z => z.BookInOrders)
                .HasForeignKey(z => z.BookId);

            builder.Entity<BookInOrder>()
                .HasOne(z => z.UserOrder)
                .WithMany(z => z.BookInOrders)
                .HasForeignKey(z => z.OrderId);
            builder.Entity<Book>()
                .HasData(new Book { BookName = "Atomic Habits", BookImage = "https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcSlWR55VTSq5rQ5bjdm9bjNtT75d0KZcd8Lv1Jn72v-GGxQPJaa", BookDescription = "James Clear, an expert on habit formation, reveals practical strategies that will teach you how to form good habits, break bad ones, and master the tiny behaviors that lead to remarkable results. He draws on proven ideas from biology, psychology, and neuroscience to create an easy-to-understand guide for making good habits inevitable and bad habits impossible.", Genre = "Self-care", Price = 150 });
            // add roles
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN"
            });
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "2",
                Name = "Standard_User",
                NormalizedName = "STANDARD_USER"
            });

            // sett admin user
            var appUser = new EShopAppUser
            {
                Email = "admin@test.com",
                NormalizedEmail = "ADMIN@TEST.COM",
                EmailConfirmed = true,
                UserName = "admin@test.com",
                NormalizedUserName = "ADMIN@TEST.COM",
                PhoneNumberConfirmed = true
            };
            //set user password
            PasswordHasher<EShopAppUser> ph = new PasswordHasher<EShopAppUser>();
            appUser.PasswordHash = ph.HashPassword(appUser, "Pass123!");

            //seed user
            builder.Entity<EShopAppUser>().HasData(appUser);

            // shopping cart for admin user
            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                OwnerId = appUser.Id
            };
            builder.Entity<ShoppingCart>().HasData(shoppingCart);

            //set user role to admin
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "1",
                UserId = appUser.Id
            });
        }
    
    }


}

