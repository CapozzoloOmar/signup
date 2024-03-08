using System.ComponentModel.DataAnnotations;

public class Login
{

    public DateOnly DataNascita { get; set; }
    public string Nome { get; set; }
    public string Cognome { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    // Aggiungi altri campi necessari per l'autenticazione, ad esempio nome, cognome, etc.
    public string Sesso { get; set; }
    public string Ruolo { get; set; }
}
