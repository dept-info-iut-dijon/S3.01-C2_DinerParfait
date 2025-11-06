using EpicurApp_API.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using EpicurAPP_Partage.Interfaces;

namespace EpicurApp_API.DAO
{
    public class PlatDAO : IPlatDAO
    {
        private string _connexionString = "Data Source=epicurapp.db";

        public List<Plat> GetAll()
        {
            List<Plat> plats = new List<Plat>();
            const string query = "SELECT Id, Nom, Categorie, IngredientsPrincipaux FROM Plats ORDER BY Categorie, Nom;";

            using (SqliteConnection connexion = new SqliteConnection(_connexionString))
            {
                connexion.Open();
                using (SqliteCommand cmd = new SqliteCommand(query, connexion))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        plats.Add(new Plat
                        {
                            Id = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Categorie = reader.GetString(2),
                            IngredientsPrincipaux = reader.IsDBNull(3) ? string.Empty : reader.GetString(3) 
                        });
                    }
                }
            }

            return plats;
        }

        public Plat? GetById(int id)
        {
            Plat? plat = null;
            const string query = "SELECT Id, Nom, Categorie, IngredientsPrincipaux FROM Plats WHERE Id = @Id;";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            plat = new Plat
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Categorie = reader.GetString(2),
                                IngredientsPrincipaux = reader.IsDBNull(3) ? string.Empty : reader.GetString(3) 
                            };
                        }
                    }
                }
            }

            return plat;
        }

        public void Add(Plat plat)
        {
           
            const string query = "INSERT INTO Plats (Nom, Categorie, IngredientsPrincipaux) VALUES (@Nom, @Categorie, @IngredientsPrincipaux);";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", plat.Categorie);
                    command.Parameters.AddWithValue("@IngredientsPrincipaux", plat.IngredientsPrincipaux ?? string.Empty); 

                    command.ExecuteNonQuery();


                    command.CommandText = "SELECT last_insert_rowid();";
                    plat.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

       
        public void Update(Plat plat)
        {
            const string query = "UPDATE Plats SET Nom = @Nom, Categorie = @Categorie, IngredientsPrincipaux = @IngredientsPrincipaux WHERE Id = @Id;";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", plat.Id);
                    command.Parameters.AddWithValue("@Nom", plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", plat.Categorie);
                    command.Parameters.AddWithValue("@IngredientsPrincipaux", plat.IngredientsPrincipaux ?? string.Empty);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            const string query = "DELETE FROM Plats WHERE Id = @Id;";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}