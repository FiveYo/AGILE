using System;

namespace FastDelivery_Library
/// <summary>
/// 3 Classes différentes pour 3 types d'exceptions différentes
/// </summary>
{
    /// <summary>
    /// Classe d'exception liée aux erreurs de stream (Utilisée pour charger un fichier XML)
    /// </summary>
    public class Exception_Stream : Exception
    {
        public Exception_Stream() : base() { }
        public Exception_Stream(string message) : base(message) { }
        public Exception_Stream(string message, System.Exception inner) : base(message, inner) { }
    }
    /// <summary>
    /// Classe d'exception liée aux erreurs de parsage XML
    /// </summary>
    public class Exception_XML : Exception
    {
        public Exception_XML() : base() { }
        public Exception_XML(string message) : base(message) { }
        public Exception_XML(string message, System.Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Classe d'exception liée à l'arrêt d'un calcul
    /// </summary>
    public class Exception_STOP : Exception
    {
        public Exception_STOP() : base() { }
        public Exception_STOP(string message) : base(message) { }
        public Exception_STOP(string message, System.Exception inner) : base(message, inner) { }
    }

}

