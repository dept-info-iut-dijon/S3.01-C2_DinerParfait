using EpicurAPP_Partage.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Page d'accueil avec tableau de bord et statistiques
    /// </summary>
    public partial class DashboardView : UserControl
    {
        // Client HTTP pour appeler l'API
        private readonly HttpClient _httpClient;

        // Liste de tous les clients
        private List<Client> clients;

        // Variable pour savoir quel graphique on affiche
        //private bool afficherTopClients = true;

        /// <summary>
        /// Constructeur - initialise la page
        /// </summary>
        public DashboardView()
        {
            InitializeComponent();
            _httpClient = App.ApiClient.HttpClient;
            clients = new List<Client>();

            // Charger les données au démarrage
            ChargerDonnees();
        }

        /// <summary>
        /// Charge toutes les données depuis l'API
        /// </summary>
        private async void ChargerDonnees()
        {
            try
            {
                // Récupérer tous les clients
                clients = await _httpClient.GetFromJsonAsync<List<Client>>("Client");

                if (clients == null)
                {
                    clients = new List<Client>();
                }

                // Afficher les statistiques
                AfficherStatistiques();

                // Afficher le graphique des top clients
                AfficherGraphiqueTopClients();

                // Afficher la liste des clients inactifs
                AfficherClientsInactifs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des données : " + ex.Message,
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Affiche les statistiques dans les 3 cartes en haut
        /// </summary>
        private async void AfficherStatistiques()
        {
            // Carte 1 : Total de clients
            TxtTotalClients.Text = clients.Count.ToString();

            // Carte 2 et 3 : on doit compter les repas
            int repasDuMois = 0;
            int clientsInactifs = 0;
            DateTime dateUnAnAvant = DateTime.Now.AddYears(-1);
            DateTime debutMois = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Pour chaque client, récupérer ses repas
            foreach (Client client in clients)
            {
                try
                {
                    List<Repas> repasClient = await _httpClient.GetFromJsonAsync<List<Repas>>($"Client/{client.Id}/repas");

                    if (repasClient == null || repasClient.Count == 0)
                    {
                        // Pas de repas = client inactif
                        clientsInactifs++;
                    }
                    else
                    {
                        // Compter les repas du mois
                        foreach (Repas repas in repasClient)
                        {
                            if (repas.Date >= debutMois)
                            {
                                repasDuMois++;
                            }
                        }

                        // Vérifier si le client est inactif (dernier repas > 1 an)
                        Repas dernierRepas = repasClient[0];
                        foreach (Repas repas in repasClient)
                        {
                            if (repas.Date > dernierRepas.Date)
                            {
                                dernierRepas = repas;
                            }
                        }

                        if (dernierRepas.Date < dateUnAnAvant)
                        {
                            clientsInactifs++;
                        }
                    }
                }
                catch
                {
                    // Si erreur, on considère le client comme inactif
                    clientsInactifs++;
                }
            }

            TxtReservationsMois.Text = repasDuMois.ToString();
            TxtClientsInactifs.Text = clientsInactifs.ToString();
        }

        /// <summary>
        /// Affiche le graphique des 10 meilleurs clients
        /// </summary>
        private async void AfficherGraphiqueTopClients()
        {
            // Liste pour stocker le nombre de repas par client
            List<string> nomsClients = new List<string>();
            List<int> nombreRepas = new List<int>();

            DateTime dateDebutAnnee = DateTime.Now.AddYears(-1);

            // Pour chaque client, compter ses repas sur l'année
            foreach (Client client in clients)
            {
                try
                {
                    List<Repas> repasClient = await _httpClient.GetFromJsonAsync<List<Repas>>($"Client/{client.Id}/repas");

                    if (repasClient != null)
                    {
                        // Compter les repas de l'année
                        int compteur = 0;
                        foreach (Repas repas in repasClient)
                        {
                            if (repas.Date >= dateDebutAnnee)
                            {
                                compteur++;
                            }
                        }

                        if (compteur > 0)
                        {
                            nomsClients.Add(client.Prenom + " " + client.Nom);
                            nombreRepas.Add(compteur);
                        }
                    }
                }
                catch
                {
                    // Si erreur, on passe au client suivant
                }
            }

            // Trier pour garder les 10 meilleurs (tri à bulles simple)
            for (int i = 0; i < nombreRepas.Count - 1; i++)
            {
                for (int j = 0; j < nombreRepas.Count - 1 - i; j++)
                {
                    if (nombreRepas[j] < nombreRepas[j + 1])
                    {
                        // Échanger les nombres
                        int tempNombre = nombreRepas[j];
                        nombreRepas[j] = nombreRepas[j + 1];
                        nombreRepas[j + 1] = tempNombre;

                        // Échanger les noms
                        string tempNom = nomsClients[j];
                        nomsClients[j] = nomsClients[j + 1];
                        nomsClients[j + 1] = tempNom;
                    }
                }
            }

            // Garder seulement les 10 premiers
            if (nomsClients.Count > 10)
            {
                nomsClients = nomsClients.GetRange(0, 10);
                nombreRepas = nombreRepas.GetRange(0, 10);
            }

            // Créer le graphique
            ChartValues<double> valeurs = new ChartValues<double>();
            foreach (int nombre in nombreRepas)
            {
                valeurs.Add(nombre);
            }

            SeriesCollection series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Réservations",
                    Values = valeurs,
                    Fill = new SolidColorBrush(Color.FromRgb(139, 21, 56)),
                    DataLabels = true
                }
            };

            ChartPrincipal.Series = series;
            ChartPrincipal.AxisX[0].Labels = nomsClients.ToArray();
        }

        /// <summary>
        /// Affiche le graphique des 10 plats les plus servis
        /// </summary>
        /// <summary>
        /// Affiche le graphique des 10 plats les plus servis
        /// </summary>
        
        /// <summary>
        /// Méthode pour compter un plat dans le dictionnaire
        /// </summary>
        private void CompterPlat(Plat plat, Dictionary<int, int> compteur, Dictionary<int, string> noms)
        {
            if (compteur.ContainsKey(plat.Id))
            {
                compteur[plat.Id]++;
            }
            else
            {
                compteur[plat.Id] = 1;
                noms[plat.Id] = plat.Nom;
            }
        }

        /// <summary>
        /// Affiche la liste des clients inactifs en bas
        /// </summary>
        private async void AfficherClientsInactifs()
        {
            List<Client> clientsInactifs = new List<Client>();
            DateTime dateUnAnAvant = DateTime.Now.AddYears(-1);

            foreach (Client client in clients)
            {
                try
                {
                    List<Repas> repasClient = await _httpClient.GetFromJsonAsync<List<Repas>>($"Client/{client.Id}/repas");

                    if (repasClient == null || repasClient.Count == 0)
                    {
                        clientsInactifs.Add(client);
                    }
                    else
                    {
                        // Trouver le dernier repas
                        Repas dernierRepas = repasClient[0];
                        foreach (Repas repas in repasClient)
                        {
                            if (repas.Date > dernierRepas.Date)
                            {
                                dernierRepas = repas;
                            }
                        }

                        if (dernierRepas.Date < dateUnAnAvant)
                        {
                            clientsInactifs.Add(client);
                        }
                    }
                }
                catch
                {
                    clientsInactifs.Add(client);
                }
            }

            ListeClientsInactifs.ItemsSource = clientsInactifs;
        }

        
    }
}