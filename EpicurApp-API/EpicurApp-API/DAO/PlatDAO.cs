using EpicurApp_API.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using EpicurAPP_Partage.Interfaces;

namespace EpicurApp_API.DAO
{
    public class PlatDAO : IPlatDAO
    {
        private readonly string connexionString = "Data Source=epicurapp.db";

        public async Task<IEnumerable<Plat>> GetAllAsync()
        {
            var plats = new List <Plat>();

            const string query = "SELECT Id, Nom, Categorie, IngredientsPrincipaux, Cout FROM Plats ORDER BY Categorie, Nom;";

            using (var connexion = new SqliteConnection(connexionString))
            {
                await connexion.OpenAsync();
                using (var cmd = new SqliteCommand(query, connexion))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            plats.Add(new Plat
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Categorie = reader.GetString(2),
                                IngredientsPrincipaux = reader.GetString(3),
                                Cout = reader.GetDecimal(4)
                            });
                        }
                    }
                }
            }
            return plats;
        }

        public async Task<Plat> GetByIdAsync(int id)
        {
            Plat plat = null;
            const string query = "SELECT Id, Nom, Categorie, IngredientsPrincipaux, Cout FROM Plats WHERE Id = @Id;";

            using (var connection = new SqliteConnection(connexionString))
            {
                await connection.OpenAsync();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            plat = new Plat
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Categorie = reader.GetString(2),
                                IngredientsPrincipaux = reader.GetString(3),
                                Cout = reader.GetDecimal(4)
                            };
                        }
                    }
                }
            }
            return plat;
        }

        public async Task AddAsync(Plat plat)
        {
            const string query= "INSERT INTO Plats (Nom, Categorie, IngredientsPrincipaux, Cout) VALUES (@Nom, @Categorie, @IngredientsPrincipaux, @Cout);";

            using (var connection = new SqliteConnection(connexionString))
            {
                await connection.OpenAsync();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom",plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", plat.Categorie);
                    command.Parameters.AddWithValue("@IngredientsPrincipaux", plat.IngredientsPrincipaux);
                    command.Parameters.AddWithValue("@Cout", plat.Cout);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
