using Soneta.Business;
using Soneta.Business.App;
using Soneta.Business.UI;

// rejestracja serwisu implementującego interfejs ILoginListenerUI
// dla wszystkich zarejestrowanych w ten sposób serwisów zostanie podczas logowania zawołana metoda AfterLoginResult
[assembly: Service(typeof(ILoginListenerUI), typeof(Soneta.AfterLoginTestUISample.Registration), ServiceScope.Login, Priority = 200)]

namespace Soneta.AfterLoginTestUISample
{
    // obiekt klasy AfterLoginTestUISample zostanie wyświetlony po zalogowaniu do systemu
    //prosty obiekt dla którego będzie wyświetlana odpowiednia informacja
    public class AfterLoginTestUISample 
    {
        [Context]
        //to jest autmoatyczna właściwość pozwalająca na przekazanie bieżącego Context-u 
        public Context Context { get; set; }

        public string Komunikat
        {
            get { return "Przykładowy komunikat po zalogowaniu"; }
        }
    }
    // klasa Registration implementuje intefejs ILoginListenerUI
    // podczas logowania zostanie zawołana metoda AfterLoginResult dla tej klasy
    public class Registration : ILoginListenerUI
    {
        // "techniczna" deklaracja zmiennej 
        // ze względu na formularze (pageform.xml) dodatek musi posiadać referencję do Soneta.Types
        Soneta.Types.Currency c1 = Soneta.Types.Currency.Zero;

        // implementacja metody z interfejsu ILoginListenerUI
        // przekazany obiekt klasy AfterLoginResultArgs zawiera listę obiektów, które zostaną otwarte po zalogowaniu
        // dodanie do listy obiektu klasy AfterLoginTestUISample oznacza, 
        // że po zalogowaniu system otworzy odpowiedni formularz dla tego obiektu (AfterLoginTestUISample.General.pageform.xml)
        void ILoginListenerUI.AfterLoginResult(AfterLoginResultArgs args)
        {
            //warunek, jeżeli użytkownikiem jest administrator zostanie dla niego wyświetlony komunikat
            if (args.Login.UserName == "Administrator")
                args.Values.Add(new AfterLoginTestUISample());
        }
    }
}



