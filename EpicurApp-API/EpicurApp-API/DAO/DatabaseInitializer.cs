using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace EpicurApp_API.DAO
{
    public static class DatabaseInitializer
    {
        public static void Initialize(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=epicurapp.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Créer la table Allergenes
                var createAllergenesTable = @"
                    CREATE TABLE IF NOT EXISTS Allergenes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Description TEXT
                    );";

                // Créer la table Clients
                var createClientsTable = @"
                    CREATE TABLE IF NOT EXISTS Clients (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Prenom TEXT NOT NULL,
                        Email TEXT,
                        Telephone TEXT,
                        Allergies TEXT,
                        Notes TEXT
                    );";

                //table ClientAllergene
                var createClientAllergeneTable = @"
                    CREATE TABLE IF NOT EXISTS ClientAllergene (
                        ClientId INTEGER NOT NULL,
                        AllergeneId INTEGER NOT NULL,
                        PRIMARY KEY (ClientId, AllergeneId),
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id),
                        FOREIGN KEY (AllergeneId) REFERENCES Allergenes(Id)
                    );";

                // table Plats
                var createPlatsTable = @"
                    CREATE TABLE IF NOT EXISTS Plats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Categorie TEXT NOT NULL,
                        IngredientsPrincipaux TEXT
                    );";

                //la table Menus
                var createMenusTable = @"
                    CREATE TABLE IF NOT EXISTS Menus (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Date DATETIME NOT NULL,
                        Statut TEXT NOT NULL,
                        AmuseBoucheId INTEGER,
                        BoissonAperitifId INTEGER,
                        EntreeId INTEGER,
                        PlatPrincipalId INTEGER,
                        VinId INTEGER,
                        FromageId INTEGER,
                        DessertId INTEGER,
                        FOREIGN KEY (AmuseBoucheId) REFERENCES Plats(Id),
                        FOREIGN KEY (BoissonAperitifId) REFERENCES Plats(Id),
                        FOREIGN KEY (EntreeId) REFERENCES Plats(Id),
                        FOREIGN KEY (PlatPrincipalId) REFERENCES Plats(Id),
                        FOREIGN KEY (VinId) REFERENCES Plats(Id),
                        FOREIGN KEY (FromageId) REFERENCES Plats(Id),
                        FOREIGN KEY (DessertId) REFERENCES Plats(Id)
                    );";

                // table MenuPlat
                var createMenuPlatTable = @"
                    CREATE TABLE IF NOT EXISTS MenuPlat (
                        MenuId INTEGER NOT NULL,
                        PlatId INTEGER NOT NULL,
                        PRIMARY KEY (MenuId, PlatId),
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id),
                        FOREIGN KEY (PlatId) REFERENCES Plats(Id)
                    );";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = createAllergenesTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createClientsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createClientAllergeneTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createPlatsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createMenusTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createMenuPlatTable;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

