﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.Repositories
{
    public class AuthorRepository : DatabaseConnector, IRepository<Author>
    {
        public AuthorRepository(string connectionString) : base(connectionString) { }

        public List<Author> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id,
                                               FirstName,
                                               LastName,
                                               Bio,
                                               IsDeleted
                                          FROM Author";

                    List<Author> authors = new List<Author>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Author author = new Author()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Bio = reader.GetString(reader.GetOrdinal("Bio")),
                            IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                        };
                        authors.Add(author);
                    }

                    reader.Close();

                    return authors;
                }
            }
        }

        public Author Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT a.Id AS AuthorId,
                                               a.FirstName,
                                               a.LastName,
                                               a.Bio,
                                               t.Id AS TagId,
                                               t.Name
                                          FROM Author a 
                                               LEFT JOIN AuthorTag at on a.Id = at.AuthorId
                                               LEFT JOIN Tag t on t.Id = at.TagId
                                         WHERE a.id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    Author author = null;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (author == null)
                        {
                            author = new Author()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Bio = reader.GetString(reader.GetOrdinal("Bio")),
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("TagId")))
                        {
                            author.Tags.Add(new Tag()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            });
                        }
                    }

                    reader.Close();

                    return author;
                }
            }
        }

//this is to add
        public void Insert(Author author)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Author (FirstName, LastName, Bio, IsDeleted )
                                                     VALUES (@firstName, @lastName, @bio, @isDeleted)";
                    cmd.Parameters.AddWithValue("@firstName", author.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", author.LastName);
                    cmd.Parameters.AddWithValue("@bio", author.Bio);
                    cmd.Parameters.AddWithValue("@isDeleted", false);
                       

                    cmd.ExecuteNonQuery();
                }
            }
        }

//this is to edit
        public void Update(Author author)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Author 
                                           SET FirstName = @firstName,
                                               LastName = @lastName,
                                               bio = @bio
                                               
                                         WHERE id = @id";

                    cmd.Parameters.AddWithValue("@firstName", author.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", author.LastName);
                    cmd.Parameters.AddWithValue("@bio", author.Bio);
                    cmd.Parameters.AddWithValue("@id", author.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Author WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //this is actually updating the prop IsDeleted to true
        public void SoftDelete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Author 
                                        SET IsDeleted = @isDeleted 
                                        WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@isDeleted", true);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertTag(Author author, Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO AuthorTag (AuthorId, TagId)
                                                       VALUES (@authorId, @tagId)";
                    cmd.Parameters.AddWithValue("@authorId", author.Id);
                    cmd.Parameters.AddWithValue("@tagId", tag.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTag(int authorId, int tagId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM AuthorTAg 
                                         WHERE AuthorId = @authorid AND 
                                               TagId = @tagId";
                    cmd.Parameters.AddWithValue("@authorId", authorId);
                    cmd.Parameters.AddWithValue("@tagId", tagId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
     }
}
