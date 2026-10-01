using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GESTIONEINVESTI88
{
    public class Banca //classe contenitore 
    {
        private List<ContoBancario> contiRuntime = new List<ContoBancario>();

        public void AggiungiConto(ContoBancario conto)
        {
            if (conto == null)
                throw new ArgumentException("Il conto non può essere null");

            contiRuntime.Add(conto);
        }

        public void RimuoviConto(int indice)
        {
            if (indice < 0 || indice >= contiRuntime.Count)
                throw new ArgumentException("Indice non valido");

            contiRuntime.RemoveAt(indice);
        }

        public List<ContoBancario> OttieniConti() //restituisce la lista dei conti correnti 
        {
            return contiRuntime;
        }

        public void SalvaInJson(string percorsoFile)
        {
            var contiDaSalvare = new List<Dictionary<string, object>>();

            foreach (var conto in contiRuntime)
            {
                var contoJson = new Dictionary<string, object>
                {
                    { "tipo", conto is ContoInvestimenti ? "ContoInvestimenti" : "ContoBancario" },
                    { "nome", conto.Nome },
                    { "cognome", conto.Cognome },
                    { "iban", conto.IBAN },
                    { "saldo", conto.Saldo }
                };

                if (conto is ContoInvestimenti contoInvestimenti)
                    contoJson["investimenti"] = contoInvestimenti.Investimenti;

                contiDaSalvare.Add(contoJson);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(contiDaSalvare, options);
            File.WriteAllText(percorsoFile, json);
        }

        public void CaricaDaJson(string percorsoFile)
        {
            if (!File.Exists(percorsoFile))
                throw new Exception("Il file non esiste");

            string json = File.ReadAllText(percorsoFile);
            var conti = new List<ContoBancario>();

            using (var document = JsonDocument.Parse(json))
            {
                foreach (var contoElement in document.RootElement.EnumerateArray())
                {
                    string tipo = contoElement.GetProperty("tipo").GetString();
                    string nome = contoElement.GetProperty("nome").GetString();
                    string cognome = contoElement.GetProperty("cognome").GetString();
                    string iban = contoElement.GetProperty("iban").GetString();
                    double saldo = contoElement.GetProperty("saldo").GetDouble();

                    if (tipo == "ContoInvestimenti")
                    {
                        var contoInvestimenti = new ContoInvestimenti(nome, cognome, iban, saldo);

                        if (contoElement.TryGetProperty("investimenti", out var investimentiElement))
                        {
                            foreach (var investimento in investimentiElement.EnumerateObject())
                                contoInvestimenti.Investimenti[investimento.Name] = investimento.Value.GetDouble();
                        }

                        conti.Add(contoInvestimenti);
                    }
                    else
                    {
                        conti.Add(new ContoBancario(nome, cognome, iban, saldo));
                    }
                }
            }

            contiRuntime = conti;
        }
    }
}
