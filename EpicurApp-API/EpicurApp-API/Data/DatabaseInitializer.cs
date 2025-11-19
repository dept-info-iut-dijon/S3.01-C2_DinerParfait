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
                        Description TEXT
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
                }

                SeedAllergenes(connection);
                SeedIngredients(connection);
                SeedPlats(connection);
                SeedClients(connection);
                SeedMenus(connection);
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
                }

                var ingredients = new (string Nom, string Description)[]
                {
                    ("Tomates", "Tomates fraîches"),
                    ("Basilic", "Herbe aromatique"),
                    ("Huile d'olive", "Huile végétale"),
                    ("Saumon fumé", "Poisson fumé"),
                    ("Avocat", "Fruit exotique"),
                    ("Citron vert", "Agrume"),
                    ("Prosecco", "Vin pétillant italien"),
                    ("Aperol", "Apéritif italien"),
                    ("Eau pétillante", "Boisson gazeuse"),
                    ("Framboise", "Fruit rouge"),
                    ("Myrtille", "Fruit rouge"),
                    ("Citron", "Agrume"),
                    ("Potiron", "Légume d'automne"),
                    ("Crème fraîche", "Produit laitier"),
                    ("Muscade", "Épice"),
                    ("Dorade", "Poisson blanc"),
                    ("Agrumes", "Fruits"),
                    ("Ciboulette", "Herbe aromatique"),
                    ("Magret de canard", "Viande"),
                    ("Miel", "Produit sucré"),
                    ("Romarin", "Herbe aromatique"),
                    ("Riz arborio", "Céréale"),
                    ("Cèpes", "Champignons"),
                    ("Parmesan", "Fromage italien"),
                    ("Comté", "Fromage français"),
                    ("Brie", "Fromage français"),
                    ("Roquefort", "Fromage bleu"),
                    ("Chèvre", "Fromage de chèvre"),
                    ("Miel d'acacia", "Miel doux"),
                    ("Noix", "Fruit à coque"),
                    ("Meringue italienne", "Préparation sucrée"),
                    ("Chocolat noir", "Cacao"),
                    ("Crème", "Produit laitier"),
                    ("Œufs", "Produit animal")
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
        /// Méthode pour insérer des plats prédéfinis dans la table Plats.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
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
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var clients = new (string Nom, string Prenom, string Email, string Telephone, string Preferences)[]
                {
                    ("Dupont", "Jean", "jean.dupont@email.fr", "0612345678", "Préfère les plats végétariens"),
                    ("Martin", "Sophie", "sophie.martin@email.fr", "0623456789", "Amateur de cuisine asiatique"),
                    ("Bernard", "Pierre", "pierre.bernard@email.fr", "0634567890", "Aime les plats traditionnels français"),
                    ("Dubois", "Marie", "marie.dubois@email.fr", "0645678901", "Fan de desserts"),
                    ("Petit", "Lucas", "lucas.petit@email.fr", "0656789012", "Cuisine méditerranéenne")
                };

                using (var insertCommand = new SqliteCommand(
                    "INSERT INTO Clients (Nom, Prenom, Email, Telephone, platsNonApprecies, preferences) VALUES (@Nom, @Prenom, @Email, @Telephone, @PlatsNonApprecies, @Preferences);",
                    connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Prenom", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Email", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Telephone", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@PlatsNonApprecies", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Preferences", SqliteType.Text));

                    foreach (var client in clients)
                    {
                        insertCommand.Parameters["@Nom"].Value = client.Nom;
                        insertCommand.Parameters["@Prenom"].Value = client.Prenom;
                        insertCommand.Parameters["@Email"].Value = client.Email;
                        insertCommand.Parameters["@Telephone"].Value = client.Telephone;
                        insertCommand.Parameters["@PlatsNonApprecies"].Value = "";
                        insertCommand.Parameters["@Preferences"].Value = client.Preferences;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                // Assigner quelques allergènes aux clients
                var allergeneAssignments = new (int ClientId, int AllergeneId)[]
                {
                    (1, 1),  // Jean Dupont - Gluten
                    (1, 7),  // Jean Dupont - Lait
                    (2, 3),  // Sophie Martin - Œufs
                    (3, 4),  // Pierre Bernard - Poissons
                    (4, 5),  // Marie Dubois - Arachides
                };

                using (var insertAllergeneCommand = new SqliteCommand(
                    "INSERT INTO ClientAllergene (ClientId, AllergeneId) VALUES (@ClientId, @AllergeneId);",
                    connection, transaction))
                {
                    insertAllergeneCommand.Parameters.Add(new SqliteParameter("@ClientId", SqliteType.Integer));
                    insertAllergeneCommand.Parameters.Add(new SqliteParameter("@AllergeneId", SqliteType.Integer));

                    foreach (var assignment in allergeneAssignments)
                    {
                        insertAllergeneCommand.Parameters["@ClientId"].Value = assignment.ClientId;
                        insertAllergeneCommand.Parameters["@AllergeneId"].Value = assignment.AllergeneId;
                        insertAllergeneCommand.ExecuteNonQuery();
                    }
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

                var menus = new (string Nom, string Date, string Statut, int? AmuseBoucheId, int? BoissonId, int? EntreeId, int? PlatId, int? VinId, int? FromageId, int? DessertId)[]
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
                        insertCommand.Parameters["@Nom"].Value = menu.Nom;
                        insertCommand.Parameters["@Date"].Value = menu.Date;
                        insertCommand.Parameters["@Statut"].Value = menu.Statut;
                        insertCommand.Parameters["@AmuseBoucheId"].Value = menu.AmuseBoucheId.HasValue ? menu.AmuseBoucheId.Value : DBNull.Value;
                        insertCommand.Parameters["@BoissonAperitifId"].Value = menu.BoissonId.HasValue ? menu.BoissonId.Value : DBNull.Value;
                        insertCommand.Parameters["@EntreeId"].Value = menu.EntreeId.HasValue ? menu.EntreeId.Value : DBNull.Value;
                        insertCommand.Parameters["@PlatPrincipalId"].Value = menu.PlatId.HasValue ? menu.PlatId.Value : DBNull.Value;
                        insertCommand.Parameters["@VinId"].Value = menu.VinId.HasValue ? menu.VinId.Value : DBNull.Value;
                        insertCommand.Parameters["@FromageId"].Value = menu.FromageId.HasValue ? menu.FromageId.Value : DBNull.Value;
                        insertCommand.Parameters["@DessertId"].Value = menu.DessertId.HasValue ? menu.DessertId.Value : DBNull.Value;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                // Assigner les menus à certains clients
                var menuAssignments = new (int ClientId, int MenuId)[]
                {
                    (1, 1),  // Jean Dupont - Menu Découverte
                    (2, 2),  // Sophie Martin - Menu Végétarien
                    (3, 1),  // Pierre Bernard - Menu Découverte
                    (4, 3),  // Marie Dubois - Menu du Jour
                    (5, 2),  // Lucas Petit - Menu Végétarien
                };

                using (var insertMenuCommand = new SqliteCommand(
                    "INSERT INTO ClientMenu (ClientId, MenuId) VALUES (@ClientId, @MenuId);",
                    connection, transaction))
                {
                    insertMenuCommand.Parameters.Add(new SqliteParameter("@ClientId", SqliteType.Integer));
                    insertMenuCommand.Parameters.Add(new SqliteParameter("@MenuId", SqliteType.Integer));

                    foreach (var assignment in menuAssignments)
                    {
                        insertMenuCommand.Parameters["@ClientId"].Value = assignment.ClientId;
                        insertMenuCommand.Parameters["@MenuId"].Value = assignment.MenuId;
                        insertMenuCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }
    }
}