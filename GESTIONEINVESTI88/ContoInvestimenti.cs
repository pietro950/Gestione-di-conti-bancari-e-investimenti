using System;
using System.Collections.Generic;
using System.Linq;

namespace GESTIONEINVESTI88
{
    public class ContoInvestimenti : ContoBancario //classe derivata
    {
        public Dictionary<string, double> Investimenti { get; set; } // Asset -> Importo investito

        public static Dictionary<string, double> RendimentiAsset = new Dictionary<string, double>()
        {
            { "NASDAQ", 0.03 },
            { "S&P 500", 0.025 },
            { "FTSE 100", 0.015 },
            { "DAX 40", 0.028 },
            { "Oro", 0.01 },
            { "Argento", 0.007 },
            { "Petrolio", 0.022 },
            { "CryptoCoin", 0.08 },
            { "GlobalTech ETF", 0.035 },
            { "Bond Governativo", 0.012 }
        };

        public ContoInvestimenti()  //costruttore senza parametri
        {
            Investimenti = new Dictionary<string, double>();
        }

        public ContoInvestimenti(string nome, string cognome, string iban, double saldoIniziale)
            : base(nome, cognome, iban, saldoIniziale)   //  costruttore 
        {
            Investimenti = new Dictionary<string, double>();
        }

        // Investi(asset, importo) OVERLOADING
        public void Investi(string asset, double importo)
        {
            if (string.IsNullOrEmpty(asset) || !RendimentiAsset.ContainsKey(asset))
                throw new ArgumentException("Asset non valido");

            if (importo <= 0)
                throw new ArgumentException("L'importo deve essere positivo");

            if (importo > Saldo)
                throw new ArgumentException("Saldo insufficiente per l'investimento");

            Saldo -= importo;

            if (Investimenti.ContainsKey(asset))
                Investimenti[asset] += importo;
            else
                Investimenti[asset] = importo;
        }

        // Investi(importo) Investimento generico senza specificare asset
        public void Investi(double importo)
        {
            if (importo <= 0)
                throw new ArgumentException("L'importo deve essere positivo");

            if (importo > Saldo)
                throw new ArgumentException("Saldo insufficiente per l'investimento");

            Saldo -= importo;

            // Distribuisce l'importo uniformemente su tutti gli asset
            double importoPerAsset = importo / RendimentiAsset.Count;
            foreach (var asset in RendimentiAsset.Keys)
            {
                if (Investimenti.ContainsKey(asset))
                    Investimenti[asset] += importoPerAsset;
                else
                    Investimenti[asset] = importoPerAsset;
            }
        }

        // Investi(asset, importo, commissione)
        public void Investi(string asset, double importo, double commissione)
        {
            if (commissione < 0 || commissione > 1)
                throw new ArgumentException("La commissione deve essere tra 0 e 1 (0-100%)");

            double importoTotale = importo + (importo * commissione);

            if (importoTotale > Saldo)
                throw new ArgumentException("Saldo insufficiente per l'investimento con commissione");

            // Applica la prima versione (senza commissione nell'investimento)
            Investi(asset, importo);

            // Sottrae la commissione dal saldo
            Saldo -= importo * commissione;
        }

        public override void Preleva(double importo)
        {
            
            base.Preleva(importo);
        }

        public override double CalcolaValoreTotale() //OVERRIDE cioè utilizza un metodo della classe base e aggiunge 
        {
            double valoreInvestimenti = 0;

            foreach (var kvp in Investimenti)
            {
                string asset = kvp.Key;
                double importo = kvp.Value;

                if (RendimentiAsset.ContainsKey(asset))
                {
                    double rendimento = RendimentiAsset[asset];
                    valoreInvestimenti += importo * (1 + rendimento);
                }
            }

            return Saldo + valoreInvestimenti;
        }

        public override string ToString()//OVERRIDE (metodo della classe base che viene ridefinito nella classe derivata)
        {
            string investimentiStr = string.Join("; ",
                Investimenti.Select(x => $"{x.Key}: €{x.Value:F2}")); // crea una stringa che rappresenta gli investimenti

            if (string.IsNullOrEmpty(investimentiStr))
                investimentiStr = "Nessun investimento";

            return $"{Nome} {Cognome} | IBAN: {IBAN} | Saldo: €{Saldo:F2} | Investimenti: {investimentiStr} | Valore Totale: €{CalcolaValoreTotale():F2}";
        }
    }
}
