﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using TabloidCLI.Models;
using TabloidCLI.Repositories;
using TabloidCLI.UserInterfaceManagers;

namespace TabloidCLI
{
    public class TagRepository : DatabaseConnector, IRepository<Tag>
    {
        public TagRepository(string connectionString) : base(connectionString) { }

        public List<Tag> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select id, name FROM Tag ";
                    List<Tag> tags = new List<Tag>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Tag tag = new Tag()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                        tags.Add(tag);
                    }

                    reader.Close();

                    return tags;
                }
            }
        }

        public Tag Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id AS TagId,
                                                        Name         
                                          FROM Tag 
                                         WHERE id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    Tag tag = null;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (tag == null)
                        {
                            tag = new Tag()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };

                        }
                    }

                    reader.Close();

                    return tag;
                }
            }
        }

        public void Insert(Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Tag (Name)
                                                     VALUES (@Name)";
                    cmd.Parameters.AddWithValue("@Name", tag.Name);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Tag 
                                           SET Name = @Name
                                         WHERE id = @id";

                    cmd.Parameters.AddWithValue("@Name", tag.Name);
                    cmd.Parameters.AddWithValue("@id", tag.Id);



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
                    cmd.CommandText = @"DELETE FROM Tag WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public SearchResults<Author> SearchAuthors(string tagName)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT a.id,
                                               a.FirstName,
                                               a.LastName,
                                               a.Bio
                                          FROM Author a
                                               LEFT JOIN AuthorTag at on a.Id = at.AuthorId
                                               LEFT JOIN Tag t on t.Id = at.TagId
                                         WHERE t.Name LIKE @name";
                    cmd.Parameters.AddWithValue("@name", $"%{tagName}%");
                    SqlDataReader reader = cmd.ExecuteReader();

                    SearchResults<Author> results = new SearchResults<Author>();
                    while (reader.Read())
                    {
                        Author author = new Author()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Bio = reader.GetString(reader.GetOrdinal("Bio")),
                        };
                        results.Add(author);
                    }

                    reader.Close();

                    return results;
                }
            }
        }

     


        public SearchResults<Post> SearchPosts(string tagName)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT p.id,
                                               p.Title,
                                               p.Url,
                                               p.PublishDateTime,
                                               p.AuthorId,
                                               p.BlogId 
                                          FROM Post p
                                               LEFT JOIN PostTag pt on p.Id = pt.PostId
                                               LEFT JOIN Tag t on t.Id = pt.TagId
                                         WHERE t.Name LIKE @name";
                    cmd.Parameters.AddWithValue("@name", $"%{tagName}%");
                    SqlDataReader reader = cmd.ExecuteReader();

                    SearchResults<Post> results = new SearchResults<Post>();
                    while (reader.Read())
                    {
                        Post post = new Post()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Url = reader.GetString(reader.GetOrdinal("Url")),
                            PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),


                        };
                        results.Add(post);
                    }

                    reader.Close();

                    return results;
                }
            }
        }


        public SearchResults<Blog> SearchBlogs(string tagName)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT b.id,
                                               b.Title,
                                               b.Url
                                          FROM Blog b
                                               LEFT JOIN BlogTag bt on b.Id = bt.BlogId
                                               LEFT JOIN Tag t on t.Id = bt.TagId
                                         WHERE t.Name LIKE @name";
                    cmd.Parameters.AddWithValue("@name", $"%{tagName}%");
                    SqlDataReader reader = cmd.ExecuteReader();

                    SearchResults<Blog> results = new SearchResults<Blog>();
                    while (reader.Read())
                    {
                        Blog blog = new Blog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Url = reader.GetString(reader.GetOrdinal("Url")),

                        };
                        results.Add(blog);
                    }

                    reader.Close();

                    return results;
                }
            }
        }

        public SearchResults<Tag> SearchAll(string tagName)
        {
            SearchBlogs(tagName);
            SearchAuthors(tagName);
            SearchPosts(tagName);

            return SearchAll(tagName);
                }
            }
        }



        
        


       

        


    

        //this is for post search by tag

        //public SearchResults<Post> SearchPosts(string tagName)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT a.id,
        //                                       a.FirstName,
        //                                       a.LastName,
        //                                       a.Bio
        //                                  FROM Author a
        //                                       LEFT JOIN AuthorTag at on a.Id = at.AuthorId
        //                                       LEFT JOIN Tag t on t.Id = at.TagId
        //                                 WHERE t.Name LIKE @name";
        //            cmd.Parameters.AddWithValue("@name", $"%{tagName}%");
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            SearchResults<Author> results = new SearchResults<Author>();
        //            while (reader.Read())
        //            {
        //                Author author = new Author()
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                    Bio = reader.GetString(reader.GetOrdinal("Bio")),
        //                };
        //                results.Add(author);
        //            }

        //            reader.Close();

        //            return results;
        //        }
        //    }



        //////

    
