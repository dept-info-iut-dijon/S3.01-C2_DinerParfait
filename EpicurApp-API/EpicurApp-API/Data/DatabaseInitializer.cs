using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace EpicurApp_API.Data
{
    /// <summary>
    /// Classe responsable de l'initialisation de la base de données.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Initialise la base de données en créant les tables nécessaires et en insérant des données initiales.
        /// </summary>
        /// <param name="configuration">Configuration de la db</param>
        public static void Initialize(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=epicurapp.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Table Allergenes
                string createAllergenesTable = @"
                    CREATE TABLE IF NOT EXISTS Allergenes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Description TEXT
                    );";

                // Table Clients 
                string createClientsTable = @"
                    CREATE TABLE IF NOT EXISTS Clients (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Prenom TEXT NOT NULL,
                        Email TEXT,
                        Telephone TEXT,
                        platsNonApprecies TEXT,
                        preferences TEXT
                    );";

                // Table de liaison ClientAllergene 
                string createClientAllergeneTable = @"
                    CREATE TABLE IF NOT EXISTS ClientAllergene (
                        ClientId INTEGER NOT NULL,
                        AllergeneId INTEGER NOT NULL,
                        PRIMARY KEY (ClientId, AllergeneId),
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE,
                        FOREIGN KEY (AllergeneId) REFERENCES Allergenes(Id) ON DELETE CASCADE
                    );";

                // Table Ingredients
                string createIngredientsTable = @"
                    CREATE TABLE IF NOT EXISTS Ingredients (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Description TEXT,
                        Categorie TEXT NOT NULL DEFAULT 'Autre'
                    );";

                // Table Plats
                string createPlatsTable = @"
                    CREATE TABLE IF NOT EXISTS Plats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nom TEXT NOT NULL,
                        Categorie TEXT NOT NULL,
                        IngredientsPrincipaux TEXT
                    );";

                // Table de liaison PlatIngredient
                string createPlatIngredientTable = @"
                    CREATE TABLE IF NOT EXISTS PlatIngredient (
                        PlatId INTEGER NOT NULL,
                        IngredientId INTEGER NOT NULL,
                        PRIMARY KEY (PlatId, IngredientId),
                        FOREIGN KEY (PlatId) REFERENCES Plats(Id) ON DELETE CASCADE,
                        FOREIGN KEY (IngredientId) REFERENCES Ingredients(Id) ON DELETE CASCADE
                    );";

                // Table Menus
                string createMenusTable = @"
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
                string createMenuPlatTable = @"
                    CREATE TABLE IF NOT EXISTS MenuPlat (
                        MenuId INTEGER NOT NULL,
                        PlatId INTEGER NOT NULL,
                        PRIMARY KEY (MenuId, PlatId),
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id),
                        FOREIGN KEY (PlatId) REFERENCES Plats(Id)
                    );";

                // Table ClientMenu (pour l'historique)
                string createClientMenuTable = @"
                    CREATE TABLE IF NOT EXISTS ClientMenu (
                        ClientId INTEGER NOT NULL,
                        MenuId INTEGER NOT NULL,
                        PRIMARY KEY (ClientId, MenuId),
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id),
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id)
                    );";

                // Table Repas (historique détaillé des repas avec retours)
                var createRepasTable = @"
                    CREATE TABLE IF NOT EXISTS Repas (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ClientId INTEGER NOT NULL,
                        MenuId INTEGER NOT NULL,
                        Date DATETIME NOT NULL,
                        Retours TEXT,
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE,
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id) ON DELETE CASCADE
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

                    command.CommandText = createMenusTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createMenuPlatTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createClientMenuTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createRepasTable;
                    command.ExecuteNonQuery();
                }

                SeedAllergenes(connection);
                SeedIngredients(connection);
                SeedPlats(connection);
                SeedClients(connection);
                SeedMenus(connection);
                SeedRepas(connection);
            }
        }

        /// <summary>
        /// Méthode pour insérer des allergènes prédéfinis dans la table Allergenes.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
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
        /// Méthode pour insérer des ingrédients prédéfinis dans la table Ingredients.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
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

                    // Données avec Catégorie
                    var ingredients = new (string Nom, string Description, string Categorie)[]
                    {
                    ("Tomates", "Tomates fraîches", "FruitLegume"),
                    ("Basilic", "Herbe aromatique", "FruitLegume"),
                    ("Huile d'olive", "Huile végétale", "Epicerie"),
                    ("Saumon fumé", "Poisson fumé", "Poisson"),
                    ("Avocat", "Fruit exotique", "FruitLegume"),
                    ("Citron vert", "Agrume", "FruitLegume"),
                    ("Prosecco", "Vin pétillant italien", "Boisson"),
                    ("Aperol", "Apéritif italien", "Boisson"),
                    ("Eau pétillante", "Boisson gazeuse", "Boisson"),
                    ("Framboise", "Fruit rouge", "FruitLegume"),
                    ("Myrtille", "Fruit rouge", "FruitLegume"),
                    ("Citron", "Agrume", "FruitLegume"),
                    ("Potiron", "Légume d'automne", "FruitLegume"),
                    ("Crème fraîche", "Produit laitier", "Cremerie"),
                    ("Muscade", "Épice", "Epicerie"),
                    ("Dorade", "Poisson blanc", "Poisson"),
                    ("Agrumes", "Fruits", "FruitLegume"),
                    ("Ciboulette", "Herbe aromatique", "FruitLegume"),
                    ("Magret de canard", "Viande", "Viande"),
                    ("Miel", "Produit sucré", "Epicerie"),
                    ("Romarin", "Herbe aromatique", "FruitLegume"),
                    ("Riz arborio", "Céréale", "Epicerie"),
                    ("Cèpes", "Champignons", "FruitLegume"),
                    ("Parmesan", "Fromage italien", "Cremerie"),
                    ("Comté", "Fromage français", "Cremerie"),
                    ("Brie", "Fromage français", "Cremerie"),
                    ("Roquefort", "Fromage bleu", "Cremerie"),
                    ("Chèvre", "Fromage de chèvre", "Cremerie"),
                    ("Miel d'acacia", "Miel doux", "Epicerie"),
                    ("Noix", "Fruit à coque", "Epicerie"),
                    ("Meringue italienne", "Préparation sucrée", "Boulangerie"),
                    ("Chocolat noir", "Cacao", "Epicerie"),
                    ("Crème", "Produit laitier", "Cremerie"),
                    ("Œufs", "Produit animal", "Cremerie")
                    };

                    using (var insertCommand = new SqliteCommand("INSERT INTO Ingredients (Nom, Description, Categorie) VALUES (@Nom, @Description, @Categorie);", connection, transaction))
                    {
                        insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                        insertCommand.Parameters.Add(new SqliteParameter("@Description", SqliteType.Text));
                        insertCommand.Parameters.Add(new SqliteParameter("@Categorie", SqliteType.Text));
                        foreach (var item in ingredients)
                        {
                            insertCommand.Parameters["@Nom"].Value = item.Nom;
                            insertCommand.Parameters["@Description"].Value = item.Description;
                            insertCommand.Parameters["@Categorie"].Value = item.Categorie;
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

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

                var plats = new (string Nom, string Categorie, int[] IngIds)[]
                {
                    ("Bruschetta aux tomates", "AmuseBouche", new[]{1, 2, 3}),
                    ("Verrine saumon-avocat", "AmuseBouche", new[]{4, 5, 6}),
                    ("Spritz maison", "BoissonAperitif", new[]{7, 8, 9}),
                    ("Mocktail fruits rouges", "BoissonAperitif", new[]{10, 11, 12}),
                    ("Velouté de potiron", "Entree", new[]{13, 14, 15}),
                    ("Tartare de dorade", "Entree", new[]{16, 17, 18}),
                    ("Magret de canard sauce miel", "PlatPrincipal", new[]{19, 20, 21}),
                    ("Risotto aux champignons", "PlatPrincipal", new[]{22, 23, 24}),
                    ("Pinot noir de Bourgogne", "Vin", new int[]{}),
                    ("Chardonnay réserve", "Vin", new int[]{}),
                    ("Assortiment de fromages affinés", "Fromage", new[]{25, 26, 27}),
                    ("Chèvre frais miel-noix", "Fromage", new[]{28, 29, 30}),
                    ("Tartelette citron meringuée", "Dessert", new[]{12, 31}),
                    ("Mousse au chocolat grand cru", "Dessert", new[]{32, 33, 34}),
                };

                using (var insertPlat = new SqliteCommand("INSERT INTO Plats (Nom, Categorie, IngredientsPrincipaux) VALUES (@Nom, @Categorie, '');", connection, transaction))
                using (var insertLink = new SqliteCommand("INSERT INTO PlatIngredient (PlatId, IngredientId) VALUES (@PlatId, @IngId);", connection, transaction))
                {
                    insertPlat.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertPlat.Parameters.Add(new SqliteParameter("@Categorie", SqliteType.Text));

                    insertLink.Parameters.Add(new SqliteParameter("@PlatId", SqliteType.Integer));
                    insertLink.Parameters.Add(new SqliteParameter("@IngId", SqliteType.Integer));

                    int platIdCounter = 1;
                    foreach (var plat in plats)
                    {
                        insertPlat.Parameters["@Nom"].Value = plat.Nom;
                        insertPlat.Parameters["@Categorie"].Value = plat.Categorie;
                        insertPlat.ExecuteNonQuery();

                        foreach (var ingId in plat.IngIds)
                        {
                            insertLink.Parameters["@PlatId"].Value = platIdCounter;
                            insertLink.Parameters["@IngId"].Value = ingId;
                            insertLink.ExecuteNonQuery();
                        }
                        platIdCounter++;
                    }
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode pour insérer des clients prédéfinis dans la table Clients.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
        private static void SeedClients(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Clients;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0) { transaction.Commit(); return; }
                }
                var clients = new (string Nom, string Prenom, string Email, string Telephone, string Preferences)[]
                {
                    ("Dupont", "Jean", "jean.dupont@email.fr", "0612345678", "Préfère les plats végétariens"),
                    ("Martin", "Sophie", "sophie.martin@email.fr", "0623456789", "Amateur de cuisine asiatique"),
                    ("Bernard", "Pierre", "pierre.bernard@email.fr", "0634567890", "Aime les plats traditionnels français"),
                    ("Dubois", "Marie", "marie.dubois@email.fr", "0645678901", "Fan de desserts"),
                    ("Petit", "Lucas", "lucas.petit@email.fr", "0656789012", "Cuisine méditerranéenne")
                };
                using (var insertCommand = new SqliteCommand("INSERT INTO Clients (Nom, Prenom, Email, Telephone, platsNonApprecies, preferences) VALUES (@Nom, @Prenom, @Email, @Telephone, '', @Preferences);", connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Prenom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Email", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Telephone", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Preferences", SqliteType.Text));
                    foreach (var item in clients)
                    {
                        insertCommand.Parameters["@Nom"].Value = item.Nom;
                        insertCommand.Parameters["@Prenom"].Value = item.Prenom;
                        insertCommand.Parameters["@Email"].Value = item.Email;
                        insertCommand.Parameters["@Telephone"].Value = item.Telephone;
                        insertCommand.Parameters["@Preferences"].Value = item.Preferences;
                        insertCommand.ExecuteNonQuery();
                    }
                }
                using (var insertAssoc = new SqliteCommand("INSERT INTO ClientAllergene (ClientId, AllergeneId) VALUES (1, 1), (1, 7), (2, 3), (3, 4), (4, 5);", connection, transaction))
                {
                    insertAssoc.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode pour insérer des menus prédéfinis dans la table Menus.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
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

                var menus = new[]
                {
                    ("Menu Découverte", "2024-11-15", "Validé", 1, 3, 5, 7, 9, 11, 13),
                    ("Menu Végétarien", "2024-11-16", "Validé", 2, 4, 6, 8, 10, 12, 14),
                    ("Menu du Jour", "2024-11-18", "Validé", 1, 3, 6, 7, 9, 11, 14),
                };

                using (var insertCommand = new SqliteCommand(
                    @"INSERT INTO Menus (Nom, Date, Statut, AmuseBoucheId, BoissonAperitifId, EntreeId, PlatPrincipalId, VinId, FromageId, DessertId)
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
                        insertCommand.Parameters["@Nom"].Value = menu.Item1;
                        insertCommand.Parameters["@Date"].Value = menu.Item2;
                        insertCommand.Parameters["@Statut"].Value = menu.Item3;
                        insertCommand.Parameters["@AmuseBoucheId"].Value = menu.Item4;
                        insertCommand.Parameters["@BoissonAperitifId"].Value = menu.Item5;
                        insertCommand.Parameters["@EntreeId"].Value = menu.Item6;
                        insertCommand.Parameters["@PlatPrincipalId"].Value = menu.Item7;
                        insertCommand.Parameters["@VinId"].Value = menu.Item8;
                        insertCommand.Parameters["@FromageId"].Value = menu.Item9;
                        insertCommand.Parameters["@DessertId"].Value = menu.Item10;
                        insertCommand.ExecuteNonQuery();
                    }
                }
                using (var insertCM = new SqliteCommand("INSERT INTO ClientMenu (ClientId, MenuId) VALUES (1, 1), (2, 2), (3, 1), (4, 3), (5, 2);", connection, transaction))
                {
                    insertCM.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode pour insérer des repas de test dans la table Repas.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
        private static void SeedRepas(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Repas;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var repas = new (int ClientId, int MenuId, string Date, string? Retours)[]
                {
                    (1, 1, "2024-11-15 12:00:00", "Excellent repas, très satisfait du menu découverte"),
                    (1, 2, "2024-11-10 19:00:00", "Bon menu végétarien, mais un peu épicé pour moi"),
                    (1, 3, "2024-11-05 12:30:00", null),
                    (2, 2, "2024-11-16 20:00:00", "Parfait ! J'adore la cuisine végétarienne"),
                    (3, 1, "2024-11-15 19:30:00", "Le magret de canard était excellent"),
                    (4, 3, "2024-11-18 12:00:00", "Dessert incroyable, je recommande"),
                    (5, 2, "2024-11-16 13:00:00", null)
                };

                using (var insertCommand = new SqliteCommand(
                    "INSERT INTO Repas (ClientId, MenuId, Date, Retours) VALUES (@ClientId, @MenuId, @Date, @Retours);",
                    connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@ClientId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@MenuId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@Date", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Retours", SqliteType.Text));

                    foreach (var r in repas)
                    {
                        insertCommand.Parameters["@ClientId"].Value = r.ClientId;
                        insertCommand.Parameters["@MenuId"].Value = r.MenuId;
                        insertCommand.Parameters["@Date"].Value = r.Date;
                        insertCommand.Parameters["@Retours"].Value = r.Retours ?? (object)DBNull.Value;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }
    }
}