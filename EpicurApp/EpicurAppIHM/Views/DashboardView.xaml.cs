using EpicurAPP_Partage.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace EpicurAppIHM.Views
{

    /// <summary>
    /// Classe simple pour afficher un client avec ses stats
    /// </summary>
    public class ClientAvecStats
    {
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string InfoVisites { get; set; }
    }
    /// <summary>
    /// Page d'accueil avec tableau de bord et statistiques
    /// </summary>
    public partial class DashboardView : UserControl
    {
        // Client HTTP pour appeler l'API
        private readonly HttpClient _httpClient;

        // Listes de clients
        private List<Client> tousLesClients;
        private List<Client> clientsReguliers;
        private List<Client> clientsInactifs;

        /// <summary>
        /// Constructeur - initialise la page
        /// </summary>
        public DashboardView()
        {
            InitializeComponent();
            _httpClient = App.ApiClient.HttpClient;
            tousLesClients = new List<Client>();
            clientsReguliers = new List<Client>();
            clientsInactifs = new List<Client>();

            // Charger les données au démarrage
            ChargerDonnees();
            
        }

        /// <summary>
        /// Rafraîchit les données quand la page devient visible
        /// </summary>
        public void Rafraichir()
        {
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
                tousLesClients = await _httpClient.GetFromJsonAsync<List<Client>>("Client");
                if (tousLesClients == null) tousLesClients = new List<Client>();

                // Récupérer les clients réguliers (3+ visites)
                clientsReguliers = await _httpClient.GetFromJsonAsync<List<Client>>("Client/Reguliers");
                if (clientsReguliers == null) clientsReguliers = new List<Client>();

                // Récupérer les clients inactifs (60+ jours sans visite)
                clientsInactifs = await _httpClient.GetFromJsonAsync<List<Client>>("Client/Inactifs");
                if (clientsInactifs == null) clientsInactifs = new List<Client>();

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
            TxtTotalClients.Text = tousLesClients.Count.ToString();

            // Carte 2 : Réservations du mois (on doit encore compter)
            int repasDuMois = 0;
            DateTime debutMois = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            foreach (Client client in tousLesClients)
            {
                try
                {
                    List<Repas> repasClient = await _httpClient.GetFromJsonAsync<List<Repas>>($"Client/{client.Id}/repas");

                    if (repasClient != null)
                    {
                        foreach (Repas repas in repasClient)
                        {
                            if (repas.Date >= debutMois)
                            {
                                repasDuMois++;
                            }
                        }
                    }
                }
                catch
                {
                    // Si erreur, on passe au suivant
                }
            }

            TxtReservationsMois.Text = repasDuMois.ToString();

            // Carte 3 : Clients inactifs (utilise le nouvel endpoint !)
            TxtClientsInactifs.Text = clientsInactifs.Count.ToString();
        }

        /// <summary>
        /// Affiche le graphique des 10 meilleurs clients (par nombre de visites)
        /// </summary>
        private async void AfficherGraphiqueTopClients()
        {
            List<string> nomsClients = new List<string>();
            List<int> nombreRepas = new List<int>();

            // Déterminer la date de début selon le filtre
            DateTime? dateDebut = null;

            if (CbFiltrePeriode != null && CbFiltrePeriode.SelectedItem != null)
            {
                ComboBoxItem itemSelectionne = (ComboBoxItem)CbFiltrePeriode.SelectedItem;
                string filtre = itemSelectionne.Content.ToString();

                if (filtre == "Ce mois-ci")
                {
                    dateDebut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
                else if (filtre == "Cette année")
                {
                    dateDebut = new DateTime(DateTime.Now.Year, 1, 1);
                }
                else if (filtre == "Derniers 30 jours")
                {
                    dateDebut = DateTime.Now.AddDays(-30);
                }
                else if (filtre == "Derniers 90 jours")
                {
                    dateDebut = DateTime.Now.AddDays(-90);
                }
                // Si "Tout", dateDebut reste null = pas de filtre
            }

            // Utiliser TOUS les clients
            foreach (Client client in tousLesClients)
            {
                try
                {
                    List<Repas> repasClient = await _httpClient.GetFromJsonAsync<List<Repas>>($"Client/{client.Id}/repas");

                    if (repasClient != null && repasClient.Count > 0)
                    {
                        // Compter les repas selon le filtre
                        int compteur = 0;

                        if (dateDebut == null)
                        {
                            // Pas de filtre = tous les repas
                            compteur = repasClient.Count;
                        }
                        else
                        {
                            // Filtrer par date
                            foreach (Repas repas in repasClient)
                            {
                                if (repas.Date >= dateDebut)
                                {
                                    compteur++;
                                }
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
                        int tempNombre = nombreRepas[j];
                        nombreRepas[j] = nombreRepas[j + 1];
                        nombreRepas[j + 1] = tempNombre;

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
            DataLabels = true,
            LabelPoint = point => point.Y.ToString("0")
        }
    };

            ChartPrincipal.Series = series;
            ChartPrincipal.AxisX[0].Labels = nomsClients.ToArray();
        }

        /// <summary>
        /// Affiche la liste des clients inactifs (utilise le nouvel endpoint !)
        /// </summary>
        /// <summary>
        /// Affiche la liste des clients inactifs avec leurs stats
        /// </summary>
        private async void AfficherClientsInactifs()
        {
            List<ClientAvecStats> listeAffichage = new List<ClientAvecStats>();

            foreach (Client client in clientsInactifs)
            {
                int nbVisites = 0;
                string derniereVisite = "Jamais";

                try
                {
                    List<Repas> repasClient = await _httpClient.GetFromJsonAsync<List<Repas>>($"Client/{client.Id}/repas");

                    if (repasClient != null && repasClient.Count > 0)
                    {
                        nbVisites = repasClient.Count;

                        // Trouver la dernière visite
                        DateTime derniere = repasClient[0].Date;
                        foreach (Repas repas in repasClient)
                        {
                            if (repas.Date > derniere)
                            {
                                derniere = repas.Date;
                            }
                        }
                        derniereVisite = derniere.ToString("dd/MM/yyyy");
                    }
                }
                catch
                {
                    // Si erreur, on garde les valeurs par défaut
                }

                ClientAvecStats clientStats = new ClientAvecStats
                {
                    Prenom = client.Prenom,
                    Nom = client.Nom,
                    Email = client.Email,
                    InfoVisites = nbVisites + " visite(s) - Dernière : " + derniereVisite
                };

                listeAffichage.Add(clientStats);
            }

            ListeClientsInactifs.ItemsSource = listeAffichage;
        }

        /// <summary>
        /// Événement quand on change le filtre de période
        /// </summary>
        private void FiltrePeriode_Changed(object sender, SelectionChangedEventArgs e)
        {
            // Ne rien faire si pas encore initialisé
            if (CbFiltrePeriode.SelectedItem == null) return;
            if (tousLesClients == null) return;

            // Recharger le graphique avec le nouveau filtre
            AfficherGraphiqueTopClients();
        }
    }
}