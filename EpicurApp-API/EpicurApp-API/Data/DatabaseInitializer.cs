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
                        Statut TEXT NOT NULL
                    );";

                // Table ElementMenus (nouvelle structure extensible)
                string createElementMenusTable = @"
                    CREATE TABLE IF NOT EXISTS ElementMenus (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        MenuId INTEGER NOT NULL,
                        PlatId INTEGER NOT NULL,
                        Categorie TEXT NOT NULL,
                        Ordre INTEGER NOT NULL,
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id) ON DELETE CASCADE,
                        FOREIGN KEY (PlatId) REFERENCES Plats(Id)
                    );";

                // Table MenuPlat
                string createMenuPlatTable = @"
                    CREATE TABLE IF NOT EXISTS MenuPlat (
                        MenuId INTEGER NOT NULL,
                        PlatId INTEGER NOT NULL,
                        PRIMARY KEY (MenuId, PlatId),
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id) ON DELETE CASCADE,
                        FOREIGN KEY (PlatId) REFERENCES Plats(Id) ON DELETE CASCADE
                    );";

                // Table ClientMenu (pour l'historique)
                string createClientMenuTable = @"
                    CREATE TABLE IF NOT EXISTS ClientMenu (
                        ClientId INTEGER NOT NULL,
                        MenuId INTEGER NOT NULL,
                        Note INTEGER CHECk (Note<=5 AND Note>=0),   
                        Avis TEXT, 
                        PRIMARY KEY (ClientId, MenuId),
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE,
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id) ON DELETE CASCADE
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

                // Table IdeesPlats (boîte à idées pour futurs plats)
                var createIdeesPlatTable = @"
                    CREATE TABLE IF NOT EXISTS IdeesPlats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Titre TEXT NOT NULL,
                        Description TEXT,
                        Categorie TEXT,
                        Notes TEXT,
                        DateCreation DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";

                // Table Services (gestion des services midi/soir)
                var createServicesTable = @"
                    CREATE TABLE IF NOT EXISTS Services (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date DATETIME NOT NULL,
                        MidiSoir TEXT NOT NULL,
                        MenuId INTEGER NOT NULL,
                        Statut TEXT NOT NULL DEFAULT 'Ouvert',
                        FOREIGN KEY (MenuId) REFERENCES Menus(Id) ON DELETE CASCADE
                    );";

                // Table Reservations (réservations clients pour les services)
                var createReservationsTable = @"
                    CREATE TABLE IF NOT EXISTS Reservations (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ServiceId INTEGER NOT NULL,
                        ClientId INTEGER NOT NULL,
                        NbCouverts INTEGER NOT NULL,
                        FOREIGN KEY (ServiceId) REFERENCES Services(Id) ON DELETE CASCADE,
                        FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE
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

                    command.CommandText = createElementMenusTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createMenuPlatTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createClientMenuTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createRepasTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createIdeesPlatTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createServicesTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createReservationsTable;
                    command.ExecuteNonQuery();
                }

                SeedAllergenes(connection);
                SeedIngredients(connection);
                SeedPlats(connection);
                SeedClients(connection);
                SeedMenus(connection);
                SeedRepas(connection);
                SeedIdeesPlats(connection);
                SeedServices(connection);
                SeedReservations(connection);
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

                // Insertion des menus
                var menus = new[]
                {
                    ("Menu Découverte", "2024-11-15", "Validé","Parfait, je reviendrai !"),
                    ("Menu Végétarien", "2024-11-16", "Validé", "Très bon mais l'entrée manquais un peu de sel"),
                    ("Menu du Jour", "2024-11-18", "Validé", null),
                };

                using (var insertMenuCommand = new SqliteCommand(
                    "INSERT INTO Menus (Nom, Date, Statut,Note,Remarques) VALUES (@Nom, @Date, @Statut,@Note,@Remarques);",
                    connection, transaction))
                {
                    insertMenuCommand.Parameters.Add(new SqliteParameter("@Nom", SqliteType.Text));
                    insertMenuCommand.Parameters.Add(new SqliteParameter("@Date", SqliteType.Text));
                    insertMenuCommand.Parameters.Add(new SqliteParameter("@Statut", SqliteType.Text));
                    insertMenuCommand.Parameters.Add(new SqliteParameter("@Note", SqliteType.Integer));
                    insertMenuCommand.Parameters.Add(new SqliteParameter("@Retours", SqliteType.Text));

                    foreach (var menu in menus)
                    {
                        insertMenuCommand.Parameters["@Nom"].Value = menu.Item1;
                        insertMenuCommand.Parameters["@Date"].Value = menu.Item2;
                        insertMenuCommand.Parameters["@Statut"].Value = menu.Item3;
                        insertMenuCommand.Parameters["@Note"].Value = menu.Item4;
                        insertMenuCommand.Parameters["@Remarques"].Value = menu.Item5;
                        insertMenuCommand.ExecuteNonQuery();
                    }
                }

                // Insertion des éléments des menus (ElementMenus)
                // Format: (MenuId, PlatId, Categorie, Ordre)
                var elements = new (int MenuId, int PlatId, string Categorie, int Ordre)[]
                {
                    // Menu Découverte (Menu 1)
                    (1, 1, "AmuseBouche", 1),
                    (1, 3, "BoissonAperitif", 1),
                    (1, 5, "Entree", 1),
                    (1, 7, "PlatPrincipal", 1),
                    (1, 9, "Vin", 1),
                    (1, 11, "Fromage", 1),
                    (1, 13, "Dessert", 1),

                    // Menu Végétarien (Menu 2)
                    (2, 2, "AmuseBouche", 1),
                    (2, 4, "BoissonAperitif", 1),
                    (2, 6, "Entree", 1),
                    (2, 8, "PlatPrincipal", 1),
                    (2, 10, "Vin", 1),
                    (2, 12, "Fromage", 1),
                    (2, 14, "Dessert", 1),

                    // Menu du Jour (Menu 3)
                    (3, 1, "AmuseBouche", 1),
                    (3, 3, "BoissonAperitif", 1),
                    (3, 6, "Entree", 1),
                    (3, 7, "PlatPrincipal", 1),
                    (3, 9, "Vin", 1),
                    (3, 11, "Fromage", 1),
                    (3, 14, "Dessert", 1),
                };

                using (var insertElementCommand = new SqliteCommand(
                    "INSERT INTO ElementMenus (MenuId, PlatId, Categorie, Ordre) VALUES (@MenuId, @PlatId, @Categorie, @Ordre);",
                    connection, transaction))
                {
                    insertElementCommand.Parameters.Add(new SqliteParameter("@MenuId", SqliteType.Integer));
                    insertElementCommand.Parameters.Add(new SqliteParameter("@PlatId", SqliteType.Integer));
                    insertElementCommand.Parameters.Add(new SqliteParameter("@Categorie", SqliteType.Text));
                    insertElementCommand.Parameters.Add(new SqliteParameter("@Ordre", SqliteType.Integer));

                    foreach (var element in elements)
                    {
                        insertElementCommand.Parameters["@MenuId"].Value = element.MenuId;
                        insertElementCommand.Parameters["@PlatId"].Value = element.PlatId;
                        insertElementCommand.Parameters["@Categorie"].Value = element.Categorie;
                        insertElementCommand.Parameters["@Ordre"].Value = element.Ordre;
                        insertElementCommand.ExecuteNonQuery();
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

        /// <summary>
        /// Méthode pour insérer des idées de plats de test dans la table IdeesPlats.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
        private static void SeedIdeesPlats(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM IdeesPlats;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var idees = new (string Titre, string Description, string Categorie, string Notes)[]
                {
                    ("Soupe glacée à la courgette",
                     "Une soupe froide rafraîchissante à base de courgettes et menthe fraîche",
                     "Entree",
                     "Parfait pour l'été, peut être servi avec des croûtons au parmesan"),

                    ("Tataki de thon mi-cuit",
                     "Thon rouge saisi rapidement avec une croûte de sésame",
                     "Entree",
                     "Servir avec sauce soja-yuzu et gingembre mariné"),

                    ("Poulet rôti au citron confit",
                     "Poulet mariné aux herbes et citrons confits",
                     "PlatPrincipal",
                     "Accompagner de pommes de terre grenailles et légumes de saison"),

                    ("Crème brûlée à la vanille bourbon",
                     "Dessert classique avec une touche de vanille de Madagascar",
                     "Dessert",
                     "Peut être décliné en différentes saveurs : café, lavande, etc.")
                };

                using (var insertCommand = new SqliteCommand(
                    "INSERT INTO IdeesPlats (Titre, Description, Categorie, Notes) VALUES (@Titre, @Description, @Categorie, @Notes);",
                    connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Titre", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Description", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Categorie", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@Notes", SqliteType.Text));

                    foreach (var idee in idees)
                    {
                        insertCommand.Parameters["@Titre"].Value = idee.Titre;
                        insertCommand.Parameters["@Description"].Value = idee.Description;
                        insertCommand.Parameters["@Categorie"].Value = idee.Categorie;
                        insertCommand.Parameters["@Notes"].Value = idee.Notes;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode pour insérer des services de test dans la table Services.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
        private static void SeedServices(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Services;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var services = new (string Date, string MidiSoir, int MenuId, string Statut)[]
                {
                    ("2024-12-10 12:00:00", "Midi", 1, "Ouvert"),
                    ("2024-12-10 19:00:00", "Soir", 2, "Ouvert"),
                    ("2024-12-11 12:00:00", "Midi", 3, "Ouvert"),
                    ("2024-12-11 19:00:00", "Soir", 1, "Complet"),
                    ("2024-12-12 12:00:00", "Midi", 2, "Ouvert")
                };

                using (var insertCommand = new SqliteCommand(
                    "INSERT INTO Services (Date, MidiSoir, MenuId, Statut) VALUES (@Date, @MidiSoir, @MenuId, @Statut);",
                    connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@Date", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@MidiSoir", SqliteType.Text));
                    insertCommand.Parameters.Add(new SqliteParameter("@MenuId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@Statut", SqliteType.Text));

                    foreach (var service in services)
                    {
                        insertCommand.Parameters["@Date"].Value = service.Date;
                        insertCommand.Parameters["@MidiSoir"].Value = service.MidiSoir;
                        insertCommand.Parameters["@MenuId"].Value = service.MenuId;
                        insertCommand.Parameters["@Statut"].Value = service.Statut;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Méthode pour insérer des réservations de test dans la table Reservations.
        /// </summary>
        /// <param name="connection">connexion a la db</param>
        private static void SeedReservations(SqliteConnection connection)
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (var countCommand = new SqliteCommand("SELECT COUNT(*) FROM Reservations;", connection, transaction))
                {
                    long count = (long)(countCommand.ExecuteScalar() ?? 0);
                    if (count > 0)
                    {
                        transaction.Commit();
                        return;
                    }
                }

                var reservations = new (int ServiceId, int ClientId, int NbCouverts)[]
                {
                    (1, 1, 2),
                    (1, 2, 4),
                    (2, 3, 2),
                    (3, 4, 3),
                    (4, 5, 2),
                    (4, 1, 4)
                };

                using (var insertCommand = new SqliteCommand(
                    "INSERT INTO Reservations (ServiceId, ClientId, NbCouverts) VALUES (@ServiceId, @ClientId, @NbCouverts);",
                    connection, transaction))
                {
                    insertCommand.Parameters.Add(new SqliteParameter("@ServiceId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@ClientId", SqliteType.Integer));
                    insertCommand.Parameters.Add(new SqliteParameter("@NbCouverts", SqliteType.Integer));

                    foreach (var reservation in reservations)
                    {
                        insertCommand.Parameters["@ServiceId"].Value = reservation.ServiceId;
                        insertCommand.Parameters["@ClientId"].Value = reservation.ClientId;
                        insertCommand.Parameters["@NbCouverts"].Value = reservation.NbCouverts;
                        insertCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }
    }
}