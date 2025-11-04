using EpicurApp_API.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

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

        public Task<Plat> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Plat plat)
        {
            throw new NotImplementedException();
        }
    }
}
