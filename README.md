# QuizApp (in lucru)

## Requirements

- .NET 5.0 sau mai recent
- Visual Studio sau alt IDE pentru C#

## Structura Proiectului

- **QuizApp/QuizTaker**: Aplicația client pentru utilizatori. Permite utilizatorilor să răspundă la întrebări din quizuri.
  - `MainWindow.xaml` și `MainWindow.xaml.cs`: Interfața principală pentru utilizatori. Introduce numele de utilizator și așteaptă quizul.
  - `QuizWindow.xaml` și `QuizWindow.xaml.cs`: Interfața pentru quiz. Afișează întrebările și răspunsurile, calculează și afișează scorul.
  - `ViewModels/QuizViewModel.cs`: Logica de business pentru gestionarea întrebărilor, răspunsurilor și scorurilor utilizatorilor.
  - `Services/CommunicationService.cs`: Gestionarea comunicării în timp real cu serverul prin WebSockets.

- **QuizApp/Evaluator**: Aplicația server pentru evaluatori. Permite evaluatorilor să creeze și să trimită quizuri utilizatorilor, și să monitorizeze scorurile.
  - `MainWindow.xaml` și `MainWindow.xaml.cs`: Interfața principală pentru evaluatori. Listează utilizatorii și permite trimiterea quizurilor.
  - `Views/QuizCreationWindow.xaml` și `Views/QuizCreationWindow.xaml.cs`: Interfața pentru crearea quizurilor noi.
  - `ViewModels/EvaluatorViewModel.cs`: Logica de business pentru gestionarea utilizatorilor și quizurilor.
  - `Services/CommunicationService.cs`: Gestionarea comunicării în timp real cu clienții prin WebSockets.

- **QuizApp/Server**: Serverul central care gestionează comunicarea dintre clienți și evaluatori.
  - `Program.cs`: Punctul de intrare principal pentru server. Inițializează și gestionează conexiunile WebSocket.

## Funcționalități Implementate

- Încărcarea întrebărilor și răspunsurilor din fișiere JSON.
- Utilizare WebSockets pentru comunicare în timp real între clienți și server.
- Interfață grafică pentru utilizatori și evaluatori.
- Calcularea și salvarea automată a scorurilor.

## TODO: Aplicatia de Evaluator nu asculta corect mesajele de la server (FIX)
