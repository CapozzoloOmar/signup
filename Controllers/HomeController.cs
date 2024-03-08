using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nuova_cartella.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Nuova_cartella.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly dbContext _db;

        public HomeController(ILogger<HomeController> logger, dbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public List<Purchase> Carrelli
        {
            get
            {
                var carrelliString = HttpContext.Session.GetString("Carrelli");
                return carrelliString != null ? JsonConvert.DeserializeObject<List<Purchase>>(carrelliString) : new List<Purchase>();
            }
            set
            {
                HttpContext.Session.SetString("Carrelli", JsonConvert.SerializeObject(value));
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost] // Gestione dati
        public IActionResult Riepilogo(Registrazione registrazione)
        {
            _db.Registrazioni.Add(registrazione);
            _db.SaveChanges();
            return View(_db.Registrazioni.ToList());
        }
        [HttpGet]
        public IActionResult Login(){
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login login){

            var utenteRegistrato = _db.Registrazioni;
            foreach(var item in utenteRegistrato){
                //Console.WriteLine("Data nel database: " + item.Data.ToString("dd/MM/yyyy"));
                //Console.WriteLine("Data immessa: " + login.DataNascita.ToString("dd/MM/yyyy"));
                if(item.Data.Equals(login.DataNascita)&&item.nome == login.Nome&&item.cognome == login.Cognome&&item.sesso == login.Sesso&&item.ruolo == login.Ruolo&&item.email == login.Email&&item.RegistrazioneId==10){
                    HttpContext.Session.SetString("CanAccessPurchasePage", "true");
                    HttpContext.Session.SetString("CanAccessPurchasePage2", "true");
                    break;
                }
                else{
                    return Redirect("\\home\\SignUp");
                }
            }
                
                return View(login);
            }

       
        [HttpGet] // Mostra la pagina SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Purchase()
        {
            var prodotti = _db.Purchase.ToList();
            return View(prodotti);
        }

        [HttpPost]
        public IActionResult Purchase(Purchase p)
        {
            return View(p);
        }

        [HttpPost]
        public IActionResult Cart(string prodottoId, int quantitaDesiderata)
        {
            var selectedProduct = _db.Purchase.FirstOrDefault(p => p.PurchaseId == prodottoId);

            if (selectedProduct != null && quantitaDesiderata > 0 && quantitaDesiderata <= selectedProduct.quantita)
            {
                var existingItem = Carrelli.FirstOrDefault(p => p.PurchaseId == prodottoId);

                if (existingItem != null)
                {
                    existingItem.quantita += quantitaDesiderata;
                }
                else
                {
                    var carrello = new Purchase
                    {
                        PurchaseId = selectedProduct.PurchaseId,
                        prodotto = selectedProduct.prodotto,
                        quantita = quantitaDesiderata,
                        prezzo = selectedProduct.prezzo
                    };

                    double totaleElemento = Convert.ToDouble(carrello.prezzo * carrello.quantita);
                    carrello.PrezzoTotale = totaleElemento;

                    var carrelli = Carrelli;
                    carrelli.Add(carrello);
                    Carrelli = carrelli;
                }
            }

            double totaleComplessivo = Carrelli.Sum(item => item.PrezzoTotale);
            ViewData["TotaleComplessivo"] = totaleComplessivo;

            HttpContext.Session.SetString("Carrelli", JsonConvert.SerializeObject(Carrelli));

            return View("Cart", Carrelli);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
