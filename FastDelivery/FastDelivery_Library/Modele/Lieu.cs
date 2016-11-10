namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Interface permettant de lier l'adresse d'entrepot et de livraison
    /// </summary>
    public interface Lieu
    {
        Point adresse { get; set; }
    }
}
