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

                SeedPlats(connection);
            }
        }

        /// <summary>
        /// Met les plats par défaut dans la base de données
        /// </summary>
        /// <param name="connection"></param>
        private static void SeedPlats(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Plats;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var plats = new (string Nom, string Categorie, string Ingredients)[]
                {
                    ("Bruschetta aux tomates", "AmuseBouche", "Tomates, basilic, huile d'olive"),
                    ("Verrine saumon-avocat", "AmuseBouche", "Saumon fumé, avocat, citron vert"),
                    ("Spritz maison", "BoissonAperitif", "Prosecco, Aperol, eau pétillante"),
                    ("Mocktail fruits rouges", "BoissonAperitif", "Framboise, myrtille, citron"),
                    ("Velouté de potiron", "Entree", "Potiron, crème fraîche, muscade"),
                    ("Tartare de dorade", "Entree", "Dorade, agrumes, ciboulette"),
                    ("Magret de canard sauce miel", "PlatPrincipal", "Magret, miel, romarin"),
                    ("Risotto aux champignons", "PlatPrincipal", "Riz arborio, cèpes, parmesan"),
                    ("Pinot noir de Bourgogne", "Vin", "Rouge, notes de fruits rouges"),
                    ("Chardonnay réserve", "Vin", "Blanc, arômes de fleurs blanches"),
                    ("Assortiment de fromages affinés", "Fromage", "Comté, Brie, Roquefort"),
                    ("Chèvre frais miel-noix", "Fromage", "Chèvre, miel d'acacia, noix"),
                    ("Tartelette citron meringuée", "Dessert", "Citron, meringue italienne"),
                    ("Mousse au chocolat grand cru", "Dessert", "Chocolat noir, crème, œufs"),
                };

                using (var insertCommand = new SqliteCommand("INSERT INTO Plats (Nom, Categorie, IngredientsPrincipaux) VALUES (@Nom, @Categorie, @Ingredients);", connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Categorie", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Ingredients", SqliteType.Text));

                    foreach (var plat in plats)
                    {
                        insertCommand.Parameters["@Nom"].Value = plat.Nom;
                        insertCommand.Parameters["@Categorie"].Value = plat.Categorie;
                        insertCommand.Parameters["@Ingredients"].Value = plat.Ingredients;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }
    }
}

