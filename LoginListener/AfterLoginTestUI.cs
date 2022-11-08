using System;
using System.Collections;
using Soneta.Business;
using Soneta.Business.App;
using Soneta.Types;
using Soneta.Business.UI;
using Soneta.AfterLoginTestUI;

// rejestracja serwisu implementującego interfejs ILoginListenerUI
// dla wszystkich zarejestrowanych w ten sposób serwisów zostanie podczas logowania zawołana metoda AfterLoginResult
[assembly: Service(typeof(ILoginListenerUI), typeof(Soneta.AfterLoginTestUI.Registration), ServiceScope.Login)]


namespace Soneta.AfterLoginTestUI
{
    // Obiekt klasy AfterLoginTestUI zostanie dodany do listy obiektów wyświetlanych po zalogowaniu
    // Ponieważ klasa AfterLoginTestUI implementuje interfejs IVerifiable, możemy zweryfikować poprawność wpisanych danych
    // Weryfikacja polega na porównaniu wpisanej wartości Control z losowo ustaloną wartością Pattern
    [DataFormStyle(UseDialog = true)]
    [Caption("Weryfikacja danych logowania")]
    public class AfterLoginTestUI : IVerifiable, IContextable
    {
        // "techniczna" deklaracja zmiennej Currency - ze względu na formularze (pageform.xml) dodatek musi posiadać referencję do Soneta.Types
        Soneta.Types.Currency c1 = Soneta.Types.Currency.Zero;

        string pattern;
        string control;
        Context context;

        public Context Context => context;

        public AfterLoginTestUI(Context cx)
        {
            this.context = cx;

            // Dodajemy weryfikator do listy weryfikatorów w sesji
            // Na akceptacji okna weryfikator zostanie aktywowany - zostanie sprawdzone, czy wartość została poprawnie wpisana
            this.context.Session.Verifiers.Add(new PoleVerifier(this));

            // jako wartość wzorcową losujemy liczbę z zakresu 1-1000
            pattern = (new Random().Next() % 1000 + 1).ToString();
        }

        //właściwość do zmiennej "pattern"
        public string Pattern
        {
            get
            {
                return pattern;
            }
        }
        //właściwość do zmiennej "control" 
        public string Control
        {
            get
            {
                return control;
            }
            set
            {
                control = value;
                this.context.Session.InvokeChanged();
            }
        }

        // Interfejs IVerifiable wymaga implementacji metody GetVerifiers zwracającej kolekcję weryfikatorów
        public IEnumerable GetVerifiers()
        {
            // rzutowanie Context.Session na obiekt IVerifiable i zwrócenie weryfikatorów zarejestrowanych w sesji
            return ((IVerifiable)this.context.Session).GetVerifiers();
        }

        // Klasa weryfikatora, który będzie sprawdzany w teście
        // klasa PoleVerifier dziedziczy po klasie Verifier
        public class PoleVerifier : Verifier
        {

            private readonly AfterLoginTestUI afterLoginTestUI;

            public PoleVerifier(AfterLoginTestUI afterLoginTestUI)
            {
                this.afterLoginTestUI = afterLoginTestUI;
            }

            // implementacja właściwości z klasy bazowej - zwracany komunikat błędu
            public override string Description
            {
                get
                {
                    return String.Format("Wartość kontrolna nie zgadza się ze wzorcem.\nWartość oczekiwana: {0}\nWartość wpisana: {1}", afterLoginTestUI.Pattern, afterLoginTestUI.Control);
                }
            }

            // implementacja właściwości z klasy bazowej - zwracany obiekt afterLoginTestUI
            public override object Source
            {
                get
                {
                    return afterLoginTestUI;
                }
            }

            // Wynik metody wskazuje, czy wpisane dane są poprawne
            // Metoda zwraca true jeśli wpisana wartość kontrolna jest zgodna ze wzorcem
            protected override bool IsValid()
            {
                return (afterLoginTestUI.Control == afterLoginTestUI.Pattern);
            }

            // Wynik metody wskazuje, czy należy przeprowadzić weryfikację
            // Weryfikację przeprowadzamy dopiero na kliknięciu OK na formularzu, czyli na akcji AcceptRow
            protected override bool IsAccept(object data, string property, VerifierAction action)
            {
                return action == VerifierAction.AcceptRow;
            }

        }
    }

    // klasa Registration implementuje intefejs ILoginListenerUI
    // podczas logowania zostanie zawołana metoda AfterLoginResult dla tej klasy
    public class Registration : ILoginListenerUI
    {
        // implementacja metody z interfejsu ILoginListenerUI
        // przekazany obiekt klasy AfterLoginResultArgs zawiera listę obiektów, które zostaną otwarte po zalogowaniu
        // dodanie do listy obiektu klasy AfterLoginTestUI oznacza, 
        // że po zalogowaniu system otworzy odpowiedni formularz dla tego obiektu (AfterLoginTestUI.General.pageform.xml)
        void ILoginListenerUI.AfterLoginResult(AfterLoginResultArgs args)
        {
            //dopisanie obiektu klasy AfterLoginTestUI do odpowiedniego kontera wraz z inicjalizacją pola Context
            args.Values.Add(new AfterLoginTestUI(args.Context));
        }
    }
}