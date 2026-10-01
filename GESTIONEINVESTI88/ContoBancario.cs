using System;
using System.Collections.Generic;

namespace GESTIONEINVESTI88
{
    public class ContoBancario //Classe base
    {
        // Identificativo assegnato dal database SQLite.
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string IBAN { get; set; }
        public double Saldo { get; set; }

        public ContoBancario()
        {
        }

        public ContoBancario(string nome, string cognome, string iban, double saldoIniziale) //costruttore con parametri 
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cognome))
                throw new ArgumentException("Nome e Cognome sono obbligatori");

            if (string.IsNullOrEmpty(iban) || iban.Length != 27)
                throw new ArgumentException("L'IBAN deve essere lungo esattamente 27 caratteri");

            if (saldoIniziale < 0)
                throw new ArgumentException("Il saldo iniziale non può essere negativo");

            Nome = nome;
            Cognome = cognome;
            IBAN = iban;
            Saldo = saldoIniziale;
        }

        public void Deposita(double importo)
        {
            if (importo <= 0)
                throw new ArgumentException("L'importo deve essere positivo");

            Saldo += importo;
        }

        public virtual void Preleva(double importo)
        {
            if (importo <= 0)
                throw new ArgumentException("L'importo deve essere positivo");

            if (importo > Saldo)
                throw new ArgumentException("Saldo insufficiente");

            Saldo -= importo;
        }

        public virtual double CalcolaValoreTotale()
        {
            return Saldo;
        }

        public override string ToString()
        {
            return $"{Nome} {Cognome} | IBAN: {IBAN} | Saldo: €{Saldo:F2} | Valore Totale: €{CalcolaValoreTotale():F2}";
        }
    }
}
