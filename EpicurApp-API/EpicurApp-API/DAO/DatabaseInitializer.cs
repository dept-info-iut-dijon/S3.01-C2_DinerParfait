using EpicurApp_API.Configuration;
using Microsoft.Data.Sqlite;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// Classe statique pour initialiser la base de données et peupler les tables
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize(DatabaseConfiguration dbConfig)
        {
            using (var connection = dbConfig.CreateConnection())
            {
                connection.Open();

                // Table Allergenes
                var createAllergenesTable = @"
                    CREATE TABLE IF NOT EXISTS Allergenes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Description TEXT
                    );";

                // Table Clients
                var createClientsTable = @"
                    CREATE TABLE IF NOT EXISTS Clients (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Prenom TEXT NOT NULL,
                        Email TEXT,
                        Telephone TEXT,
                        Preferences TEXT
                    );";

                // Table de liaison ClientAllergene 
                var createClientAllergeneTable = @"
                    CREATE TABLE IF NOT EXISTS ClientAllergene (
                        ClientId INTEGER NOT NULL,
                        AllergeneId INTEGER NOT NULL,
                        PRIMARY KEY (ClientId, AllergeneId),
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE,
                        FOREIGN KEY (AllergeneId) REFERENCES Allergenes(Id) ON DELETE CASCADE
                    );";

                // Table Ingredients
                var createIngredientsTable = @"
                    CREATE TABLE IF NOT EXISTS Ingredients (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Description TEXT
                    );";

                // Table Plats
                var createPlatsTable = @"
                    CREATE TABLE IF NOT EXISTS Plats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Categorie INTEGER NOT NULL
                    );";

                // Table de liaison PlatIngredient
                var createPlatIngredientTable = @"
                    CREATE TABLE IF NOT EXISTS PlatIngredient (
                        PlatId INTEGER NOT NULL,
                        IngredientId INTEGER NOT NULL,
                        PRIMARY KEY (PlatId, IngredientId),
                        FOREIGN KEY (PlatId) REFERENCES Plats(Id) ON DELETE CASCADE,
                        FOREIGN KEY (IngredientId) REFERENCES Ingredients(Id) ON DELETE CASCADE
                    );";

                // Table de liaison ClientPlat (pour plats non appréciés)
                var createClientPlatTable = @"
                    CREATE TABLE IF NOT EXISTS ClientPlat (
                        ClientId INTEGER NOT NULL,
                        PlatId INTEGER NOT NULL,
                        PRIMARY KEY (ClientId, PlatId),
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE,
                        FOREIGN KEY (PlatId) REFERENCES Plats(Id) ON DELETE CASCADE
                    );";

                // Table Menus
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

                // Table MenuPlat
                var createMenuPlatTable = @"
                    CREATE TABLE IF NOT EXISTS MenuPlat (
                        MenuId INTEGER NOT NULL,
                        PlatId INTEGER NOT NULL,
                        PRIMARY KEY (MenuId, PlatId),
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id),
                        FOREIGN KEY (PlatId) REFERENCES Plats(Id)
                    );";

                // Table ClientMenu (pour l'historique)
                var createClientMenuTable = @"
                    CREATE TABLE IF NOT EXISTS ClientMenu (
                        ClientId INTEGER NOT NULL,
                        MenuId INTEGER NOT NULL,
                        PRIMARY KEY (ClientId, MenuId),
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id),
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id)
                    );";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = createAllergenesTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createIngredientsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createClientsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createClientAllergeneTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createPlatsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createPlatIngredientTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createClientPlatTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createMenusTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createMenuPlatTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createClientMenuTable;
                    command.ExecuteNonQuery();
                }

                SeedAllergenes(connection);
                SeedIngredients(connection);
                SeedPlats(connection);
                SeedPlatIngredients(connection);
                SeedClients(connection);
                SeedMenus(connection);
            }
        }

        /// <summary>
        /// Méthode privée pour peupler la table Allergenes avec les données initiales
        /// </summary>
        /// <param name="connection">La connexion SQLite active</param>
        private static void SeedAllergenes(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Allergenes;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var allergenes = new (string Nom, string Description)[]
                {
                    ("Gluten", "Céréales contenant du gluten (blé, seigle, orge, avoine)"),
                    ("Crustacés", "Crustacés et produits à base de crustacés"),
                    ("Œufs", "Œufs et produits à base d'œufs"),
                    ("Poissons", "Poissons et produits à base de poissons"),
                    ("Arachides", "Arachides et produits à base d'arachides"),
                    ("Soja", "Soja et produits à base de soja"),
                    ("Lait", "Lait et produits à base de lait (lactose inclus)"),
                    ("Fruits à coque", "Amandes, noisettes, noix, noix de cajou, etc."),
                    ("Céleri", "Céleri et produits à base de céleri"),
                    ("Moutarde", "Moutarde et produits à base de moutarde"),
                    ("Graines de sésame", "Graines de sésame et produits dérivés"),
                    ("Sulfites", "Anhydride sulfureux et sulfites (>10mg/kg)"),
                    ("Lupin", "Lupin et produits à base de lupin"),
                    ("Mollusques", "Mollusques et produits à base de mollusques")
                };

                using (var insertCommand = new SqliteCommand("INSERT INTO Allergenes (Nom, Description) VALUES (@Nom, @Description);", connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Description", SqliteType.Text));

                    foreach (var allergene in allergenes)
                    {
                        insertCommand.Parameters["@Nom"].Value = allergene.Nom;
                        insertCommand.Parameters["@Description"].Value = allergene.Description;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode privée pour peupler la table Ingredients avec les données initiales
        /// </summary>
        /// <param name="connection">La connexion SQLite active</param>
        private static void SeedIngredients(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Ingredients;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var ingredients = new (string Nom, string Description)[]
                {
                    ("Tomates", "Légume fruit rouge"),
                    ("Basilic", "Herbe aromatique"),
                    ("Huile d'olive", "Huile végétale"),
                    ("Saumon fumé", "Poisson fumé"),
                    ("Avocat", "Fruit tropical"),
                    ("Citron vert", "Agrume"),
                    ("Prosecco", "Vin pétillant italien"),
                    ("Aperol", "Apéritif italien"),
                    ("Eau pétillante", "Eau gazeuse"),
                    ("Framboise", "Fruit rouge"),
                    ("Myrtille", "Fruit bleu"),
                    ("Citron", "Agrume"),
                    ("Potiron", "Courge"),
                    ("Crème fraîche", "Produit laitier"),
                    ("Muscade", "Épice"),
                    ("Dorade", "Poisson blanc"),
                    ("Agrumes", "Famille de fruits"),
                    ("Ciboulette", "Herbe aromatique"),
                    ("Magret", "Viande de canard"),
                    ("Miel", "Produit sucré naturel"),
                    ("Romarin", "Herbe aromatique"),
                    ("Riz arborio", "Riz italien"),
                    ("Cèpes", "Champignons"),
                    ("Parmesan", "Fromage italien"),
                    ("Comté", "Fromage français"),
                    ("Brie", "Fromage français"),
                    ("Roquefort", "Fromage bleu français"),
                    ("Chèvre", "Fromage de chèvre"),
                    ("Miel d'acacia", "Miel floral"),
                    ("Noix", "Fruit à coque"),
                    ("Meringue italienne", "Préparation sucrée"),
                    ("Chocolat noir", "Chocolat à haute teneur en cacao"),
                    ("Crème", "Produit laitier"),
                    ("Œufs", "Produit aviaire")
                };

                using (var insertCommand = new SqliteCommand("INSERT INTO Ingredients (Nom, Description) VALUES (@Nom, @Description);", connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Description", SqliteType.Text));

                    foreach (var ingredient in ingredients)
                    {
                        insertCommand.Parameters["@Nom"].Value = ingredient.Nom;
                        insertCommand.Parameters["@Description"].Value = ingredient.Description;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode privée pour peupler la table Plats avec les données initiales
        /// </summary>
        /// <param name="connection">La connexion SQLite active</param>
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

                // 0=AmuseBouche, 1=BoissonAperitif, 2=Entree, 3=PlatPrincipal, 4=Vin, 5=Fromage, 6=Dessert
                var plats = new (string Nom, int Categorie)[]
                {
                    ("Bruschetta aux tomates", 0),
                    ("Verrine saumon-avocat", 0),
                    ("Spritz maison", 1),
                    ("Mocktail fruits rouges", 1),
                    ("Velouté de potiron", 2),
                    ("Tartare de dorade", 2),
                    ("Magret de canard sauce miel", 3),
                    ("Risotto aux champignons", 3),
                    ("Pinot noir de Bourgogne", 4),
                    ("Chardonnay réserve", 4),
                    ("Assortiment de fromages affinés", 5),
                    ("Chèvre frais miel-noix", 5),
                    ("Tartelette citron meringuée", 6),
                    ("Mousse au chocolat grand cru", 6),
                };

                using (var insertCommand = new SqliteCommand("INSERT INTO Plats (Nom, Categorie) VALUES (@Nom, @Categorie);", connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Categorie", SqliteType.Integer));

                    foreach (var plat in plats)
                    {
                        insertCommand.Parameters["@Nom"].Value = plat.Nom;
                        insertCommand.Parameters["@Categorie"].Value = plat.Categorie;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode privée pour peupler la table PlatIngredient avec les liaisons plat-ingrédient
        /// </summary>
        /// <param name="connection">La connexion SQLite active</param>
        private static void SeedPlatIngredients(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM PlatIngredient;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                // Relations PlatId -> IngredientId
                var platIngredients = new (int PlatId, int IngredientId)[]
                {
                    // Bruschetta aux tomates (1) -> Tomates, Basilic, Huile d'olive
                    (1, 1), (1, 2), (1, 3),
                    // Verrine saumon-avocat (2) -> Saumon fumé, Avocat, Citron vert
                    (2, 4), (2, 5), (2, 6),
                    // Spritz maison (3) -> Prosecco, Aperol, Eau pétillante
                    (3, 7), (3, 8), (3, 9),
                    // Mocktail fruits rouges (4) -> Framboise, Myrtille, Citron
                    (4, 10), (4, 11), (4, 12),
                    // Velouté de potiron (5) -> Potiron, Crème fraîche, Muscade
                    (5, 13), (5, 14), (5, 15),
                    // Tartare de dorade (6) -> Dorade, Agrumes, Ciboulette
                    (6, 16), (6, 17), (6, 18),
                    // Magret de canard sauce miel (7) -> Magret, Miel, Romarin
                    (7, 19), (7, 20), (7, 21),
                    // Risotto aux champignons (8) -> Riz arborio, Cèpes, Parmesan
                    (8, 22), (8, 23), (8, 24),
                    // Assortiment de fromages affinés (11) -> Comté, Brie, Roquefort
                    (11, 25), (11, 26), (11, 27),
                    // Chèvre frais miel-noix (12) -> Chèvre, Miel d'acacia, Noix
                    (12, 28), (12, 29), (12, 30),
                    // Tartelette citron meringuée (13) -> Citron, Meringue italienne, Œufs
                    (13, 12), (13, 31), (13, 34),
                    // Mousse au chocolat grand cru (14) -> Chocolat noir, Crème, Œufs
                    (14, 32), (14, 33), (14, 34)
                };

                using (var insertCommand = new SqliteCommand("INSERT INTO PlatIngredient (PlatId, IngredientId) VALUES (@PlatId, @IngredientId);", connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@PlatId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@IngredientId", SqliteType.Integer));

                    foreach (var platIngredient in platIngredients)
                    {
                        insertCommand.Parameters["@PlatId"].Value = platIngredient.PlatId;
                        insertCommand.Parameters["@IngredientId"].Value = platIngredient.IngredientId;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode privée pour peupler la table Clients avec des exemples
        /// </summary>
        /// <param name="connection">La connexion SQLite active</param>
        private static void SeedClients(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Clients;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var clients = new (string Nom, string Prenom, string Email, string Telephone, string Preferences)[]
                {
                    ("Dupont", "Marie", "marie.dupont@email.fr", "06 12 34 56 78", "Préfère les plats végétariens"),
                    ("Martin", "Jean", "jean.martin@email.fr", "06 23 45 67 89", "Aime les plats épicés"),
                    ("Bernard", "Sophie", "sophie.bernard@email.fr", "06 34 56 78 90", "Sans préférence particulière"),
                    ("Petit", "Lucas", "lucas.petit@email.fr", "06 45 67 89 01", "Apprécie les saveurs asiatiques"),
                    ("Dubois", "Emma", "emma.dubois@email.fr", "06 56 78 90 12", "Végétalienne stricte")
                };

                using (var insertCommand = new SqliteCommand("INSERT INTO Clients (Nom, Prenom, Email, Telephone, Preferences) VALUES (@Nom, @Prenom, @Email, @Telephone, @Preferences);", connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Prenom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Email", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Telephone", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Preferences", SqliteType.Text));

                    foreach (var client in clients)
                    {
                        insertCommand.Parameters["@Nom"].Value = client.Nom;
                        insertCommand.Parameters["@Prenom"].Value = client.Prenom;
                        insertCommand.Parameters["@Email"].Value = client.Email;
                        insertCommand.Parameters["@Telephone"].Value = client.Telephone;
                        insertCommand.Parameters["@Preferences"].Value = client.Preferences;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                // Ajouter des allergènes aux clients
                var clientAllergenes = new (int ClientId, int AllergeneId)[]
                {
                    (1, 1),  // Marie Dupont - Gluten
                    (1, 7),  // Marie Dupont - Lait
                    (2, 4),  // Jean Martin - Poissons
                    (3, 3),  // Sophie Bernard - Œufs
                    (5, 1),  // Emma Dubois - Gluten
                    (5, 3),  // Emma Dubois - Œufs
                    (5, 7)   // Emma Dubois - Lait
                };

                using (var insertAllergeneCommand = new SqliteCommand("INSERT INTO ClientAllergene (ClientId, AllergeneId) VALUES (@ClientId, @AllergeneId);", connection, transaction))
                {
                    insertAllergeneCommand.Parameters.Add(new SqliteParameter("@ClientId", SqliteType.Integer));
                    insertAllergeneCommand.Parameters.Add(new SqliteParameter("@AllergeneId", SqliteType.Integer));

                    foreach (var clientAllergene in clientAllergenes)
                    {
                        insertAllergeneCommand.Parameters["@ClientId"].Value = clientAllergene.ClientId;
                        insertAllergeneCommand.Parameters["@AllergeneId"].Value = clientAllergene.AllergeneId;
                        insertAllergeneCommand.ExecuteNonQuery();
                    }
                }

                // Ajouter des plats non appréciés
                var clientPlats = new (int ClientId, int PlatId)[]
                {
                    (2, 6),  // Jean Martin n'aime pas le Tartare de dorade
                    (4, 7),  // Lucas Petit n'aime pas le Magret de canard
                    (5, 14)  // Emma Dubois n'aime pas la Mousse au chocolat
                };

                using (var insertPlatCommand = new SqliteCommand("INSERT INTO ClientPlat (ClientId, PlatId) VALUES (@ClientId, @PlatId);", connection, transaction))
                {
                    insertPlatCommand.Parameters.Add(new SqliteParameter("@ClientId", SqliteType.Integer));
                    insertPlatCommand.Parameters.Add(new SqliteParameter("@PlatId", SqliteType.Integer));

                    foreach (var clientPlat in clientPlats)
                    {
                        insertPlatCommand.Parameters["@ClientId"].Value = clientPlat.ClientId;
                        insertPlatCommand.Parameters["@PlatId"].Value = clientPlat.PlatId;
                        insertPlatCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode privée pour peupler la table Menus avec des exemples
        /// </summary>
        /// <param name="connection">La connexion SQLite active</param>
        private static void SeedMenus(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Menus;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var menus = new (string Nom, string Date, string Statut, int? AmuseBouche, int? BoissonAperitif, int? Entree, int? PlatPrincipal, int? Vin, int? Fromage, int? Dessert)[]
                {
                    ("Menu Gastronomique du 20 Nov", "2025-11-20", "Publié", 1, 3, 5, 7, 9, 11, 13),
                    ("Menu Végétarien du 21 Nov", "2025-11-21", "Publié", 2, 4, 5, 8, 10, 12, 14),
                    ("Menu de la Mer du 22 Nov", "2025-11-22", "Brouillon", 2, 3, 6, null, 10, 11, 13)
                };

                using (var insertCommand = new SqliteCommand(@"
                    INSERT INTO Menus (Nom, Date, Statut, AmuseBoucheId, BoissonAperitifId, EntreeId, PlatPrincipalId, VinId, FromageId, DessertId)
                    VALUES (@Nom, @Date, @Statut, @AmuseBoucheId, @BoissonAperitifId, @EntreeId, @PlatPrincipalId, @VinId, @FromageId, @DessertId);",
                    connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Date", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Statut", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@AmuseBoucheId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@BoissonAperitifId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@EntreeId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@PlatPrincipalId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@VinId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@FromageId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@DessertId", SqliteType.Integer));

                    foreach (var menu in menus)
                    {
                        insertCommand.Parameters["@Nom"].Value = menu.Nom;
                        insertCommand.Parameters["@Date"].Value = menu.Date;
                        insertCommand.Parameters["@Statut"].Value = menu.Statut;
                        insertCommand.Parameters["@AmuseBoucheId"].Value = menu.AmuseBouche.HasValue ? (object)menu.AmuseBouche.Value : DBNull.Value;
                        insertCommand.Parameters["@BoissonAperitifId"].Value = menu.BoissonAperitif.HasValue ? (object)menu.BoissonAperitif.Value : DBNull.Value;
                        insertCommand.Parameters["@EntreeId"].Value = menu.Entree.HasValue ? (object)menu.Entree.Value : DBNull.Value;
                        insertCommand.Parameters["@PlatPrincipalId"].Value = menu.PlatPrincipal.HasValue ? (object)menu.PlatPrincipal.Value : DBNull.Value;
                        insertCommand.Parameters["@VinId"].Value = menu.Vin.HasValue ? (object)menu.Vin.Value : DBNull.Value;
                        insertCommand.Parameters["@FromageId"].Value = menu.Fromage.HasValue ? (object)menu.Fromage.Value : DBNull.Value;
                        insertCommand.Parameters["@DessertId"].Value = menu.Dessert.HasValue ? (object)menu.Dessert.Value : DBNull.Value;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                // Ajouter l'historique des menus pour certains clients
                var clientMenus = new (int ClientId, int MenuId)[]
                {
                    (1, 1),  // Marie Dupont a déjà eu le Menu Gastronomique
                    (2, 1),  // Jean Martin a déjà eu le Menu Gastronomique
                    (3, 2)   // Sophie Bernard a déjà eu le Menu Végétarien
                };

                using (var insertClientMenuCommand = new SqliteCommand("INSERT INTO ClientMenu (ClientId, MenuId) VALUES (@ClientId, @MenuId);", connection, transaction))
                {
                    insertClientMenuCommand.Parameters.Add(new SqliteParameter("@ClientId", SqliteType.Integer));
                    insertClientMenuCommand.Parameters.Add(new SqliteParameter("@MenuId", SqliteType.Integer));

                    foreach (var clientMenu in clientMenus)
                    {
                        insertClientMenuCommand.Parameters["@ClientId"].Value = clientMenu.ClientId;
                        insertClientMenuCommand.Parameters["@MenuId"].Value = clientMenu.MenuId;
                        insertClientMenuCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }
    }
}